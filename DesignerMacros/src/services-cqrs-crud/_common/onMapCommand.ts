/// <reference path="./onMapFunctions.ts" />
/// <reference path="../../common/domainHelper.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />
/// <reference path="ensureHasField.ts" />


function onMapCommand(
    element: MacroApi.Context.IElementApi,
    isForCrudScript: boolean,
    excludePrimaryKeys: boolean = false
): void {
    const projectMappingSettingId = "942eae46-49f1-450e-9274-a92d40ac35fa";
    const mapFromDomainMappingSettingId = "1f747d14-681c-4a20-8c68-34223f41b825";
    const mapToDomainOperationSettingId = "7c31c459-6229-4f10-bf13-507348cd8828";
    const mapToDomainConstructorForDtosSettingId = "8d1f6a8a-77c8-43a2-8e60-421559725419";
    

    const mappingDetails = getMappedRequestDetails(element);
    if (mappingDetails == null) {
        return;
    }

    if (isForCrudScript ||
        mappingDetails.mappingTargetType !== "Class"
    ) {
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

    if (mappingDetails.mappingTargetType === "Operation" &&
        isComplexType(element.getMapping()?.getElement()?.typeReference?.getType())
    ) {
        getOrCreateCommandCrudDto(element, element, false, mapFromDomainMappingSettingId);
    }

    const fields = element.getChildren("DTO-Field")
        .filter(x => x.typeReference.getType()?.specialization != "DTO" && x.isMapped() && x.getMapping()?.getElement().specialization.startsWith("Association"));

    fields.forEach(field => {
        getOrCreateCommandCrudDto(element, field, !excludePrimaryKeys, projectMappingSettingId);
    });

    const complexFields = element.getChildren("DTO-Field")
        .filter(x =>
            x.typeReference.getType()?.specialization != "DTO" &&
            isComplexType(x.getMapping()?.getElement()?.typeReference?.getType()));

    complexFields.forEach(cf => {
        getOrCreateCommandCrudDto(element, cf, false, projectMappingSettingId);
    });

    function isComplexType(element: MacroApi.Context.IElementApi): boolean {
        return element?.specialization === "Data Contract" ||
            element?.specialization === "Value Object" ||
            element?.specialization === "Class";
    }

    function isComplexTypeById(typeId: string): boolean {
        let element = lookup(typeId);
        return isComplexType(element);
    }

    /*
    function getOrCreateCommandCrudDto(
        command: MacroApi.Context.IElementApi,
        dtoField: MacroApi.Context.IElementApi,
        autoAddPrimaryKey: boolean,
        mappingTypeSettingId: string
    ): void {
        let mappedElement = dtoField.getMapping().getElement();
        if (mappedElement.typeReference == null) throw new Error("TypeReference is undefined");

        let originalVerb = (command.getName().split(/(?=[A-Z])/))[0];
        let domainName = mappedElement.typeReference.getType().getName();
        let baseName = command.getMetadata("baseName")
            ? `${command.getMetadata("baseName")}${domainName}`
            : domainName;
        let dtoName = `${originalVerb}${baseName}`;
        let dto = getOrCreateDto(dtoName, command.getParent());
        dto.setMetadata("originalVerb", originalVerb);
        dto.setMetadata("baseName", baseName);

        const entityCtor: MacroApi.Context.IElementApi = mappedElement.typeReference.getType()
        .getChildren("Class Constructor")
        .sort((a, b) => {
            // In descending order:
            return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
        })[0];
        if (entityCtor != null) {
            dto.setMapping([mappedElement.typeReference.getTypeId(), entityCtor.id], mapToDomainConstructorForDtosSettingId);
            ensureDtoFieldsForCtor(autoAddPrimaryKey, entityCtor, dto);
        } else {
            dto.setMapping(mappedElement.typeReference.getTypeId(), mappingTypeSettingId);
            ensureDtoFields(autoAddPrimaryKey, mappedElement, dto);
        }

        dtoField.typeReference.setType(dto.id);
    }*/

    function getOrCreateCommandCrudDto(
        command: MacroApi.Context.IElementApi,
        dtoField: MacroApi.Context.IElementApi,
        autoAddPrimaryKey: boolean,
        mappingTypeSettingId: string
    ): MacroApi.Context.IElementApi {
        let mappedElement = dtoField.getMapping().getElement();
        if (mappedElement.typeReference == null) throw new Error("TypeReference is undefined");

        let originalVerb = (command.getName().split(/(?=[A-Z])/))[0];
        let domainName = mappedElement.typeReference.getType().getName();
        let baseName = command.getMetadata("baseName")
            ? `${command.getMetadata("baseName")}${domainName}`
            : domainName;
        let dtoName = `${originalVerb}${baseName}`;
        let dto = getOrCreateDto(dtoName, command.getParent());
        dto.setMetadata("originalVerb", originalVerb);
        dto.setMetadata("baseName", baseName);

        dtoField.typeReference.setType(dto.id);
        const entityCtor: MacroApi.Context.IElementApi = mappedElement.typeReference.getType()
        .getChildren("Class Constructor")
        .sort((a, b) => {
            // In descending order:
            return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
        })[0];
        if (entityCtor != null) {
            dto.setMapping([mappedElement.typeReference.getTypeId(), entityCtor.id], mapToDomainConstructorForDtosSettingId);
            addDtoFieldsForCtor(autoAddPrimaryKey, entityCtor, dto);
        } else {
            dto.setMapping(mappedElement.typeReference.getTypeId(), mappingTypeSettingId);
            addDtoFields(autoAddPrimaryKey, mappedElement, dto);
        }

        return dto;
    }

    function addDtoFieldsForCtor(autoAddPrimaryKey: boolean, ctor: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi) {
    
        let childrenToAdd = DomainHelper.getChildrenOfType(ctor, "Parameter").filter(x => x.typeId != null && lookup(x.typeId).specialization !== "Domain Service");
    
        childrenToAdd.forEach(e => {
            if (e.mapPath != null) {
                if (dto.getChildren("Parameter").some(x => x.getMapping()?.getElement()?.id == e.id)) {
                    return;
                }
            }
            else if (ctor.getChildren("Parameter").some(x => x.getName().toLowerCase() === e.name.toLowerCase())) {
                return;
            }
    
            let field = createElement("DTO-Field", toPascalCase(e.name), dto.id);
            field.setMapping(e.mapPath);
            if (isComplexTypeById(e.typeId)){
                let newDto = getOrCreateCommandCrudDto(dto, field, autoAddPrimaryKey, mapFromDomainMappingSettingId );
                field.typeReference.setType(newDto.id);
            }else{
                field.typeReference.setType(e.typeId);
            }
            field.typeReference.setIsCollection(e.isCollection);
            field.typeReference.setIsNullable(e.isNullable);
        });
    
        dto.collapse();
    }
    
    function addDtoFields(autoAddPrimaryKey: boolean, mappedElement: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi) {
        let dtoUpdated = false;
        let domainElement = mappedElement
            .typeReference
            .getType();
        let attributesWithMapPaths = getAttributesWithMapPath(domainElement);
        let isCreateMode = dto.getMetadata("originalVerb")?.toLowerCase()?.startsWith("create") == true;
        for (var keyName of Object.keys(attributesWithMapPaths)) {
            let entry = attributesWithMapPaths[keyName];
            if (isCreateMode && isOwnerForeignKey(entry.name, domainElement)) {
                continue;
            }
            if (dto.getChildren("DTO-Field").some(x => x.getName() == entry.name)) {
                continue;
            }
            let field = createElement("DTO-Field", entry.name, dto.id);
            field.typeReference.setType(entry.typeId);
            field.typeReference.setIsNullable(entry.isNullable);
            field.typeReference.setIsCollection(entry.isCollection);
            field.setMapping(entry.mapPath);
            dtoUpdated = true;
        }
    
        if (autoAddPrimaryKey && !isCreateMode) {
            addPrimaryKeys(dto, domainElement, true);
        }    
    
        if (dtoUpdated) {
            dto.collapse();
        }
    }  
}
