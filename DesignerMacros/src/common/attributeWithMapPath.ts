/// <reference path="../../typings/elementmacro.context.api.d.ts"/>

interface IAttributeWithMapPath {
    id: string,
    name: string,
    typeId: string,
    typeReferenceModel: MacroApi.Context.ITypeReferenceData;
    mapPath: string[],
    isNullable: boolean,
    isCollection: boolean
}