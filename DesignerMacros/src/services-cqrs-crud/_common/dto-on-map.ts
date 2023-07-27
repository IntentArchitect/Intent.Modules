/// <reference path="./on-map-functions.ts" />

function onMapDto(element: MacroApi.Context.IElementApi): void {
    var complexTypes: Array<string> = ["Data Contract", "Value Object"];

    let fields = element.getChildren("DTO-Field")
        .filter(x => x.typeReference.getType() == null && x.getMapping()?.getElement()?.specialization === "Association");

    fields.forEach(f => {
        getOrCreateDtoCrudDto(element, f, true);
    });

    let complexAttributes = element.getChildren("DTO-Field")
        .filter(x => x.typeReference.getType() == null
            && (complexTypes.includes(x.getMapping()?.getElement()?.typeReference?.getType()?.specialization)
            ));

    complexAttributes.forEach(f => {
        getOrCreateDtoCrudDto(element, f, false);
    });
}

function getOrCreateDtoCrudDto(element: MacroApi.Context.IElementApi, dtoField: MacroApi.Context.IElementApi, autoAddPrimaryKey: boolean) {
    const projectMappingSettingId = "942eae46-49f1-450e-9274-a92d40ac35fa";
    const originalDtoMappingSettingId = "1f747d14-681c-4a20-8c68-34223f41b825";

    let mappedElement = dtoField.getMapping().getElement();

    let originalVerb = "";
    if (element.hasMetadata("originalVerb")) {
        originalVerb = element.getMetadata("originalVerb");
        // In the event that the prefix is no longer the same as the
        // originally called verb, then don't propagate this any further
        // as end users might get confused.
        if (element.getName().indexOf(originalVerb) < 0) {
            originalVerb = "";
        }
    }

    let targetMappingSettingId = dtoField.getParent().getMapping().mappingSettingsId;

    let domainName = mappedElement.typeReference.getType().getName();
    let baseName = element.getMetadata("baseName")
        ? `${element.getMetadata("baseName")}${domainName}`
        : domainName;
    let dtoName = `${originalVerb}${baseName}`;
    let dto = getOrCreateDto(dtoName, element.getParent());
    dto.setMapping(mappedElement.typeReference.getTypeId(), targetMappingSettingId);
    if (originalVerb !== "") {
        dto.setMetadata("originalVerb", originalVerb);
    }
    dto.setMetadata("baseName", baseName);
    ensureDtoFields(autoAddPrimaryKey, mappedElement, dto);

    dtoField.typeReference.setType(dto.id);
}



