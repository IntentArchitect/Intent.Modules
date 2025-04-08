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

    RepositoryServiceHelper.createCqrsAction(repositoryOperation, folder, true);

    const diagramElement = folder.getChildren("Diagram").find(x => x.getName() == folderName) ?? createElement("Diagram", folderName, folder.id)
    diagramElement.loadDiagram();
    const diagram = getCurrentDiagram();
    
    //Since we're adding a single new element on the diagram, it may not be positioned below the last created one.
    let lastActionVisual: MacroApi.Context.IElementVisualApi = null;
    for (let action of folder.getChildren("Command").concat(folder.getChildren("Query"))) {
        if (diagram.isVisual(action.id)) {
            var actionElement = diagram.getVisual(action.id);

            if(!lastActionVisual || actionElement.getPosition().y > lastActionVisual.getPosition().y) {
                lastActionVisual = actionElement;
            }
        }
    }

    let newPosition: MacroApi.Context.IPoint = null;
    let repoElement = diagram.getVisual(repository.id);
    // This is an attempt to reposition the newly created elements due to the lack of
    // directly manipulating the visuals on the diagram but it ends up skewing diagonally.
    if (lastActionVisual) {
        newPosition = {
            x: repoElement.getPosition().x - (repoElement.getSize().width / 1.5),
            y: lastActionVisual.getPosition().y + (lastActionVisual.getSize().height * 1.5)
        };
    }else {
        if(diagram.isVisual(repository.id)){
            newPosition = {
                x: repoElement.getPosition().x - (repoElement.getSize().width / 1.5),
                y: repoElement.getPosition().y
            };
        }
    }
   
    diagram.layoutVisuals(folder, newPosition, true);
}

/**
 * Used by Intent.Modelers.Services.DomainInteractions
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/development/DesignerMacros/src/services-cqrs-crud/create-cqrs-repository-operation-macro-advanced-mapping/create-cqrs-repository-operation-macro-advanced-mapping.ts
 */
//await execute(element);