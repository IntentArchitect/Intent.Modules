/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/mappedDomainElement.ts" />

function getFolderParts(request: IElementApi, domainElement?: MappedDomainElement): string[] {
    let depth = 0;
    let currentElement = request.getParent();
    let folderParts: string[] = [];

    while (currentElement.specialization === "Folder") {
        let folderName = currentElement.getName();

        if (depth === 0 && domainElement != null) {
            const singularizedFolderName = singularize(folderName);
            const singularizedAggregateRootName = singularize(domainElement.entityDomainElementDetails?.getOwningOrTargetEntityName());

            if (singularizedFolderName.toLowerCase() === singularizedAggregateRootName.toLowerCase()) {
                folderName = pluralize(singularizedFolderName);
            }
        }

        folderParts.unshift(toKebabCase(folderName));
        currentElement = currentElement.getParent();
        depth++;
    }

    return folderParts;
}