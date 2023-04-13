/**
 * Used by:
 * Intent.Modules\Modules\Modelers.Eventing by the "On Loaded" event for Packages.
 *
 * Source code here:
 * https://github.com/IntentSoftware/Intent.Modules/blob/master/DesignerMacros/src/eventing-designer-on-load/eventing-designer-on-load.ts
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

    package.removeMetadata(metadataKeyName);

    const appElement = lookupTypesOf(eventingApplicationElementType)
        .filter(x => x.getPackage().id === package.id)[0];
    if (appElement == null) {
        package.addMetadata(metadataKeyName, currentVersion.toString());
        package.expand();
        continue;
    }

    appElement.loadDiagram();
    const diagram = getCurrentDiagram();
    if (diagram.getVisual(appElement.id) != null) {
        package.addMetadata(metadataKeyName, currentVersion.toString());
        package.expand();
        continue;
    }

    diagram.addElement(appElement.id, { x: 400, y: 320 }, { width: 200, height: 60 });
    package.addMetadata(metadataKeyName, currentVersion.toString());
    package.expand();
}