/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../_common/constants.ts" />

if (!element.hasMetadata("database-paradigm-selected") &&
    !element.hasStereotype(relationalDatabaseId) &&
    !element.hasStereotype("Document Database")
) {
    element.addStereotype(relationalDatabaseId);
}

element.setMetadata("database-paradigm-selected", "true");
