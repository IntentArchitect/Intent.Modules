/// <reference path="../../common/openSelectItemDialog.ts" />
/// <reference path="../../common/proxyServiceHelper.ts" />

async function execute(operation: MacroApi.Context.IElementApi) {
    let servicePackages = getPackages().filter(pkg => pkg.specialization === "Services Package");
    let selectedPackage: MacroApi.Context.IPackageApi;
    if (servicePackages.length == 1) {
        selectedPackage = servicePackages[0];
    } else {
        selectedPackage = await openSelectItemDialog(getPackageSelectItemOptions(servicePackages, "Service Package"));
    }

    const proxy = operation.getParent();
    const folderName = pluralize(ProxyServiceHelper.sanitizeServiceName(proxy.getName()));
    const folder = selectedPackage.getChildren("Folder").find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), selectedPackage.id);

    let newOperation = ProxyServiceHelper.createAppServiceOperationAction(operation, folder);
 
    const diagramElement = folder.getChildren("Diagram").find(x => x.getName() == folderName) ?? createElement("Diagram", folderName, folder.id)
    diagramElement.loadDiagram();
    const diagram = getCurrentDiagram();
    diagram.layoutVisuals([folder, newOperation.getParent()], null, true);
}

/**
 * Used by Intent.Modelers.Services.ProxyInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-cqrs-crud/create-appservice-proxy-operation-macro-advanced-mapping/create-appservice-proxy-operation-macro-advanced-mapping.ts
 */
//await execute(element);