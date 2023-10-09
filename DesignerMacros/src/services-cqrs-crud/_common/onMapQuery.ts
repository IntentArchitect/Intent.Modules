/// <reference path="./onMapFunctions.ts" />

function onMapQuery(element: MacroApi.Context.IElementApi): void {
    var complexTypes: Array<string> = ["Data Contract", "Value Object"];

    let fields = element.getChildren("DTO-Field")
        .filter(x => x.typeReference.getType()?.specialization != "DTO" && x.isMapped() && x.getMapping().getElement().specialization.startsWith("Association"));

    fields.forEach(f => {
        getOrCreateQueryCrudDto(element, f);
    });

    let complexAttributes = element.getChildren("DTO-Field")
        .filter(x => x.typeReference.getType()?.specialization != "DTO"
            && (complexTypes.includes(x.getMapping()?.getElement()?.typeReference?.getType()?.specialization)
            ));

    complexAttributes.forEach(f => {
        getOrCreateQueryCrudDto(element, f);
    });
}

function getOrCreateQueryCrudDto(element: MacroApi.Context.IElementApi, dtoField: MacroApi.Context.IElementApi) {
    const projectMappingSettingId = "942eae46-49f1-450e-9274-a92d40ac35fa";

    let mappedElement = dtoField.getMapping().getElement();

    let domainName = mappedElement.typeReference.getType().getName();
    let baseName = element.getMetadata("baseName")
        ? `${element.getMetadata("baseName")}${domainName}`
        : `${domainName}`;
    let dtoName = baseName;
    let dto = getOrCreateDto(dtoName, element.getParent());
    dto.setMapping(mappedElement.typeReference.getTypeId(), projectMappingSettingId);
    dto.setMetadata("baseName", baseName);

    ensureDtoFieldsQuery(mappedElement, dto);

    dtoField.typeReference.setType(dto.id);
}

function ensureDtoFieldsQuery(mappedElement: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi) {
    let dtoUpdated = false;
    let mappedElementAttributes = mappedElement
        .typeReference
        .getType()
        .getChildren("Attribute");
    let dtoFields = dto.getChildren("DTO-Field");
    for (let attribute of mappedElementAttributes.filter(x => !dtoFields.some(y => x.getName() === y.getName()))) {
        if (dto.getChildren("DTO-Field").some(x => x.getName() == attribute.getName())) {
            continue;
        }
        let field = createElement("DTO-Field", attribute.getName(), dto.id);
        field.typeReference.setType(attribute.typeReference.typeId);
        field.typeReference.setIsNullable(attribute.typeReference.isNullable);
        field.typeReference.setIsCollection(attribute.typeReference.isCollection);
        field.setMapping(attribute.id);
        dtoUpdated = true;
    }

    if (dtoUpdated) {
        dto.collapse();
    }
}
