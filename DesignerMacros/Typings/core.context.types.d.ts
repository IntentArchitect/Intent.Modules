// By having these in a separate file, it can still be referred to manually by the .api.d.ts file
// but also be available to rest of the TypeScript project so that we can ensure the types match.

declare namespace MacroApi.Context {
    interface IDialogService {
        /**
         * Shows an information dialog with the provided message.
         *
         * Must be called with await.
         * @param message The message to show.
         */
        info(message: string): Promise<void>;

        /**
         * Shows an information dialog with the provided message.
         *
         * Must be called with await.
         * @param message The message to show.
         */
        warn(message: string): Promise<void>;

        /**
         * Shows an warning dialog with the provided message.
         *
         * Must be called with await.
         * @param message The message to show.
         */
        error(message: string): Promise<void>;

        /**
         * Shows a dialog which allows the user to select from the provided options.
         *
         * Must be called with await.
         * @param options The available options of which one can be selected.
         */
        lookupFromOptions(options: { id: string, name: string }[]): Promise<string>;
    }

    interface IGenericTypeParameter {
        typeId: string;
        isNullable: boolean;
        isCollection: boolean;
        genericTypeParameters?: IGenericTypeParameter[]
    }

    interface ITypeReference {
        typeId: string;
        isTypeFound(): boolean;
        getTypeId(): string;
        getType(): IElementApi;
        setType(typeId: string, genericTypeParameters?: IGenericTypeParameter[]): void;
        getIsNullable(): boolean;
        setIsNullable(value: boolean): void;
        getIsCollection(): boolean;
        setIsCollection(value: boolean): void;
        isNavigable: boolean;
        isNullable: boolean;
        isCollection: boolean;
        display: string;
    }
    
    interface IElementMappingApi {
        applicationId: string;
        metadataId: string;
        mappingSettingsId: string;
        getElement(): IElementApi;
    }

    interface IStereotypeApi {
        name: string;
        properties: {};
        lookup(id: string): IElementApi;
        hasProperty(property: string): boolean;
        getProperty(property: string): IStereotypePropertyApi;
        element: IElementApi;
    }

    interface IStereotypePropertyApi {
        value: string;
        getValue(): string | boolean | IElementApi;
        setValue(value: any): void;
        getSelected(): IElementApi;
    }

    interface IPackageApi {
        id: string;
        specialization: string;
        name: string;
        getName(): string;
        setName(value: string): void;
        getChildren(ofType: string): IElementApi[]
        hasStereotype(nameOrDefinitionId: string): boolean;
        getStereotypes(): IStereotypeApi[];
        getStereotype(nameOrDefinitionId: string): IStereotypeApi;
        addStereotype(stereotypeDefinitionId: string): void;
        removeStereotype(nameOrDefinitionId: string): void;
        getMetadata(key: string): string;
        hasMetadata(key: string): boolean;
        addMetadata(key: string, value: string);
        setMetadata(key: string, value: string);
        removeMetadata(key: string);
    }

    interface IBackwardCompatibleElementApi extends IElementApi {
        name: string;
        comment: string;
        value: string;
        genericTypes: string;
        children(type: string): IBackwardCompatibleElementApi[];
    }

    interface IElementApi {
        specialization: string;
        id: string;
        getName(): string;
        setName(value: string): void;
        getComment(): string;
        setComment(value: string): void;
        getValue(): string;
        setValue(value: string): void;
        getExternalReference(): string;
        setExternalReference(value: string): void;
        getGenericTypesDisplay(): string;
        hasType: boolean;
        typeReference?: ITypeReference;
        getChildren(type: string): IElementApi[];
        loadDiagram(): void;
        getParent(type?: string): IElementApi;
        setParent(parentId: string);
        getPackage(): IPackageApi;
        isMapped(): boolean;
        getMapping(): IElementMappingApi;
        setMapping(elementId: string | string[], mappingSettingsId?: string): void;
        /**
         * Launches the mapping dialog for the element for the provided mappingSettingsId.
         *
         * Must be called with await.
         * @param mappingSettingsId
         */
        launchMappingDialog(mappingSettingsId?: string): Promise<void>
        hasStereotype(nameOrDefinitionId: string): boolean;
        getStereotypes(): IStereotypeApi[];
        getStereotype(nameOrDefinitionId: string): IStereotypeApi;
        addStereotype(stereotypeDefinitionId: string): void;
        removeStereotype(nameOrDefinitionId: string): void;
        lookup(id: string): IElementApi;
        lookupTypesOf(type: string): IElementApi[];
        expand(): void;
        collapse(): void;
        delete(): void;
        getAssociations(type?: string): IAssociationApi[];
        setOrder(index: number): void;
        getMetadata(key: string): string;
        hasMetadata(key: string): boolean;
        addMetadata(key: string, value: string);
        setMetadata(key: string, value: string);
        removeMetadata(key: string);
    }

    interface IAssociationApi {
        specialization: string;
        id: string;
        name: string;
        getName(): string;
        setName(value: string): void;
        typeReference?: ITypeReference;
        isSourceEnd(): boolean;
        isTargetEnd(): boolean;
        getOtherEnd(): IAssociationApi;
        getStereotypes(): IStereotypeApi[];
        getStereotype(name: string): IStereotypeApi;
        getOtherEnd(): IAssociationApi;
        getChildren(type: string): IElementApi[];
        getMetadata(key: string): string;
        hasMetadata(key: string): boolean;
        addMetadata(key: string, value: string);
        setMetadata(key: string, value: string);
        removeMetadata(key: string);
    }

    interface IThemeApi {
        isDark: boolean;
    }

    interface IApplication {
        name: string;
        id: string;
        description: string;
        settings: IApplicationSettings[];
        getSettings(identifier: string): IApplicationSettings;
    }

    interface IApplicationSettings {
        id: string;
        title: string;
        module: string;
        fields: IApplicationSettingsField[];
        getField(identifier: string): IApplicationSettingsField;
    }

    interface IApplicationSettingsField {
        id: string;
        title: string;
        module: string;
        value: string;
    }
}
