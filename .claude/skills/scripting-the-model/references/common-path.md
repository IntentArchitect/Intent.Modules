# The common path — a typical Domain + Services build

Everything a greenfield build usually needs, in the order you do it. The other references cover the edge cases.

## 0. One query per designer you will edit

```js
// run_designer_query — compact schema + stereotype property contracts, in ONE call
return { schema: await getDesignerSchema(), stereos: await getStereotypeDefinitions() };
```

The compact schema gives package names, containment (`elementTypes`; `!` = must set a type), association rules, stereotype names and mapping-type names; `getStereotypeDefinitions()` gives allowed property values. Add `getReferenceTypes()` when unsure a type exists (e.g. `PagedResult<T>`); use `{ full: true }` only for the whole settings payload.

No package yet? `lookupPackage(name) ?? await createPackage(name, packageType)` (see `packages.md`).

## 1. Domain designer — ONE script

```js
const pkg = lookupPackage("OrderingSystem.Domain");                  // exact name from schema.packages
if (!pkg) throw new Error("No OrderingSystem.Domain package");
const folder = createElementUnder(pkg, "Folder", "Orders");

const typed = (owner, spec, name, type, o = {}) => {
    const el = owner.addChild(spec, name);
    el.setType({ name: type, isNullable: !!o.nullable, isCollection: !!o.collection });
    return el;
};
const attr  = (owner, name, type, o) => typed(owner, "Attribute", name, type, o);
const param = (owner, name, type, o) => typed(owner, "Parameter", name, type, o);

// Types before anything references them: enums, value objects, data contracts.
const status = createElementUnder(folder, "Enum", "OrderStatus");
for (const n of ["Draft", "Confirmed", "Cancelled"]) status.addChild("Enum-Literal", n);
const method = createElementUnder(folder, "Enum", "PaymentMethod");
method.addChild("Enum-Literal", "Cash").setValue("1");                // explicit literal value

const address = createElementUnder(folder, "Value Object", "AddressSnapshot");
for (const n of ["Line1", "City", "PostalCode"]) attr(address, n, "string");

const lineInput = createElementUnder(folder, "Data Contract", "OrderLineInput");
attr(lineInput, "ProductId", "guid");                                // a Data Contract cannot point at a Class
attr(lineInput, "Quantity", "int");

// Entities: no Id / FK attributes — the designer adds them from the associations.
const customer = createElementUnder(folder, "Class", "Customer");
attr(customer, "Name", "string");
const email = attr(customer, "Email", "string", { nullable: true });
const ix = email.ensureStereotype("Index");                          // stereotype first, then its properties
ix.setProperty("UniqueKey", "IX_Customers_Email");
ix.setProperty("IsUnique", true);

const product = createElementUnder(folder, "Class", "Product");
attr(product, "Name", "string");

const order = createElementUnder(folder, "Class", "Order");
attr(order, "Status", "OrderStatus");
attr(order, "Total", "decimal");

const item = createElementUnder(folder, "Class", "OrderItem");
attr(item, "Quantity", "int");
attr(item, "UnitPrice", "decimal");

// Constructors and operations; an operation with no setType is void.
const ctor = order.addChild("Class Constructor", "Order");
param(ctor, "customerId", "guid");
param(ctor, "lines", "OrderLineInput", { collection: true });
const confirm = order.addChild("Operation", "Confirm");
param(confirm, "method", "PaymentMethod");
order.addChild("Operation", "CalculateTotal").setType("decimal");    // return type

// Associations: ALWAYS both multiplicities; read them back.
const links = [];
const link = (src, tgt, sourceMultiplicity, targetMultiplicity, targetName) => {
    const opts = { sourceMultiplicity, targetMultiplicity };
    if (targetName) opts.targetName = targetName;                    // only to tell two links to one type apart
    const a = createAssociation("Association", src.id, tgt.id, opts);
    links.push(`${src.getName()} -> ${a.getName()}: ${a.getSourceEnd().getMultiplicity()} -> ${a.getMultiplicity()}`);
    return a;
};
link(order, item, "1", "1..*");                                      // composition: root owns its children
link(order, customer, "0..*", "1");                                  // between roots: many-to-one from the referencing side
link(item, product, "0..*", "1");
link(order, address, "1", "0..1", "DeliveryAddress");                // value object, owned
link(order, address, "1", "0..1", "BillingAddress");
return links;
```

## 2. Services designer — ONE script

Domain types resolve by name through the package reference (`isReference()` is true on them).

```js
const pkg = lookupPackage("OrderingSystem.Services");
const order = lookupByName("Order", "Class");
const item = lookupByName("OrderItem", "Class");
if (!pkg || !order || !item) throw new Error(`Missing — pkg: ${!!pkg}, Order: ${!!order}, OrderItem: ${!!item}`);
const ctor = order.getChildren("Class Constructor")[0];
const confirmOp = order.getChildren("Operation").find(o => o.getName() === "Confirm");
if (!ctor || !confirmOp) throw new Error(`Missing — ctor: ${!!ctor}, Confirm: ${!!confirmOp}`);

const field = (owner, name, type, o = {}) => {
    const f = owner.addChild("DTO-Field", name);
    f.setType({ name: type, isNullable: !!o.nullable, isCollection: !!o.collection });
    if (o.source) f.ensureStereotype("Parameter Settings").setProperty("Source", o.source);   // "From Route" / "From Query"
    return f;
};
const http = (el, verb, route, code) => {
    const h = el.ensureStereotype("Http Settings");
    h.setProperty("Verb", verb);
    h.setProperty("Route", route);
    if (code) h.setProperty("Success Response Code", code);
};
const folder = createElementUnder(pkg, "Folder", "Orders");

// DTOs before the commands/queries that use them as types.
const itemIn = createElementUnder(folder, "DTO", "CreateOrderItemDto");
field(itemIn, "ProductId", "guid");
field(itemIn, "Quantity", "int");
const itemDto = createElementUnder(folder, "DTO", "OrderItemDto");
field(itemDto, "Quantity", "int");
const orderDto = createElementUnder(folder, "DTO", "OrderDto");
field(orderDto, "Id", "guid");
field(orderDto, "Status", "OrderStatus");                            // domain enums are usable directly
field(orderDto, "Total", "decimal");
field(orderDto, "OrderItems", "OrderItemDto", { collection: true });

// Create → constructor; returns the new id.
const create = createElementUnder(folder, "Command", "CreateOrderCommand");
field(create, "CustomerId", "guid");
field(create, "Lines", "CreateOrderItemDto", { collection: true });
create.setType("guid");                                              // no setType = void
http(create, "POST", "api/orders", "201 (Created)");
createAssociation("Create Entity Action", create.id, ctor.id);

// Update → domain operation; the Query Entity Mapping says which Order to load.
const confirm = createElementUnder(folder, "Command", "ConfirmOrderCommand");
field(confirm, "OrderId", "guid", { source: "From Route" });
field(confirm, "Method", "PaymentMethod");
http(confirm, "POST", "api/orders/{orderId}/confirm");
const ua = createAssociation("Update Entity Action", confirm.id, confirmOp.id);
ua.createAdvancedMapping("Query Entity Mapping").mapEnd(["OrderId"], ["Id"]);

// Get one → Query Entity Action to the Class.
const get = createElementUnder(folder, "Query", "GetOrderByIdQuery");
field(get, "OrderId", "guid", { source: "From Route" });
get.setType("OrderDto");
http(get, "GET", "api/orders/{orderId}");
createAssociation("Query Entity Action", get.id, order.id)
    .createAdvancedMapping("Query Entity Mapping").mapEnd(["OrderId"], ["Id"]);

// List → collection return, and the action's target end must be 0..*.
const list = createElementUnder(folder, "Query", "ListOrdersQuery");
field(list, "Status", "OrderStatus", { nullable: true, source: "From Query" });
list.setType({ name: "OrderDto", isCollection: true });              // or "PagedResult<OrderDto>" if getReferenceTypes() lists it
http(list, "GET", "api/orders");
createAssociation("Query Entity Action", list.id, order.id).setMultiplicity("0..*");

// Project the response DTOs from the entities.
const project = (dto, entity, nestedTypes = {}) => {
    dto.setBasicMapping(entity.id);
    const out = [];
    for (const f of dto.getChildren("DTO-Field")) {
        const n = f.getName();
        const target = nestedTypes[n]
            ? entity.getAssociations().find(a => a.getName() === n)
            : entity.getChildren().find(c => c.getName() === n);
        if (!target) { out.push(`${n}: UNMAPPED`); continue; }
        f.setBasicMapping(target.id);
        if (nestedTypes[n]) f.setType(nestedTypes[n]);               // mapping to an association retypes the field — restore it
        out.push(`${n} -> ${f.getBasicMapping()?.getPath().map(p => p.name).join(".")}`);
    }
    return out;
};
return {
    item: project(itemDto, item),
    order: project(orderDto, order, { OrderItems: { name: "OrderItemDto", isCollection: true } }),
};
```

Delete has the same shape: a `Delete Entity Action` to the Class. Per the Create/Update action rules, wire only what clears validation; add field-by-field data mappings only on request.

## Commonly set stereotypes

Accepted in real runs; confirm names and values against `getStereotypeDefinitions()`.

| On | Stereotype → properties |
| --- | --- |
| Command / Query | `Http Settings` → `Verb` (`GET`/`POST`/`PUT`/`DELETE`), `Route`, `Success Response Code` (e.g. `"201 (Created)"`) |
| Command / Query / package | `Secured` → `Security Roles`: array of Role ids (Roles live under the `Security Configuration` element) |
| DTO-Field | `Parameter Settings` → `Source` (`"From Route"`, `"From Query"`); `Validations` → `Not Empty`: `true`, `Min`: `"1"`, `Email Address`: `true` |
| Domain Attribute | `Index` → `UniqueKey`, `IsUnique` |

## Reading the result

A script returns `executed`, `result` (your `return`), `output`, `changes` (`created`/`updated`/`deleted`) and `errors[]`: what your change introduced — on what you touched first, then elsewhere in the designer (problems already there are only counted, as `preexistingProblems`). Fix those in a short follow-up script; don't follow up with `getDesignerValidationErrors()` or `getDesignerModelStructure()`.

## Pitfalls seen in real runs

1. **A Value Object / Data Contract attribute cannot be typed as a Class** (`"Product" is a Class, which this type reference cannot point to`). Use an id (`ProductId: guid`).
2. **A throw does not roll back.** The steps before it stay applied. Read `changes` and continue from the failure; re-running the whole script duplicates elements.
3. **Basic-mapping a DTO field to an association retypes it** to the entity, giving `Type reference could not be found`. `setType` the nested DTO after mapping.
4. **`await` every read global.** `getStereotypeDefinitions().stereotypeDefinitions` is `undefined`: it's a promise.
5. **No Id or FK attributes on Classes** — the designer manages them. A 1-to-1 gets an FK only when its source end is `0..1`.
6. **Composition (`1 -> *`, `1 -> 1`) runs only from a root to its own child.** Between roots, invert it to many-to-one (`Order 0..* -> 1 Customer`).
7. **List queries need `0..*` on the `Query Entity Action`**; it defaults to 1-to-1.
