/// <reference path="./onMapFunctions.ts" />
/// <reference path="../../common/domainHelper.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />
/// <reference path="ensureHasField.ts" />

function onMapCommand(
    element: MacroApi.Context.IElementApi,
    isForCrudScript: boolean,
    isForCreate: boolean = false
): void {
    const projectMappingSettingId = "942eae46-49f1-450e-9274-a92d40ac35fa";
    const mapFromDomainMappingSettingId = "1f747d14-681c-4a20-8c68-34223f41b825";
    // const mapToDomainDataSettingsId = "735c87d0-06fc-4491-8b5f-5adc6f953c54";

    const mappingDetails = getMappedRequestDetails(element);
    if (mappingDetails == null) {
        return;
    }

    if (isForCrudScript ||
        mappingDetails.mappingTargetType !== "Class"
    ) {
        let order = 0;
        let keyFields = mappingDetails.ownerKeyFields;
        if (!isForCreate) {
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
        .filter(x => x.typeReference.getType() == null && x.getMapping().getElement().specialization === "Association");

    fields.forEach(field => {
        getOrCreateCommandCrudDto(element, field, true, projectMappingSettingId);
    });

    const complexFields = element.getChildren("DTO-Field")
        .filter(x =>
            x.typeReference.getType() == null &&
            isComplexType(x.getMapping()?.getElement()?.typeReference?.getType()));

    complexFields.forEach(cf => {
        getOrCreateCommandCrudDto(element, cf, false, projectMappingSettingId);
    });

    function isComplexType(element: MacroApi.Context.IElementApi): boolean {
        return element?.specialization === "Data Contract" ||
            element?.specialization === "Value Object";
    }

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
        dto.setMapping(mappedElement.typeReference.getTypeId(), mappingTypeSettingId);
        ensureDtoFields(autoAddPrimaryKey, mappedElement, dto);
        dtoField.typeReference.setType(dto.id);
    }
}