class ServicesHelper {
    static addDtoFieldsFromDomain(dto : MacroApi.Context.IElementApi, attributes: IAttributeWithMapPath[]) {
        for (let key of attributes) {
            if (dto && !dto.getChildren("DTO-Field").some(x => x.getName() == this.getFieldFormat(key.name))) {
                let primaryKeyDtoField = createElement("DTO-Field", this.getFieldFormat(key.name), dto.id);
                primaryKeyDtoField.typeReference.setType(key.typeId)
                primaryKeyDtoField.setMapping(key.id);
            }
        }
    }

    static getParameterFormat(str) : string {
        return toCamelCase(str);
    }
    
    static getRoutingFormat(str) : string {
        return pluralize(str);
    }
    
    static getFieldFormat(str) : string {
        return toPascalCase(str);
    }
}