/// <reference path="attributeWithMapPath.ts" />

class ServicesConstants {
    static dtoToEntityMappingId = "942eae46-49f1-450e-9274-a92d40ac35fa";//"01d74d4f-e478-4fde-a2f0-9ea92255f3c5";
    static dtoFromEntityMappingId = "1f747d14-681c-4a20-8c68-34223f41b825";
    static dtoToDomainOperation = "8d1f6a8a-77c8-43a2-8e60-421559725419";
}

class ServicesHelper {
    static addDtoFieldsFromDomain(dto: MacroApi.Context.IElementApi, attributes: IAttributeWithMapPath[]): void {
        for (let key of attributes) {
            if (dto && !dto.getChildren("DTO-Field").some(x => x.getName() == ServicesHelper.getFieldFormat(key.name))) {
                let field = createElement("DTO-Field", ServicesHelper.getFieldFormat(key.name), dto.id);
                field.typeReference.setType(key.typeId);
    
                if ((key.mapPath ?? []).length > 0) {
                    field.setMapping(key.mapPath);
                }
            }
        }
    }

    static getParameterFormat(str: string): string {
        return toCamelCase(str);
    }

    static getRoutingFormat(str: string): string {
        return pluralize(str);
    }

    static getFieldFormat(str: string): string {
        return toPascalCase(str);
    }

    static formatName(str: string, type: "class" | "property" | "parameter"): string {
        switch (type) {
            case "property":
            case "class":
                return toPascalCase(str);
            case "parameter":
                return toCamelCase(str);
            default:
                return str;
        }
    }
}

interface IElementSettings {
    childSpecialization: string;
    childType?: "class" | "property" | "parameter";
}