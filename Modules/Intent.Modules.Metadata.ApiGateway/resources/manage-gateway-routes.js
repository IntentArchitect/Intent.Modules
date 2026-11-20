/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../typings/core.context.types.d.ts" />
const gatewayVerbColors = { GET: "#61affe", POST: "#49cc90", PUT: "#fca130", PATCH: "#50e3c2", DELETE: "#f93e3e" };
const gatewayMutedColor = "#8b98a5";
const gatewayDriftColor = "#fca130";
function gatewayHttpOf(element) {
    var _a, _b, _c, _d;
    const http = element.getStereotype("Http Settings");
    if (!http) {
        return null;
    }
    return {
        verb: ((_b = (_a = http.getProperty("Verb")) === null || _a === void 0 ? void 0 : _a.getValue()) !== null && _b !== void 0 ? _b : "").toUpperCase(),
        route: (_d = (_c = http.getProperty("Route")) === null || _c === void 0 ? void 0 : _c.getValue()) !== null && _d !== void 0 ? _d : ""
    };
}
// A route has drifted when its upstream verb or route no longer matches its endpoint
function gatewayRouteDrift(route, http) {
    const upstream = gatewayHttpOf(route);
    if (!upstream || (upstream.verb === http.verb && upstream.route === http.route)) {
        return null;
    }
    return `${upstream.verb} /${upstream.route}`;
}
function describeGatewayRouteChanges({ add, remove }) {
    const list = (items) => items.length <= 3 ? items.join(", ") : `${items.slice(0, 3).join(", ")} and ${items.length - 3} more`;
    const parts = [];
    if (add.length) {
        parts.push(`add ${add.length} (${list(add)})`);
    }
    if (remove.length) {
        parts.push(`remove ${remove.length} (${list(remove)})`);
    }
    return `This will ${parts.join(" and ")} gateway route(s).`
        + (remove.length ? " Hand edits to a removed route in ocelot.json are lost on the next Software Factory run." : "");
}
// A tree-view field doesn't raise onChange, so the form can't show pending changes live; confirm them on Apply instead
async function confirmGatewayRouteChanges(changes) {
    if (!changes.add.length && !changes.remove.length) {
        return true;
    }
    try {
        await dialogService.confirm(describeGatewayRouteChanges(changes), "Apply gateway route changes");
        return true;
    }
    catch {
        return false;
    }
}
// The tree's value lists the ticked node ids; folder ids are in it too, and are ignored
function selectedGatewayNodeIds(value) {
    const raw = Array.isArray(value) ? value : [value].filter(x => x);
    return new Set(raw.map(x => typeof x === "string" ? x : x.id));
}
/**
 * Opens the Manage gateway routes form for the gateway package or folder the menu was opened on.
 * New routes are created under that element.
 */
async function manageGatewayRoutes(element) {
    var _a;
    // Existing routes, keyed by the endpoint they call
    const routeByEndpointId = new Map();
    for (const route of lookupTypesOf("Api Gateway Route")) {
        for (const association of route.getAssociations("Route Association")) {
            const endpointId = (_a = association.typeReference) === null || _a === void 0 ? void 0 : _a.getTypeId();
            if (endpointId) {
                routeByEndpointId.set(endpointId, route);
            }
        }
    }
    // Endpoints in referenced packages, grouped by package, then by the folder they sit in
    const endpointsById = new Map();
    const packages = new Map();
    for (const endpoint of lookupTypesOf(["Command", "Query", "Operation"], true)) {
        const http = gatewayHttpOf(endpoint);
        if (!http) {
            continue;
        }
        const entry = { endpoint, http };
        endpointsById.set(endpoint.id, entry);
        const pkg = endpoint.getPackage();
        if (!packages.has(pkg.id)) {
            packages.set(pkg.id, { pkg, folders: new Map() });
        }
        const folders = packages.get(pkg.id).folders;
        const parent = endpoint.getParent();
        if (!folders.has(parent.id)) {
            folders.set(parent.id, { folder: parent, endpoints: [] });
        }
        folders.get(parent.id).endpoints.push(entry);
    }
    if (packages.size === 0) {
        await dialogService.info("This gateway references no packages with HTTP endpoints. Add a package reference to a Services package first.");
        return;
    }
    const endpointSpecializations = new Set();
    const groupSpecializations = new Set();
    let driftCount = 0;
    function endpointNode({ endpoint, http }) {
        endpointSpecializations.add(endpoint.specializationId);
        const route = routeByEndpointId.get(endpoint.id);
        const drifted = route ? gatewayRouteDrift(route, http) : null;
        if (drifted) {
            driftCount++;
        }
        return {
            id: endpoint.id,
            specializationId: endpoint.specializationId,
            label: `${http.verb} /${http.route}`,
            icon: endpoint.getIcon(),
            isSelected: !!route,
            // The tree renders this once, so it shows the state on opening; the confirm on Apply lists what changes
            displayFunction: () => {
                var _a;
                return [
                    { text: `${http.verb} `, color: (_a = gatewayVerbColors[http.verb]) !== null && _a !== void 0 ? _a : "#c0c0c0" },
                    { text: `/${http.route}  ` },
                    { text: `${endpoint.getName()}  `, color: gatewayMutedColor },
                    ...(drifted ? [{ text: `changed, was ${drifted}`, color: gatewayDriftColor }] : [])
                ];
            }
        };
    }
    // Only folders with a route start expanded, so the routed and changed endpoints are what you see first
    const packageNodes = [...packages.values()].map(({ pkg, folders }) => ({
        id: pkg.id,
        specializationId: pkg.specializationId,
        label: pkg.getName(),
        isExpanded: true,
        children: [...folders.values()].map(({ folder, endpoints }) => {
            groupSpecializations.add(folder.specializationId);
            return {
                id: folder.id,
                specializationId: folder.specializationId,
                label: folder.getName(),
                icon: folder.getIcon(),
                isExpanded: endpoints.some(e => routeByEndpointId.has(e.endpoint.id)),
                isSelected: endpoints.every(e => routeByEndpointId.has(e.endpoint.id)),
                children: endpoints.map(endpointNode)
            };
        })
    }));
    const result = await dialogService.openForm({
        title: "Manage gateway routes",
        icon: "fa-route",
        helpText: "Endpoints come from the Services packages this gateway references. Tick to expose an endpoint through the gateway, untick to remove its route.",
        submitButtonText: "Apply",
        minWidth: "760px",
        fields: [
            {
                id: "intro",
                fieldType: "alert",
                label: `Tick the endpoints this gateway exposes, or tick a folder for all of its endpoints. ${routeByEndpointId.size} of ${endpointsById.size} are routed now`
                    + (driftCount > 0 ? `, and ${driftCount} no longer match their endpoint.` : "."),
                hintType: "info"
            },
            {
                id: "endpoints",
                fieldType: "tree-view",
                label: "Endpoints in referenced packages",
                treeViewOptions: {
                    isMultiSelect: true,
                    height: "420px",
                    rootNode: packageNodes.length === 1
                        ? packageNodes[0]
                        : { id: "referenced-packages", specializationId: "referenced-packages", label: "Referenced packages", isExpanded: true, children: packageNodes },
                    selectableTypes: [
                        ...[...endpointSpecializations].map(s => ({ specializationId: s, isSelectable: true })),
                        ...[...groupSpecializations].map(s => ({ specializationId: s, isSelectable: true, autoSelectChildren: true }))
                    ]
                }
            },
            {
                id: "sync",
                fieldType: "checkbox",
                label: `Sync changed routes with their endpoint (${driftCount})`,
                value: driftCount > 0 ? "true" : "false",
                isDisabled: driftCount === 0,
                hint: "Copies the endpoint's current verb and route onto routes whose upstream no longer matches."
            }
        ]
    });
    if (!result) {
        return;
    }
    const selected = selectedGatewayNodeIds(result.endpoints);
    const changes = { add: [], remove: [] };
    for (const [endpointId, { http }] of endpointsById) {
        const routed = routeByEndpointId.has(endpointId);
        if (selected.has(endpointId) && !routed) {
            changes.add.push(`${http.verb} /${http.route}`);
        }
        if (!selected.has(endpointId) && routed) {
            changes.remove.push(`${http.verb} /${http.route}`);
        }
    }
    if (!await confirmGatewayRouteChanges(changes)) {
        return;
    }
    const sync = result.sync === true || result.sync === "true";
    let added = 0, removed = 0, synced = 0;
    for (const [endpointId, { endpoint, http }] of endpointsById) {
        const route = routeByEndpointId.get(endpointId);
        if (selected.has(endpointId) && !route) {
            // The modeler's Route Association scripts name the route and copy the verb, route and body
            const created = createElement("Api Gateway Route", endpoint.getName(), element.id);
            createAssociation("Route Association", created.id, endpointId);
            added++;
        }
        else if (!selected.has(endpointId) && route) {
            route.delete();
            removed++;
        }
        else if (route && sync && gatewayRouteDrift(route, http)) {
            const routeHttp = route.getStereotype("Http Settings");
            routeHttp.getProperty("Verb").setValue(http.verb);
            routeHttp.getProperty("Route").setValue(http.route);
            synced++;
        }
    }
    await dialogService.info(`Added ${added}, removed ${removed} and synced ${synced} gateway route(s).`);
}
