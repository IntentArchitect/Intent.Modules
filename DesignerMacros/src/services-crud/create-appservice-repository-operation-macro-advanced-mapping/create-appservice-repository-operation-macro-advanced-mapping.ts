/// <reference path="../../common/openSelectItemDialog.ts" />
/// <reference path="../../common/repositoryServiceHelper.ts" />

async function execute(repositoryOperation: MacroApi.Context.IElementApi) {
    let servicePackages = getPackages().filter(pkg => pkg.specialization === "Services Package");
    let selectedPackage: MacroApi.Context.IPackageApi;
    if (servicePackages.length == 1) {
        selectedPackage = servicePackages[0];
    } else {
        selectedPackage = await openSelectItemDialog(getPackageSelectItemOptions(servicePackages, "Service Package"));
    }

    const repository = repositoryOperation.getParent();
    const folderName = pluralize(repository.getName().replace("Repository", ""));
    const folder = selectedPackage.getChildren("Folder").find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), selectedPackage.id);

    let newOperation = RepositoryServiceHelper.createAppServiceOperationAction(repositoryOperation, folder, null, true);
 
    // this can only be triggered from the diagram, so there will always be one.
    const diagram = getCurrentDiagram();

    let newPosition: MacroApi.Context.IPoint = null;
    let repoElement = diagram.getVisual(repository.id);
    // This is an attempt to reposition the newly created elements due to the lack of
    // directly manipulating the visuals on the diagram but it ends up skewing diagonally.
    if(repoElement){
        newPosition = {
            x: repoElement.getPosition().x - (repoElement.getSize().width / 1.5),
            y: repoElement.getPosition().y + (repoElement.getSize().height / 2)
        };
    }

    diagram.layoutVisuals(newOperation.getParent(), newPosition , true);
}

/**
 * Used by Intent.Modelers.Services.DomainInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-crud/create-appservice-repository-operation-macro-advanced-mapping/create-appservice-repository-operation-macro-advanced-mapping.ts
 */
//await execute(element);