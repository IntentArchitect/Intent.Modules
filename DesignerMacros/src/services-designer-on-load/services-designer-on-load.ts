/**
 * Used by:
 * Intent.Modules\Modules\Modelers.Services by the "On Loaded" event for Packages.
 *
 * Source code here:
 * https://github.com/IntentSoftware/Intent.Modules/blob/master/DesignerMacros/src/services-designer-on-load/services-designer-on-load.ts
 */
const metadataKeyName = "initialized";

(function() {
    if (getPackages().some(x => x.hasMetadata(metadataKeyName))) {
        return;
    }

    for (const package of getPackages()) {
        package.expand();
        package.setMetadata(metadataKeyName, String(true))
    }
})();
