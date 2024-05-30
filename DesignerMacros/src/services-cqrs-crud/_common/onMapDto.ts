/// <reference path="./onMapFunctions.ts" />
/// <reference path="../../common/crudHelper.ts" />
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

const stringTypeId: string = "d384db9c-a279-45e1-801e-e4e8099625f2";

function onMapDto(element: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi, autoAddPrimaryKey: boolean = true, dtoPrefix: string = null, inbound: boolean = false ): void {

    let fields = element.getChildren("DTO-Field")
        .filter(x => x.typeReference.getType()?.specialization != "DTO" && x.getMapping()?.getElement()?.specialization.startsWith("Association"));

    fields.forEach(f => {
        let targetMappingSettingId = f.getParent().getMapping().mappingSettingsId;
        let newDto = CrudHelper.getOrCreateCrudDto(CrudHelper.getName(element, f.getMapping().getElement().typeReference.getType(), dtoPrefix), f.getMapping().getElement(), autoAddPrimaryKey, targetMappingSettingId, folder, inbound);
        f.typeReference.setType(newDto.id);
    });        

    let complexAttributes = element.getChildren("DTO-Field")
        .filter(x => x.typeReference.getType()?.specialization != "DTO"
            && (DomainHelper.isComplexType(x.getMapping()?.getElement()?.typeReference?.getType())));

    complexAttributes.forEach(f => {
        let targetMappingSettingId = f.getParent().getMapping().mappingSettingsId;
        let newDto = CrudHelper.getOrCreateCrudDto(CrudHelper.getName(element, f.getMapping().getElement(), dtoPrefix), f.getMapping().getElement().typeReference.getType(), false, targetMappingSettingId, folder, inbound);
        f.typeReference.setType(newDto.id);
    });
}
/*
function getOrCreateDtoCrudDto(element: MacroApi.Context.IElementApi, dtoField: MacroApi.Context.IElementApi, autoAddPrimaryKey: boolean, dtoPrefix: string = null) {
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
    if (dtoPrefix)
        dtoName = `${dtoPrefix}${dtoName}`;
    let dto = getOrCreateDto(dtoName, element.getParent());
    dto.setMapping(mappedElement.typeReference.getTypeId(), targetMappingSettingId);
    if (originalVerb !== "") {
        dto.setMetadata("originalVerb", originalVerb);
    }
    dto.setMetadata("baseName", baseName);
    ensureDtoFields(autoAddPrimaryKey, mappedElement, dto);

    dtoField.typeReference.setType(dto.id);
}
*/


