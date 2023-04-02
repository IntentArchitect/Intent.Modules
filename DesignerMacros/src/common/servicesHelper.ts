class ServicesConstants {
    static dtoToEntityMappingId = "01d74d4f-e478-4fde-a2f0-9ea92255f3c5";
    static dtoFromEntityMappingId = "1f747d14-681c-4a20-8c68-34223f41b825";
}

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

interface IElementSettings {
    childSpecialization: string;
    childNameFormat?: "camel-case" | "pascal-case";
}
class ElementManager {
    private mappedElement: MacroApi.Context.IElementApi;

    constructor(private command: MacroApi.Context.IElementApi, private settings: IElementSettings){}
    
    get id(): string { return this.command.id; };

    setReturnType(typeId: string, isCollection?: boolean): ElementManager {
        this.command.typeReference.setType(typeId);
        if (isCollection != null) {
            this.command.typeReference.setIsCollection(isCollection);
        }
        return this;
    }

    addChild(name: string, typeId?: string): MacroApi.Context.IElementApi {
        let field = createElement(this.settings.childSpecialization, ServicesHelper.getFieldFormat(name), this.command.id);
        if (typeId) {
            field.typeReference.setType(typeId)
        }
        return field;
    }

    

    addChildrenFrom(elements: IAttributeWithMapPath[]) {
        elements.forEach(e => {
            if (this.command.getChildren(this.settings.childSpecialization).some(x => x.getMapping()?.getElement()?.id == e.id)) { 
                return;
            }
            let field = this.addChild(ServicesHelper.getFieldFormat(e.name), e.typeId);
            field.typeReference.setIsCollection(e.isCollection);
            field.typeReference.setIsNullable(e.isNullable);
            if (this.mappedElement != null && e.mapPath) {
                field.setMapping(e.mapPath);
            }
        });
        return this;
    }

    mapToElement(entity: MacroApi.Context.IElementApi, mappingSettingsId?: string): ElementManager {
        this.mappedElement = entity;
        this.command.setMapping(entity.id, mappingSettingsId);
        return this;
    }

    getElement(): MacroApi.Context.IElementApi {
        return this.command;
    }

    collapse(): void {
        this.command.collapse();
    }

}