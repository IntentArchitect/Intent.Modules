/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function changeReturnType(element: MacroApi.Context.IElementApi): void{   
    
    const pagedResultType = "9204e067-bdc8-45e7-8970-8a833fdc5253";
    
    let currentReturnType = element.typeReference.typeId;
    element.typeReference.setType(pagedResultType, [{ typeId : currentReturnType, isCollection : false, isNullable : false  }] );
    element.typeReference.setIsCollection(false);
    element.typeReference.setIsNullable(false);
}

function addPagingParameters(element: MacroApi.Context.IElementApi, childElementType : string): void{   

    const commonTypes = {
        guid: "6b649125-18ea-48fd-a6ba-0bfff0d8f488",
        long: "33013006-E404-48C2-AC46-24EF5A5774FD",
        int: "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74"
    };

    var pageOffsetAttributes: Array<string> = ["pageno", "pageindex"];

    if (!element.getChildren(childElementType).find(x => pageOffsetAttributes.includes( x.getName().toLowerCase()))){
        let pageSize = createElement(childElementType, "PageNo", element.id);
        pageSize.typeReference.setType(commonTypes.int)    
    }    

    if (!element.getChildren(childElementType).find(x => x.getName().toLowerCase() == "pagesize")){
        let pageSize = createElement(childElementType, "PageSize", element.id);
        pageSize.typeReference.setType(commonTypes.int)    
    }
}
