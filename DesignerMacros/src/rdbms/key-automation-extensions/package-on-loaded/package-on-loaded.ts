/// <reference path="../../../../typings/elementmacro.context.api.d.ts" />

let rdbmsDatabaseId = "51a7bcf5-0eb9-4c9a-855e-3ead1048729c";
element.getStereotypes()
if (!element.hasMetadata("database-paradigm-selected") && 
    !element.hasStereotype(rdbmsDatabaseId) && 
    !element.hasStereotype("Document Database")) {
    
    element.addStereotype(rdbmsDatabaseId);
}
element.setMetadata("database-paradigm-selected", "true");
