/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function getFolderParts(request: IElementApi, entity: IElementApi, owningEntity: IElementApi): string[] {
    let depth = 0;
    let currentElement = request.getParent();
    let folderParts: string[] = [];

    while (currentElement.specialization === "Folder") {
        let folderName = currentElement.getName();

        if (depth === 0 && entity != null) {
            const singularizedFolderName = singularize(folderName);
            const singularizedAggregateRootName = singularize((owningEntity ?? entity).getName());

            if (singularizedFolderName.toLowerCase() === singularizedAggregateRootName.toLowerCase()) {
                folderName = singularizedFolderName;
            }
        }

        folderParts.unshift(toKebabCase(folderName));
        currentElement = currentElement.getParent();
        depth++;
    }

    return folderParts;
}