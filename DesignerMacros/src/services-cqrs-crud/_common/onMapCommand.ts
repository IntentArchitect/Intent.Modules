/// <reference path="./onMapFunctions.ts" />
/// <reference path="../../common/domainHelper.ts" />
/// <reference path="../../common/crudHelper.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />
/// <reference path="ensureHasField.ts" />


function onMapCommand(
    element: MacroApi.Context.IElementApi,
    isForCrudScript: boolean,
    excludePrimaryKeys: boolean = false,
    inbound: boolean = false
): void {
    const projectMappingSettingId = "942eae46-49f1-450e-9274-a92d40ac35fa";
    const mapFromDomainMappingSettingId = "1f747d14-681c-4a20-8c68-34223f41b825";    

    const mappingDetails = getMappedRequestDetails(element);

    if (mappingDetails && (isForCrudScript || mappingDetails.mappingTargetType !== "Class" )) {
        let order = 0;
        let keyFields = mappingDetails.ownerKeyFields;
        if (!excludePrimaryKeys) {
            keyFields = keyFields.concat(mappingDetails.entityKeyFields);
        }

        for (const keyField of keyFields) {
            ensureHasField({
                contract: element,
                fieldDetail: keyField,
                order: order++
            });
        }
    }

    if (mappingDetails && 
        mappingDetails.mappingTargetType === "Operation" &&
        DomainHelper.isComplexType(element.getMapping()?.getElement()?.typeReference?.getType())) {
            
        let mappedElement = element.getMapping().getElement();
        let newDto = CrudHelper.getOrCreateCrudDto( CrudHelper.getName(element, mappedElement)
            , mappedElement.typeReference.getType()
            , false
            , mapFromDomainMappingSettingId
            , element.getParent()
            , false);
        setTypeRef(element, newDto, mappedElement);
    }

    const fields = element.getChildren("DTO-Field")
        .filter(x => x.typeReference.getType()?.specialization != "DTO" && x.isMapped() && x.getMapping()?.getElement().specialization.startsWith("Association"));

    fields.forEach(field => {
        let mappedElement = field.getMapping().getElement();
        let newDto = CrudHelper.getOrCreateCrudDto( CrudHelper.getName(element, mappedElement)
            , mappedElement.typeReference.getType()
            , !excludePrimaryKeys
            , projectMappingSettingId
            , element.getParent()
            , inbound);

        setTypeRef(field, newDto, mappedElement);
    });

    const complexFields = element.getChildren("DTO-Field")
        .filter(x =>
            x.typeReference.getType()?.specialization != "DTO" &&
            DomainHelper.isComplexType(x.getMapping()?.getElement()?.typeReference?.getType()));

    complexFields.forEach(cf => {
        let mappedElement = cf.getMapping().getElement();
        let newDto = CrudHelper.getOrCreateCrudDto( CrudHelper.getName(element, mappedElement)
            , mappedElement.typeReference.getType()
            , false
            , projectMappingSettingId
            , element.getParent()
            , inbound);

        setTypeRef(cf, newDto, mappedElement);
    });

    function setTypeRef(element: MacroApi.Context.IElementApi, newDto: MacroApi.Context.IElementApi , mappedElement: MacroApi.Context.IElementApi){
        element.typeReference.setType(newDto.id);
        if (mappedElement?.typeReference?.isCollection != null) {
            element.typeReference.setIsCollection(mappedElement.typeReference.isCollection);
        }
        if (mappedElement?.typeReference?.isNullable) {
            element.typeReference.setIsNullable(mappedElement.typeReference.isNullable);
        }
    }
}
