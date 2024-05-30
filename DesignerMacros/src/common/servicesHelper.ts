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

class ElementManager {
    private mappedElement: MacroApi.Context.IElementApi;

    constructor(private command: MacroApi.Context.IElementApi, private settings: IElementSettings) {
        this.mappedElement = command.getMapping()?.getElement();
    }

    get id(): string { return this.command.id; };

    setReturnType(typeId: string, isCollection?: boolean, isNullable?: boolean): ElementManager {
        this.command.typeReference.setType(typeId);
        if (isCollection != null) {
            this.command.typeReference.setIsCollection(isCollection);
        }
        if (isNullable != null) {
            this.command.typeReference.setIsNullable(isNullable);
        }
        return this;
    }

    addChild(name: string, type?: string | MacroApi.Context.ITypeReference): MacroApi.Context.IElementApi {
        let field = createElement(this.settings.childSpecialization, ServicesHelper.formatName(name, this.settings.childType ?? "property"), this.command.id);
        const typeReferenceDetails = type == null
            ? null
            : typeof (type) === "string"
                ? { id: type as string, isNullable: false, isCollection: false }
                : { id: type.typeId, isNullable: type.isNullable, isCollection: type.isCollection };

        if (typeReferenceDetails != null) {
            field.typeReference.setType(typeReferenceDetails.id);
            field.typeReference.setIsCollection(typeReferenceDetails.isCollection);
            field.typeReference.setIsNullable(typeReferenceDetails.isNullable);
        }

        return field;
    }

    addChildrenFrom(elements: IAttributeWithMapPath[], options?: { addToTop: boolean }): ElementManager {
        let order = 0;

        elements.forEach(e => {
            if (e.mapPath != null) {
                if (this.command.getChildren(this.settings.childSpecialization).some(x => x.getMapping()?.getElement()?.id == e.id)) {
                    return;
                }
            }
            else if (this.command.getChildren(this.settings.childSpecialization).some(x => x.getName().toLowerCase() === e.name.toLowerCase())) {
                return;
            }

            let field = this.addChild(e.name, e.typeId);
            field.typeReference.setIsCollection(e.isCollection);
            field.typeReference.setIsNullable(e.isNullable);

            if (options?.addToTop) {
                field.setOrder(order++);
            }

            if (this.mappedElement != null && e.mapPath) {
                field.setMapping(e.mapPath);
            }
        });

        return this;
    }

    mapToElement(elementIds: string[], mappingSettingsId?: string): ElementManager;
    mapToElement(element: MacroApi.Context.IElementApi, mappingSettingsId?: string): ElementManager;
    mapToElement(param1: string[] | MacroApi.Context.IElementApi, mappingSettingsId?: string): ElementManager {
        let elementIds: string[];
        let element: MacroApi.Context.IElementApi;

        if (Array.isArray(param1)) {
            elementIds = param1;
            element = lookup(elementIds[elementIds.length - 1]);
        }
        else {
            elementIds = [param1.id];
            element = param1;
        }

        this.mappedElement = element;
        this.command.setMapping(elementIds, mappingSettingsId);
        return this;
    }

    getElement(): MacroApi.Context.IElementApi {
        return this.command;
    }

    collapse(): void {
        this.command.collapse();
    }

}