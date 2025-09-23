class ElementManager {
    private mappedElement: MacroApi.Context.IElementApi;

    constructor(private innerElement: MacroApi.Context.IElementApi, private settings: IElementSettings) {
        this.mappedElement = innerElement.getMapping()?.getElement();
    }

    get id(): string { return this.innerElement.id; };

    setReturnType(typeId: string, isCollection?: boolean, isNullable?: boolean): ElementManager {
        this.innerElement.typeReference.setType(typeId);
        if (isCollection != null) {
            this.innerElement.typeReference.setIsCollection(isCollection);
        }
        if (isNullable != null) {
            this.innerElement.typeReference.setIsNullable(isNullable);
        }
        return this;
    }

    addChild(name: string, type?: string | MacroApi.Context.ITypeReference): MacroApi.Context.IElementApi {
        let existingField = this.innerElement.getChildren(this.settings.childSpecialization)
            .find(c => c.getName().toLowerCase() == ServicesHelper.formatName(name, this.settings.childType ?? "property").toLowerCase());

        let field = existingField ?? createElement(this.settings.childSpecialization, ServicesHelper.formatName(name, this.settings.childType ?? "property"), this.innerElement.id);

        if (type != null) {
            if (typeof (type) === "string") {
                field.typeReference.setType(type as string);
                field.typeReference.setIsCollection(false);
                field.typeReference.setIsNullable(false);
            } else {
                field.typeReference.setType(type.toModel());
            }
        }

        return field;
    }

    addChildrenFrom(elements: IAttributeWithMapPath[], options?: { addToTop: boolean }): ElementManager {
        let order = 0;

        elements.forEach(e => {
            if (e.mapPath != null) {
                if (this.innerElement.getChildren(this.settings.childSpecialization).some(x => x.getMapping()?.getElement()?.id == e.id)) {
                    return;
                }
            }
            else if (this.innerElement.getChildren(this.settings.childSpecialization).some(x => x.getName().toLowerCase() === e.name.toLowerCase())) {
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
        this.innerElement.setMapping(elementIds, mappingSettingsId);
        return this;
    }

    getElement(): MacroApi.Context.IElementApi {
        return this.innerElement;
    }

    collapse(): void {
        this.innerElement.collapse();
    }

}