/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />

function getFolderParts(request: IElementApi, mappedDetails: IMappedRequestDetails): string[] {
    let depth = 0;
    let currentElement = request.getParent();
    let folderParts: string[] = [];

    while (currentElement.specialization === "Folder") {
        let folderName = currentElement.getName();

        if (depth === 0 && mappedDetails != null) {
            const singularizedFolderName = singularize(folderName);
            const singularizedAggregateRootName = singularize((mappedDetails.owningEntity ?? mappedDetails.entity).getName());

            if (singularizedFolderName.toLowerCase() === singularizedAggregateRootName.toLowerCase()){            
                folderName = singularizedFolderName;
            }
        }

        folderParts.unshift(toKebabCase(folderName));
        currentElement = currentElement.getParent();
        depth++;
    }

    return folderParts;
}