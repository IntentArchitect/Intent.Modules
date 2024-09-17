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
