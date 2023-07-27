/// <reference path="./on-map-functions.ts" />

function onMapCommand(element: MacroApi.Context.IElementApi): void {

    const projectMappingSettingId = "942eae46-49f1-450e-9274-a92d40ac35fa";
    const mapFromDomainMappingSettingId = "1f747d14-681c-4a20-8c68-34223f41b825";

    var complexTypes: Array<string> = ["Data Contract", "Value Object"];
    let isOperationMappedCommand = element.getMapping() && element.getMapping().getElement().specialization === "Operation";

    if (isOperationMappedCommand) {
        //Add the entity PK for the repo lookup to invoke the operation
        let entityPkDescr = getPrimaryKeyDescriptor(element);
        addPrimaryKeys(element, entityPkDescr);
        //check for return type
        if (complexTypes.includes(element.getMapping()?.getElement()?.typeReference?.getType()?.specialization)) {
            getOrCreateCommandCrudDto(element, element, false, mapFromDomainMappingSettingId);
        }
    }

    let fields = element.getChildren("DTO-Field")
        .filter(x => x.typeReference.getType() == null && x.getMapping().getElement().specialization === "Association");

    fields.forEach(f => {
        getOrCreateCommandCrudDto(element, f, true, projectMappingSettingId);
    });

    let complexFields = element.getChildren("DTO-Field")
        .filter(x => x.typeReference.getType() == null
            && (complexTypes.includes(x.getMapping()?.getElement()?.typeReference?.getType()?.specialization)
            ));

    complexFields.forEach(cf => {
        getOrCreateCommandCrudDto(element, cf, false, projectMappingSettingId);
    });
}

function getOrCreateCommandCrudDto(command: MacroApi.Context.IElementApi, dtoField: MacroApi.Context.IElementApi, autoAddPrimaryKey: boolean, mappingTypeSettingId: string) {
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
