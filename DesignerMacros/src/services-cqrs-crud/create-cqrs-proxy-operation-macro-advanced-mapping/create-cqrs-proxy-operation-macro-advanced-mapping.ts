/// <reference path="../../common/openSelectItemDialog.ts" />
/// <reference path="../../common/proxyServiceHelper.ts" />

async function execute(proxyOperation: MacroApi.Context.IElementApi) {
    let servicePackages = getPackages().filter(pkg => pkg.specialization === "Services Package");
    let selectedPackage: MacroApi.Context.IPackageApi;
    if (servicePackages.length == 1) {
        selectedPackage = servicePackages[0];
    } else {
        selectedPackage = await openSelectItemDialog(getPackageSelectItemOptions(servicePackages, "Service Package"));
    }

    const proxy = proxyOperation.getParent();
    const folderName = pluralize(proxy.getName());
    const folder = selectedPackage.getChildren("Folder").find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), selectedPackage.id);

    ProxyServiceHelper.createCqrsAction(proxyOperation, folder);

    const diagramElement = folder.getChildren("Diagram").find(x => x.getName() == folderName) ?? createElement("Diagram", folderName, folder.id)
    diagramElement.loadDiagram();
    const diagram = getCurrentDiagram();
    
    //Since we're adding a single new element on the diagram, it may not be positioned below the last created one.
    let lastActionVisual: MacroApi.Context.IElementVisualApi = null;
    for (let action of folder.getChildren("Command").concat(folder.getChildren("Query"))) {
        if (diagram.isVisual(action.id)) {
            lastActionVisual = diagram.getVisual(action.id);
        }
    }

    let newPosition: MacroApi.Context.IPoint = null;
    // This is an attempt to reposition the newly created elements due to the lack of
    // directly manipulating the visuals on the diagram but it ends up skewing diagonally.
    if (lastActionVisual) {
        newPosition = {
            x: lastActionVisual.getPosition().x,
            y: lastActionVisual.getPosition().y - lastActionVisual.getSize().height
        };
    }
    
    diagram.layoutVisuals(folder, newPosition, true);
}

/**
 * Used by Intent.Modelers.Services.DomainInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-cqrs-crud/create-proxy-operation-cqrs-macro-advanced-mapping/create-proxy-operation-cqrs-macro-advanced-mapping.ts
 */
//await execute(element);