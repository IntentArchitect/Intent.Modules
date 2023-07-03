/// <reference path="../../../typings/elementmacro.context.api.d.ts" />


function getServiceRoute(element: MacroApi.Context.IElementApi) : string{
    let folder = element.getParent();
    let folderName = folder.getName();
    let serviceRoute = toKebabCase(folderName);

    while (folder.getParent().specialization === "Folder"){
        folder = folder.getParent();
        folderName = folder.getName();
        serviceRoute = `${toKebabCase(folderName)}/${serviceRoute}`;    
    }

    return serviceRoute;
}
