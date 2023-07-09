/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/domainHelper.ts" />

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

function getServiceRouteIdentifier(element: MacroApi.Context.IElementApi, domainClass : MacroApi.Context.IElementApi, defaultEntityName : string) : string{
    let domainMappedPks = getRouteEntityIds(element, domainClass, defaultEntityName);
    let serviceRouteIdentifier = "";
    if (domainMappedPks.length > 0){
        domainMappedPks.forEach(dmpk => {
            serviceRouteIdentifier += `/{${toCamelCase(dmpk.getName())}}`;
        })
    }
    
    return serviceRouteIdentifier;
}

function getConventionalSubRoute(element: MacroApi.Context.IElementApi, mappedEntity: MacroApi.Context.IElementApi ) : string{
    let subRoute = `/${toKebabCase(pluralize(mappedEntity.getName()))}`;
    let mappedDomainEntity = getDomainEntity(mappedEntity.getName());
    let mappedPks = getRouteEntityIds(element, mappedDomainEntity, mappedEntity.getName());
    if (mappedPks.length > 0){
        mappedPks.forEach(mpk => {
            subRoute += `/{${toCamelCase(mpk.getName())}}`;
        })
    }    

    return subRoute;
}

function getDomainEntity(possibleName : string) : MacroApi.Context.IElementApi{
    let possibleNames = [singularize(possibleName), pluralize(possibleName)]; 
    return lookupTypesOf("Class").find(x => possibleNames.includes( x.getName()));
}

function getRouteEntityIds(element: MacroApi.Context.IElementApi, domainClass : MacroApi.Context.IElementApi, defaultEntityName : string) : MacroApi.Context.IElementApi[]{
    let result :MacroApi.Context.IElementApi[] = [];

    if (domainClass != null){
        let keys = DomainHelper.getPrimaryKeys(domainClass);
        if (!(keys.length == 1 && keys[0].name.toLowerCase() == 'id')){
            let children = element.getChildren();
            children.forEach(field => {
                if (keys.find(pk => pk.name.toLowerCase() == field.getName().toLowerCase() 
                                    || singularize(defaultEntityName.toLowerCase()) + pk.name.toLowerCase() == field.getName().toLowerCase())){
                    result.push(field);
                }
            });
        }
    }
    if (result.length == 0){    
        let conventionIdAttribute = element.getChildren().find(x => x.getName().toLowerCase() == `${singularize(defaultEntityName.toLowerCase())}id`);
        let idAttribute = element.getChildren().find(x => x.getName().toLowerCase() == "id");
        if (conventionIdAttribute != null){
            result.push(conventionIdAttribute);
        }else if (idAttribute != null){
            result.push(idAttribute);
        }
    }
    return result;
}