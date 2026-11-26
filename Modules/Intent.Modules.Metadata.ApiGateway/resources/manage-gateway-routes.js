// A package has no getIcon(), so the package rows and the Referenced packages root draw their own:
// a package box, and a stack of them. The sources are icons/package.svg and icons/packages.svg.
// The client reads an svg data URI with atob(), so these must be base64; there is no btoa() at run time to encode them.
class GatewayRouteIcons {
}
GatewayRouteIcons.packageIcon = "data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA1MCA1MCI+PGRlZnM+PGxpbmVhckdyYWRpZW50IGlkPSJndy1wa2ctdG9wIiB4MT0iMjUiIHkxPSI1IiB4Mj0iMjUiIHkyPSIyNyIgZ3JhZGllbnRVbml0cz0idXNlclNwYWNlT25Vc2UiPjxzdG9wIG9mZnNldD0iMCIgc3RvcC1jb2xvcj0iI2E4ZWNmMiIvPjxzdG9wIG9mZnNldD0iMSIgc3RvcC1jb2xvcj0iIzZmZDBlMCIvPjwvbGluZWFyR3JhZGllbnQ+PGxpbmVhckdyYWRpZW50IGlkPSJndy1wa2ctbGVmdCIgeDE9IjUiIHkxPSIxNiIgeDI9IjI1IiB5Mj0iNDYiIGdyYWRpZW50VW5pdHM9InVzZXJTcGFjZU9uVXNlIj48c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiM1Y2M3ZDYiLz48c3RvcCBvZmZzZXQ9IjEiIHN0b3AtY29sb3I9IiMzYjhmYzQiLz48L2xpbmVhckdyYWRpZW50PjxsaW5lYXJHcmFkaWVudCBpZD0iZ3ctcGtnLXJpZ2h0IiB4MT0iNDUiIHkxPSIxNiIgeDI9IjI1IiB5Mj0iNDYiIGdyYWRpZW50VW5pdHM9InVzZXJTcGFjZU9uVXNlIj48c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiM0YThjY2IiLz48c3RvcCBvZmZzZXQ9IjEiIHN0b3AtY29sb3I9IiMyZjVkOWUiLz48L2xpbmVhckdyYWRpZW50PjwvZGVmcz48cGF0aCBkPSJNMjUgNSA0NSAxNS41IDI1IDI2IDUgMTUuNVoiIGZpbGw9InVybCgjZ3ctcGtnLXRvcCkiLz48cGF0aCBkPSJNNSAxNS41IDI1IDI2djIwTDUgMzUuNVoiIGZpbGw9InVybCgjZ3ctcGtnLWxlZnQpIi8+PHBhdGggZD0iTTI1IDI2IDQ1IDE1LjV2MjBMMjUgNDZaIiBmaWxsPSJ1cmwoI2d3LXBrZy1yaWdodCkiLz48cGF0aCBkPSJNMTUgMTAuMjUgMzUgMjAuNzV2Ni41bC00IDIuMXYtNi41TDExIDEyLjM1WiIgZmlsbD0iI2ZmZmZmZiIgZmlsbC1vcGFjaXR5PSIuNTUiLz48L3N2Zz4=";
GatewayRouteIcons.packagesIcon = "data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA1MCA1MCI+PGRlZnM+PGxpbmVhckdyYWRpZW50IGlkPSJndy1wa2dzLXRvcCIgeDE9IjAiIHkxPSIwIiB4Mj0iMCIgeTI9IjEiPjxzdG9wIG9mZnNldD0iMCIgc3RvcC1jb2xvcj0iI2E4ZWNmMiIvPjxzdG9wIG9mZnNldD0iMSIgc3RvcC1jb2xvcj0iIzZmZDBlMCIvPjwvbGluZWFyR3JhZGllbnQ+PGxpbmVhckdyYWRpZW50IGlkPSJndy1wa2dzLWxlZnQiIHgxPSIwIiB5MT0iMCIgeDI9IjEiIHkyPSIxIj48c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiM1Y2M3ZDYiLz48c3RvcCBvZmZzZXQ9IjEiIHN0b3AtY29sb3I9IiMzYjhmYzQiLz48L2xpbmVhckdyYWRpZW50PjxsaW5lYXJHcmFkaWVudCBpZD0iZ3ctcGtncy1yaWdodCIgeDE9IjEiIHkxPSIwIiB4Mj0iMCIgeTI9IjEiPjxzdG9wIG9mZnNldD0iMCIgc3RvcC1jb2xvcj0iIzRhOGNjYiIvPjxzdG9wIG9mZnNldD0iMSIgc3RvcC1jb2xvcj0iIzJmNWQ5ZSIvPjwvbGluZWFyR3JhZGllbnQ+PGcgaWQ9Imd3LXBrZ3MtY3ViZSIgc3Ryb2tlPSIjZThmYmZkIiBzdHJva2Utb3BhY2l0eT0iLjciIHN0cm9rZS13aWR0aD0iLjkiIHN0cm9rZS1saW5lam9pbj0icm91bmQiPjxwYXRoIGQ9Ik0xMiAwIDI0IDYuMyAxMiAxMi42IDAgNi4zWiIgZmlsbD0idXJsKCNndy1wa2dzLXRvcCkiLz48cGF0aCBkPSJNMCA2LjMgMTIgMTIuNnYxMkwwIDE4LjNaIiBmaWxsPSJ1cmwoI2d3LXBrZ3MtbGVmdCkiLz48cGF0aCBkPSJNMTIgMTIuNiAyNCA2LjN2MTJMMTIgMjQuNloiIGZpbGw9InVybCgjZ3ctcGtncy1yaWdodCkiLz48L2c+PC9kZWZzPjx1c2UgaHJlZj0iI2d3LXBrZ3MtY3ViZSIgeD0iMSIgeT0iMjQiLz48dXNlIGhyZWY9IiNndy1wa2dzLWN1YmUiIHg9IjI1IiB5PSIyNCIvPjx1c2UgaHJlZj0iI2d3LXBrZ3MtY3ViZSIgeD0iMTMiIHk9IjExLjQiLz48L3N2Zz4=";
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../typings/core.context.types.d.ts" />
/// <reference path="./icons.ts" />
const gatewayVerbColors = { GET: "#61affe", POST: "#49cc90", PUT: "#fca130", PATCH: "#50e3c2", DELETE: "#f93e3e" };
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
function gatewayRouteIsUnsecured(route) {
    var _a;
    const decide = (element) => element.hasStereotype(gatewaySecuredStereotypeId) ? false
        : element.hasStereotype(gatewayUnsecuredStereotypeId) ? true
            : null;
    const pkg = route.getPackage();
    for (let element = route; element && element.id !== pkg.id; element = element.getParent()) {
        const unsecured = decide(element);
        if (unsecured !== null) {
            return unsecured;
        }
    }
    return (_a = decide(pkg)) !== null && _a !== void 0 ? _a : false;
}
// Tags after a routed endpoint, styled like the Synchronize with Domain Entity tree. They show the route's state when the
// form opens; security and rate limiting are changed on the route in the designer, not here.
function gatewayRouteTags(route) {
    const tags = gatewayRouteIsUnsecured(route)
        ? [{ text: "[UNSECURED]  ", color: gatewayUnsecuredColor }]
        : [{ text: "[SECURED]  ", color: gatewaySecuredColor }];
    if (route.hasStereotype(gatewayRateLimitStereotypeId)) {
        tags.push({ text: "[RATE LIMITED]  ", color: gatewayRateLimitColor });
    }
    return tags;
}
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
    // Endpoints that sit directly in the package (getParent() is then the package, which has no icon) go in rootEndpoints
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
            // The tree renders this once, so the tags show each route's state when the form opens
            displayFunction: () => {
                var _a;
                return [
                    { text: `${http.verb} `, color: (_a = gatewayVerbColors[http.verb]) !== null && _a !== void 0 ? _a : "#c0c0c0" },
                    { text: `/${http.route}  ` },
                    { text: `${endpoint.getName()}  `, color: gatewayMutedColor },
                    ...(route ? gatewayRouteTags(route) : []),
                    ...(drifted ? [{ text: `changed, was ${drifted}`, color: gatewayDriftColor }] : [])
                ];
            }
        };
    }
    // Only folders with a route start expanded, so the routed and changed endpoints are what you see first
    const packageNodes = [...packages.values()].map(({ pkg, rootEndpoints, folders }) => ({
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
                    fieldType: "checkbox",
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
        }
        else if (!selected.has(endpointId) && route) {
            route.delete();
        }
        else if (route && sync && gatewayRouteDrift(route, http)) {
            const routeHttp = route.getStereotype("Http Settings");
            routeHttp.getProperty("Verb").setValue(http.verb);
            routeHttp.getProperty("Route").setValue(http.route);
        }
    }
}
