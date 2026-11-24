/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../typings/core.context.types.d.ts" />
/// <reference path="./icons.ts" />

// Manage gateway routes
// Lists the HTTP endpoints (Commands, Queries and Operations with Http Settings) in the packages this gateway references.
// Tick to expose an endpoint through the gateway, untick to remove its route. Ctrl+Z undoes an Apply.

interface IGatewayEndpoint {
    endpoint: MacroApi.Context.IElementApi;
    http: IHttpRoute;
}

interface IHttpRoute {
    verb: string;
    route: string;
}

const gatewayVerbColors: { [verb: string]: string } = { GET: "#61affe", POST: "#49cc90", PUT: "#fca130", PATCH: "#50e3c2", DELETE: "#f93e3e" };
const gatewayMutedColor = "#8b98a5";
const gatewayDriftColor = "#fca130";
const gatewaySecuredColor = "#22c55e";
const gatewayUnsecuredColor = "#ef4444";
const gatewayRateLimitColor = "#a78bfa";

// Looked up by definition id, so they match without a dependency: Secured and Unsecured come from Intent.Metadata.Security,
// Rate Limit from Intent.ApiGateway.Ocelot. Without those modules installed no route carries them.
const gatewaySecuredStereotypeId = "a9eade71-1d56-4be7-a80c-81046c0c978b";
const gatewayUnsecuredStereotypeId = "8b65c29e-1448-43ac-a92a-0e0f86efd6c6";
const gatewayRateLimitStereotypeId = "fee00939-5360-4a07-8d80-300a7d9ee81e";

// Same rule as Intent.ApiGateway.Ocelot's ocelot.json: routes are secured by default, and the closest Secured or Unsecured
// stereotype, looking at the route, its folders and then its package, decides. Where one element carries both, Secured wins.
function gatewayRouteIsUnsecured(route: MacroApi.Context.IElementApi): boolean {
    const decide = (element: { hasStereotype(id: string): boolean }): boolean | null =>
        element.hasStereotype(gatewaySecuredStereotypeId) ? false
            : element.hasStereotype(gatewayUnsecuredStereotypeId) ? true
                : null;
    const pkg = route.getPackage();
    for (let element = route; element && element.id !== pkg.id; element = element.getParent()) {
        const unsecured = decide(element);
        if (unsecured !== null) {
            return unsecured;
        }
    }
    return decide(pkg) ?? false;
}

// Tags after a routed endpoint, styled like the Synchronize with Domain Entity tree. They show the route's state when the
// form opens; security and rate limiting are changed on the route in the designer, not here.
function gatewayRouteTags(route: MacroApi.Context.IElementApi): MacroApi.Context.IDisplayTextComponent[] {
    const tags: MacroApi.Context.IDisplayTextComponent[] = gatewayRouteIsUnsecured(route)
        ? [{ text: "[UNSECURED]  ", color: gatewayUnsecuredColor }]
        : [{ text: "[SECURED]  ", color: gatewaySecuredColor }];
    if (route.hasStereotype(gatewayRateLimitStereotypeId)) {
        tags.push({ text: "[RATE LIMITED]  ", color: gatewayRateLimitColor });
    }
    return tags;
}

function gatewayHttpOf(element: MacroApi.Context.IElementApi): IHttpRoute {
    const http = element.getStereotype("Http Settings");
    if (!http) {
        return null;
    }
    return {
        verb: ((http.getProperty("Verb")?.getValue() as string) ?? "").toUpperCase(),
        route: (http.getProperty("Route")?.getValue() as string) ?? ""
    };
}

// A route has drifted when its upstream verb or route no longer matches its endpoint
function gatewayRouteDrift(route: MacroApi.Context.IElementApi, http: IHttpRoute): string {
    const upstream = gatewayHttpOf(route);
    if (!upstream || (upstream.verb === http.verb && upstream.route === http.route)) {
        return null;
    }
    return `${upstream.verb} /${upstream.route}`;
}

// The tree's value lists the ticked node ids; folder ids are in it too, and are ignored
function selectedGatewayNodeIds(value: any): Set<string> {
    const raw: any[] = Array.isArray(value) ? value : [value].filter(x => x);
    return new Set(raw.map(x => typeof x === "string" ? x : x.id));
}

/**
 * Opens the Manage gateway routes form for the gateway package or folder the menu was opened on.
 * New routes are created under that element.
 */
async function manageGatewayRoutes(element: MacroApi.Context.IElementApi): Promise<void> {
    // Existing routes, keyed by the endpoint they call
    const routeByEndpointId = new Map<string, MacroApi.Context.IElementApi>();
    for (const route of lookupTypesOf("Api Gateway Route")) {
        for (const association of route.getAssociations("Route Association")) {
            const endpointId = association.typeReference?.getTypeId();
            if (endpointId) {
                routeByEndpointId.set(endpointId, route);
            }
        }
    }

    // Endpoints in referenced packages, grouped by package, then by the folder they sit in
    const endpointsById = new Map<string, IGatewayEndpoint>();
    // Endpoints that sit directly in the package (getParent() is then the package, which has no icon) go in rootEndpoints
    const packages = new Map<string, { pkg: MacroApi.Context.IPackageApi, rootEndpoints: IGatewayEndpoint[], folders: Map<string, { folder: MacroApi.Context.IElementApi, endpoints: IGatewayEndpoint[] }> }>();
    for (const endpoint of lookupTypesOf(["Command", "Query", "Operation"], true)) {
        const http = gatewayHttpOf(endpoint);
        if (!http) {
            continue;
        }
        const entry = { endpoint, http };
        endpointsById.set(endpoint.id, entry);
        const pkg = endpoint.getPackage();
        if (!packages.has(pkg.id)) {
            packages.set(pkg.id, { pkg, rootEndpoints: [], folders: new Map() });
        }
        const folders = packages.get(pkg.id).folders;
        const parent = endpoint.getParent();
        if (!parent || parent.id === pkg.id) {
            packages.get(pkg.id).rootEndpoints.push(entry);
            continue;
        }
        if (!folders.has(parent.id)) {
            folders.set(parent.id, { folder: parent, endpoints: [] });
        }
        folders.get(parent.id).endpoints.push(entry);
    }

    if (packages.size === 0) {
        await dialogService.info("This gateway references no packages with HTTP endpoints. Add a package reference to a Services package first.");
        return;
    }

    const endpointSpecializations = new Set<string>();
    const groupSpecializations = new Set<string>();
    let driftCount = 0;

    function endpointNode({ endpoint, http }: IGatewayEndpoint): MacroApi.Context.ISelectableTreeNode {
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
            // The tree renders this once, so the tags show each route's state when the form opens
            displayFunction: () => [
                { text: `${http.verb} `, color: gatewayVerbColors[http.verb] ?? "#c0c0c0" },
                { text: `/${http.route}  ` },
                { text: `${endpoint.getName()}  `, color: gatewayMutedColor },
                ...(route ? gatewayRouteTags(route) : []),
                ...(drifted ? [{ text: `changed, was ${drifted}`, color: gatewayDriftColor }] : [])
            ]
        };
    }

    // Only folders with a route start expanded, so the routed and changed endpoints are what you see first
    const packageNodes: MacroApi.Context.ISelectableTreeNode[] = [...packages.values()].map(({ pkg, rootEndpoints, folders }) => ({
        id: pkg.id,
        specializationId: pkg.specializationId,
        label: pkg.getName(),
        icon: GatewayRouteIcons.packageIcon,
        isExpanded: true,
        children: [...[...folders.values()].map(({ folder, endpoints }) => {
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
        }), ...rootEndpoints.map(endpointNode)]
    }));

    const result = await dialogService.openForm({
        title: "Manage gateway routes",
        icon: "fa-route",
        helpText: "Endpoints come from the Services packages this gateway references. Tick to expose an endpoint through the gateway, untick to remove its route. A routed endpoint shows whether its route is secured or unsecured, and rate limited; change those on the route in the designer. Your changes are made as soon as you submit the form, and Ctrl+Z undoes them. Removing a route also drops any hand edits to it in ocelot.json on the next Software Factory run.",
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
                        : { id: "referenced-packages", specializationId: "referenced-packages", label: "Referenced packages", icon: GatewayRouteIcons.packagesIcon, isExpanded: true, children: packageNodes },
                    selectableTypes: [
                        ...[...endpointSpecializations].map(s => ({ specializationId: s, isSelectable: true })),
                        ...[...groupSpecializations].map(s => ({ specializationId: s, isSelectable: true, autoSelectChildren: true }))
                    ]
                }
            },
            // Only offered when a route's verb or route no longer matches its endpoint; otherwise there is nothing to sync
            ...(driftCount > 0 ? [{
                id: "sync",
                fieldType: "checkbox" as const,
                label: `Update the ${driftCount} route(s) whose verb or route no longer matches its endpoint`,
                value: "true",
                hint: "Copies each endpoint's current verb and route onto its gateway route."
            }] : [])
        ]
    });

    if (!result) {
        return;
    }

    // Submitting the form is the confirmation, and the designer shows the result: no dialog before or after. Ctrl+Z undoes it.
    const selected = selectedGatewayNodeIds(result.endpoints);
    const sync = result.sync === true || result.sync === "true";
    for (const [endpointId, { endpoint, http }] of endpointsById) {
        const route = routeByEndpointId.get(endpointId);
        if (selected.has(endpointId) && !route) {
            // The modeler's Route Association scripts name the route and copy the verb, route and body
            const created = createElement("Api Gateway Route", endpoint.getName(), element.id);
            createAssociation("Route Association", created.id, endpointId);
        } else if (!selected.has(endpointId) && route) {
            route.delete();
        } else if (route && sync && gatewayRouteDrift(route, http)) {
            const routeHttp = route.getStereotype("Http Settings");
            routeHttp.getProperty("Verb").setValue(http.verb);
            routeHttp.getProperty("Route").setValue(http.route);
        }
    }
}
