/**
 * Used by:
 * Intent.Modules\Modules\Modelers.Domain by the "On Loaded" event for Packages.
 *
 * Source code here:
 * https://github.com/IntentSoftware/Intent.Modules/blob/master/DesignerMacros/src/domain-designer-on-load/domain-designer-on-load.ts
 */
const metadataKeyName = "initialized";
const diagramElementType = "Diagram";

(function() {
    if (getPackages().some(x => x.hasMetadata(metadataKeyName))) {
        return;
    }

    const diagram = lookupTypesOf(diagramElementType)[0];
    if (diagram != null) {
        diagram.loadDiagram();
    }

    for (const package of getPackages()) {
        package.expand();
        package.setMetadata(metadataKeyName, String(true))
    }
})();
