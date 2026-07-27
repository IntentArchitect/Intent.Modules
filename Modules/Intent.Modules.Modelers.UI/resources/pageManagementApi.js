/// <reference path="../../typings/elementmacro.context.api.d.ts" />

const PAGE_STEREOTYPE_NAME = "Page";
const PAGE_ROUTE_PROPERTY = "Route";
const PAGE_TITLE_PROPERTY = "Title";

const NAVIGATION_ASSOCIATION = "Navigation";

let PageManagementApi = {
    createPage,
    updatePage,
    deletePage,
    findPage,
    addNavigation,
};

/**
 * Creates a Page (a Component with the "Page" stereotype) under `parent`, or updates
 * it in place if a page with the same name already exists there.
 * @param {string} pageName - the Component's name.
 * @param {string} pageTitle - value for the Page stereotype's "Title" property.
 * @param {string} route - value for the Page stereotype's "Route" property.
 * @param {Object.<string,string>} [metadata] - free-form key/value pairs stored via
 *        element metadata (`setMetadata`), e.g. to tag which module/feature owns the page.
 * @param {object} [parent] - element or package to create the page under. Defaults to
 *        the current diagram when omitted - callers running outside a diagram context
 *        (e.g. a future settings-changed script) must pass an explicit parent.
 * @returns {object} the page element.
 */
function createPage(pageName, pageTitle, route, metadata, parent) {
    const existing = findPage(pageName, parent);
    if (existing) {
        return updatePage(pageName, pageTitle, route, metadata, parent);
    }
    const container = resolveParent(parent);
    const page = container.addChild("Component", pageName);
    const stereotype = page.ensureStereotype(PAGE_STEREOTYPE_NAME);
    applyPageProperties(stereotype, pageTitle, route);
    applyMetadata(page, metadata);
    return page;
}

/**
 * Updates an existing page's title, route and/or metadata. Throws if no page named
 * `pageName` (with the Page stereotype) is found under `parent`.
 * @param {string} pageName
 * @param {string} [pageTitle] - when omitted, the existing "Title" is left unchanged.
 * @param {string} [route] - when omitted, the existing "Route" is left unchanged.
 * @param {Object.<string,string>} [metadata] - keys provided are set/overwritten;
 *        keys not present in `metadata` are left untouched (additive merge).
 * @param {object} [parent]
 * @returns {object} the updated page element.
 */
function updatePage(pageName, pageTitle, route, metadata, parent) {
    const page = findPage(pageName, parent);
    if (!page) {
        throw new Error(`Page "${pageName}" was not found; cannot update.`);
    }
    const stereotype = page.ensureStereotype(PAGE_STEREOTYPE_NAME);
    applyPageProperties(stereotype, pageTitle, route);
    applyMetadata(page, metadata);
    return page;
}

/**
 * Deletes the page named `pageName` if it exists.
 * @param {string} pageName
 * @param {object} [parent]
 * @returns {boolean} true if a page was found and deleted, false if none was found.
 */
function deletePage(pageName, parent) {
    const page = findPage(pageName, parent);
    if (!page) {
        return false;
    }
    page.delete();
    return true;
}

/**
 * Finds an existing Page Component by name.
 *
 * Lookup is by CURRENT name only (scoped to `parent` when provided, otherwise
 * searched across the whole designer) - a page renamed by hand in the designer will
 * no longer be found by its original name.
 * @param {string} pageName
 * @param {object} [parent]
 * @returns {object|null}
 */
function findPage(pageName, parent) {
    const candidates = parent
        ? resolveParent(parent).getChildren("Component")
        : findElements({ name: pageName, specialization: "Component" });
    return candidates.find(c => c.getName() === pageName && c.hasStereotype(PAGE_STEREOTYPE_NAME)) ?? null;
}

function applyPageProperties(stereotype, pageTitle, route) {
    if (pageTitle != null) {
        stereotype.setProperty(PAGE_TITLE_PROPERTY, pageTitle);
    }
    if (route != null) {
        stereotype.setProperty(PAGE_ROUTE_PROPERTY, route);
    }
}

function applyMetadata(page, metadata) {
    if (!metadata) {
        return;
    }
    Object.keys(metadata).forEach(key => page.setMetadata(key, metadata[key]));
}

function resolveParent(parent) {
    return parent ?? getCurrentDiagram();
}

function resolvePageRef(pageOrName) {
    if (typeof pageOrName !== "string") {
        return pageOrName;
    }
    const found = findPage(pageOrName);
    if (!found) {
        throw new Error(`Page "${pageOrName}" was not found; cannot add navigation.`);
    }
    return found;
}

/**
 * Adds a Navigation association from `fromPage` to `toPage` (each a page name or
 * an already-resolved page element).
 * @param {string|object} fromPage
 * @param {string|object} toPage
 * @param {boolean} [mergeBidirectional=true] - when true (default) and a Navigation
 *        already connects these two pages in the OPPOSITE direction, that existing
 *        edge is made bidirectional instead of creating a second one. Re-adding the
 *        SAME direction is a no-op (returns the existing edge unchanged). When
 *        false, a brand-new, separate Navigation association is always created,
 *        regardless of any existing edge.
 * @returns {object} the Navigation association (existing, merged, or newly created).
 */
function addNavigation(fromPage, toPage, mergeBidirectional) {
    if (mergeBidirectional === undefined) {
        mergeBidirectional = true;
    }
    const source = resolvePageRef(fromPage);
    const target = resolvePageRef(toPage);

    if (mergeBidirectional) {
        const handle = source.getAssociations(NAVIGATION_ASSOCIATION).find(assoc => {
            const other = assoc.getAssociatedElement();
            return other && other.id === target.id;
        });
        if (handle) {
            if (!handle.isSourceEnd()) {
                // existing edge runs target -> source (the reverse of what's being
                // added) - merge into a single bidirectional edge.
                handle.setBidirectional(true);
            }
            // same direction already existed - idempotent no-op either way.
            return handle;
        }
    }

    return createAssociation(NAVIGATION_ASSOCIATION, source.id, target.id);
}
