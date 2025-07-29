/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/paginateHelper.ts" />
/// <reference path="../../common/crudHelper.ts" />

class CursorPaginateApi {

    static changeReturnType(element: MacroApi.Context.IElementApi): void{   
        
        const pagedResultType = "2a11c92d-d27f-4faa-b6fb-c33b93a6ff12";
        
        let currentReturnType = element.typeReference.typeId;
        element.typeReference.setType(pagedResultType, [{ typeId : currentReturnType, isCollection : false, isNullable : false  }] );
        element.typeReference.setIsCollection(false);
        element.typeReference.setIsNullable(false);
    }

    static addPagingParameters(element: MacroApi.Context.IElementApi, childElementType : string): void{   

        const commonTypes = {
            guid: "6b649125-18ea-48fd-a6ba-0bfff0d8f488",
            long: "33013006-E404-48C2-AC46-24EF5A5774FD",
            int: "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74",
            string: "d384db9c-a279-45e1-801e-e4e8099625f2"
        };

        var filterSettings = PaginateHelper.getCursorFiltering(element);

        if (filterSettings.enabled && !element.getChildren(childElementType).find(x => x.getName().toLowerCase() == "partitionKey")) {
            let partKey = createElement(childElementType, "PartitionKey", element.id);
            partKey.typeReference.setType(commonTypes.string);
            partKey.typeReference.setIsNullable(filterSettings.nullable);

            let queryAssociation = element.getAssociations().find(x => x.isTargetEnd() && x?.typeReference.getType()?.specialization == "Class") as IAssociationApi;
            if(queryAssociation != null) {
                const entity = queryAssociation.typeReference.getType();
                const entityPartKey = entity.getChildren("Attribute").find(c => c.getName() == "PartitionKey");
                
                if(entityPartKey != null) {
                    const mapping = queryAssociation.createAdvancedMapping(element.id, entity.id);
                    mapping.addMappedEnd("01d09a7f-0e7c-4670-b7bc-395d7e893ef2",[partKey.id],[entityPartKey.id]);
                }
            }
        }
        if (!element.getChildren(childElementType).find(x => x.getName().toLowerCase() == "pageSize")) {
            let pageSize = createElement(childElementType, "PageSize", element.id);
            pageSize.typeReference.setType(commonTypes.int);
        }
        if (!element.getChildren(childElementType).find(x => x.getName().toLowerCase() == "cursorToken")) {
            let cursorToken = createElement(childElementType, "CursorToken", element.id);
            cursorToken.typeReference.setType(commonTypes.string);
            cursorToken.typeReference.setIsNullable(true);
        }
    }

    static addServicePagination(element: MacroApi.Context.IElementApi): void{   

        CursorPaginateApi.addPagingParameters(element, "Parameter");    
        CursorPaginateApi.changeReturnType(element);
    }

    static addCqrsPagination(element: MacroApi.Context.IElementApi): void{   

        CursorPaginateApi.addPagingParameters(element, "DTO-Field");    
        CursorPaginateApi.changeReturnType(element);
    }
}
