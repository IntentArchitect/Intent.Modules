/// <reference path="../../common/openSelectItemDialog.ts" />
/// <reference path="../../common/repositoryServiceHelper.ts" />
/// <reference path="common.ts" />

async function createCQRSService(repositoryOperation: IElementApi, diagram: IDiagramApi | undefined) {
    let servicePackages = getPackages().filter(pkg => pkg.specialization === "Services Package");
    let selectedPackage: MacroApi.Context.IPackageApi;
    if (servicePackages.length == 1) {
        selectedPackage = servicePackages[0];
    } else {
        selectedPackage = await openSelectItemDialog(getPackageSelectItemOptions(servicePackages, "Service Package"));
    }

    const repository = repositoryOperation.getParent();
    const folderName = pluralize(removeSuffix(repository.getName(), "Repository", "DAL"));
    const folder = selectedPackage.getChildren("Folder").find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), selectedPackage.id);
    const requestElement = RepositoryServiceHelper.createCqrsAction(repositoryOperation, folder, true);

    if (diagram == null) {
        diagram = await getOrCreateDiagram(folder, repositoryOperation, "CQRS Creation Options");
    }

    if (diagram == null) {
        selectElement(requestElement.id);
        return;
    }

    //Since we're adding a single new element on the diagram, it may not be positioned below the last created one.
    let lastActionVisual: MacroApi.Context.IElementVisualApi = null;
    for (let action of folder.getChildren("Command").concat(folder.getChildren("Query"))) {
        if (diagram.isVisual(action.id)) {
            var actionElement = diagram.getVisual(action.id);

            if (!lastActionVisual || actionElement.getPosition().y > lastActionVisual.getPosition().y) {
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
    } else {
        if (diagram.isVisual(repository.id)) {
            newPosition = {
                x: repoElement.getPosition().x - (repoElement.getSize().width / 1.5),
                y: repoElement.getPosition().y
            };
        }
    }

    diagram.layoutVisuals(folder, newPosition, true);
    diagram.getVisual(requestElement.id)?.select();
}
