/**
 * Used by:
 * Intent.Modules\Modules\Modelers.Eventing by the "On Loaded" event for Packages.
 *
 * Source code here:
 * https://github.com/IntentSoftware/Intent.Modules/blob/master/DesignerMacros/eventing-designer-on-load/eventing-designer-on-load.ts
 */
const eventingApplicationElementType = "Application";
const metadataKeyName = "initialized-version";
const currentVersion = 1;

for (const package of getPackages()) {
    if (package.hasMetadata(metadataKeyName) &&
        Number(package.getMetadata(metadataKeyName)) == currentVersion
    ) {
        continue;
    }

    var appElement = lookupTypesOf(eventingApplicationElementType)
        .filter(x => x.getPackage().id === package.id)[0];
    if (appElement == null) {
        package.setMetadata(metadataKeyName, currentVersion.toString());
        continue;
    }

    appElement.loadDiagram();
    if (currentDiagram.getVisual(appElement.id) != null) {
        package.setMetadata(metadataKeyName, currentVersion.toString());
        continue;
    }

    currentDiagram.addElement(appElement.id, { x: 400, y: 320 }, { width: 200, height: 60 });
    package.setMetadata(metadataKeyName, currentVersion.toString());
}
