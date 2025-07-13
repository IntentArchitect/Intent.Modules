// By having these in a separate file, it can still be referred to manually by the .api.d.ts file
// but also be available to rest of the TypeScript project so that we can ensure the types match.

type IApplication = MacroApi.Context.IApplication;
type IElementApi = MacroApi.Context.IElementApi;
type IMappableElementApi = MacroApi.Context.IMappableElementApi;
type IAssociationApi = MacroApi.Context.IAssociationApi;
type IPackageApi = MacroApi.Context.IPackageApi;
type IDialogService = MacroApi.Context.IDialogService;
type IDiagramApi = MacroApi.Context.IDiagramApi;

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
         * Shows a confirmation dialog which returns a promise. An error will be thrown if the user clicks cancel.
         *
         * Must be called with await.
         * @param message The message to show.
         * @param title The confirmation dialog's title (default is "Confirmation").
         */
        confirm(message: string, title?: string): Promise<void>;

        /**
         * Shows a dialog which allows the user to select from the provided options.
         *
         * Must be called with await.
         * @param options The available options of which one can be selected.
         */
        lookupFromOptions(options: { id: string, name: string, additionalInfo?: string }[]): Promise<string>;

        /**
         * Shows a dynamic form dialog which displays fields as per it's provided config and returns
         * the values as input by the user.
         *
         * Must be called with await.
         * @param config The configuration of the form.
         */
        openForm(config: IDynamicFormConfig): Promise<any>;
    }

    interface IDynamicFormConfig {
        title: string;
        /**
         * Sets the primary button text. Defaults to "Done" if not set.
         **/
        submitButtonText?: string;
        /**
         * Sets the dialog's minimum width (e.g. "500px" or "60%"). If not set, the dialog's width will default to "500px".
         **/
        minWidth?: string;
        /**
         * Sets the dialog's maximum width (e.g. "500px" or "60%"). If not set, the dialog's width will default to "60%".
         **/
        maxWidth?: string;
        /**
         * Sets the dialog's height (e.g. "500px" or "60%"). If not set, the dialog will autosize to its content.
         **/
        height?: string;
        /**
         * The field configurations for the fields to be added to the form.
         **/
        fields: IDynamicFormFieldConfig[];

        sections?: IDynamicFormSectionConfig[];
    }

    interface IDynamicFormSectionConfig {
        name: string;
        isHidden: boolean;
        isCollapsed: boolean;
        fields: IDynamicFormFieldConfig[];
    }

    interface IDynamicFormFieldConfig {
        id: string;
        fieldType: "text" | "select" | "checkbox" | "textarea" | "tree-view" | "tiles" | "open-file" | "button"
        label: string;
        isRequired?: boolean;
        isHidden?: boolean;
        isDisabled?: boolean;
        placeholder?: string,
        hint?: string,
        value?: string | string[];
        errorMessage?: string;
        selectOptions?: IDynamicFormFieldSelectOption[];
        treeViewOptions?: ISelectableTreeViewOptions;
        openFileOptions?: IDynamicFormOpenFileOptions
        onClick?: (formApi: IDynamicFormApi) => void;
        onChange?: (formApi: IDynamicFormApi) => void;
    }

    interface IDynamicFormFieldSelectOption {
        id: string;
        description: string;
        additionalInfo?: string;
        icon?: IIcon;
    }

    interface IDynamicFormApi {
        getField(id: string): IDynamicFormFieldConfig;
    }

    interface IDynamicFormOpenFileOptions {
        /**
         * Set the title of the open file dialog.
         */
        title?: string;
        /**
         * Set the location to start browsing with the open file dialog. If not specified, it will open at your last location.  
         */
        defaultPath?: string;
        /**
         * Custom label for the confirmation button, when left empty, the default label will
         * be used.
         */
        buttonLabel?: string;
        fileFilters: IDynamicFormOpenFileOptions_FileFilters[];
    }

    interface IDynamicFormOpenFileOptions_FileFilters {
        name: string;
        extensions: string[];
    }

    interface ISelectableTreeViewOptions {
        /**
         * Specifes the element Id in the current designer that will become the root node in this selectable tree-view.
         */
        rootId?: string;
        /**
         * Specifies the root node and its children to be displayed in this selectable tree-view. If a rootId is specified, this field will be ignored.
         */
        rootNode?: ISelectableTreeNode
        height?: string;
        width?: string;
        /**
         * Default is false.
         **/
        isMultiSelect?: boolean;
        submitFormTriggers?: ("double-click" | "enter")[]
        selectableTypes: ISelectableTypeCriteria[];
        resultFilter?: ((element: MacroApi.Context.IElementReadOnlyApi) => boolean)
    }

    export interface ISelectableTreeNode {
        /**
         * The specialization identifier of this node. This field is required.
         */
        specializationId: string;
        /**
         * The unique identifier of this node.
         */
        id: string;
        /**
         * The label text to display for this node.
         */
        label: string;
        /**
         * The child nodes of this node.
         */
        children?: ISelectableTreeNode[];
        /**
         * Whether this node is expanded by default or not. Default is true.
         */
        isExpanded?: boolean;
        /**
         * Whether this node is selected by default or not. Default is false.
         */
        isSelected?: boolean;
        /**
         * The icon of this node. A default icon will be used if this is not specified.
         */
        icon?: string | MacroApi.Context.IIcon;
    }

    interface ISelectableTypeCriteria {
        specializationId: string;
        isSelectable?: boolean | ((element: MacroApi.Context.IElementReadOnlyApi) => boolean);
        autoExpand?: boolean;
        autoSelectChildren?: boolean;
    }

    interface ITypeReferenceData {
        typeId: string;
        isNullable: boolean;
        isCollection: boolean;
        genericTypeId?: string;
        genericTypeParameters?: ITypeReferenceData[]
    }

    interface ITypeReferenceReadOnly {
        typeId: string;
        isTypeFound(): boolean;
        getTypeId(): string;
        getType(): IElementReadOnlyApi;
        getIsNullable(): boolean;
        getIsCollection(): boolean;
        getDisplayTextComponents(): IDisplayTextComponent[]
        isNavigable: boolean;
        isNullable: boolean;
        isCollection: boolean;
        display: string;
        toModel(): ITypeReferenceData;
    }

    interface ITypeReference extends ITypeReferenceReadOnly {
        getType(): IElementApi;
        setType(typeIdOrModel: string | ITypeReferenceData, genericTypeParameters?: ITypeReferenceData[]): void;
        setIsNullable(value: boolean): void;
        setIsCollection(value: boolean): void;
    }

    interface IElementToElementMappingApi {
        mappingType: string;
        mappingTypeId: string;
        getSourceElement(): IElementApi;
        getTargetElement(): IElementApi;
        addMappedEnd(mappingTypeNameOrId: string, sourcePath: string[], targetPath: string[]): void;
        addMappingExpression(targetPath: string[], mappingExpression: string): void;
        getMappedEnds(): IElementToElementMappedEndApi[];
        launchDialog(): Promise<void>;
    }

    interface IElementToElementMappedEndApi {
        /**
         * The name of the mapping type settings of the first {@link sources}, if one exists.
         */
        readonly mappingType: string;

        /**
         * The identifier of the mapping type settings of the first {@link sources}, if one exists.
         */
        readonly mappingTypeId: string;

        /**
         * The expression for this mapped end.
         */
        readonly mappingExpression: string;

        /**
         * The mapping target path.
         */
        readonly targetPath: IElementMappingPathApi[];

        /**
         * The ultimate element of the {@link targetPath}.
         */
        getTargetElement(): IElementApi;

        /**
         * The source mappings for this mapped end.
         */
        readonly sources: IElementToElementMappedEndSourceApi[];

        /**
         * Returns the {@link IElementToElementMappedEndSourceApi} for the provided {@link identifier}.
         * Will throw an exception if a source cannot be found.
         */
        getSource(identifier: string): IElementToElementMappedEndSourceApi;

        /**
         * If there is a single source, will return that source's SourcePath. If there are more 
         * than one source, will return the common SourcePath to all of them.
         */
        readonly sourcePath: IElementMappingPathApi[];

        /**
         * The ultimate element of the {@link sourcePath).
         */
        getSourceElement(): IElementApi;
    }

    interface IElementToElementMappedEndSourceApi {
        /**
         * The name of the mapping type settings for this source connection.
         */
        readonly mappingType: string;

        /**
         * The identifier of the mapping type settings for this source connection.
         */
        readonly mappingTypeId: string;

        /**
         * The string identifier used in the {@link IElementToElementMappedEndApi.mappingExpression}.
         */
        readonly expressionIdentifier: string;

        /**
         * The mapping target path.
         */
        readonly path: IElementMappingPathApi[];

        /**
         * The ultimate element of the {@link path}.
         */
        getElement(): IElementApi;
    }

    interface IElementMappingApi {
        applicationId: string;
        metadataId: string;
        mappingSettingsId: string;
        getPath(): IElementMappingPathApi[]
        getElement(): IElementApi;
    }

    interface IElementMappingPathApi {
        id: string;
        name: string;
        specialization: string;
        getElement(): IElementApi;
    }

    interface IStereotypeReadOnlyApi {
        definitionId: string;
        name: string;
        properties: object;
        hasProperty(property: string): boolean;
        getProperty(property: string): IStereotypePropertyReadOnlyApi;
    }

    interface IStereotypeApi extends IStereotypeReadOnlyApi {
        lookup(id: string): IElementApi;
        getProperty(property: string): IStereotypePropertyApi;
        element: IElementApi;
    }

    interface IStereotypePropertyReadOnlyApi {
        value: string;
        getValue(): string | boolean | number | IElementReadOnlyApi | IAssociationReadOnlyApi;
        getSelected(): IElementReadOnlyApi;
    }

    interface IStereotypePropertyApi extends IStereotypePropertyReadOnlyApi {
        value: string;
        getValue(): string | boolean | number | IElementApi | IAssociationApi;
        setValue(value: any): void;
        getSelected(): IElementApi;
        focus(): void;
    }

    interface IPackageReadOnlyApi {
        id: string;
        specialization: string;
        name: string;
        /**
         * Returns true if the package implements a trait identified by the provided traitId
         */
        hasTrait(traitId: string): boolean;
        getName(): string;
        getChildren(ofType: string): IElementReadOnlyApi[]
        hasStereotype(nameOrDefinitionId: string): boolean;
        getStereotypes(): IStereotypeReadOnlyApi[];
        getStereotype(nameOrDefinitionId: string): IStereotypeReadOnlyApi;
        getMetadata(key: string): string;
        hasMetadata(key: string): boolean;
        getParent(): IPackageReadOnlyApi;
        getPackage(): IPackageReadOnlyApi;
        /**
         * Expands this package in the designer model.
         */
        expand(): void;
        /**
         * Collapses this package in the designer model.
         */
        collapse(): void;
    }

    interface IPackageApi extends IPackageReadOnlyApi {
        setName(value: string): void;
        getChildren(ofType: string): IElementApi[]
        hasStereotype(nameOrDefinitionId: string): boolean;
        getStereotypes(): IStereotypeApi[];
        getStereotype(nameOrDefinitionId: string): IStereotypeApi;
        addStereotype(stereotypeDefinitionId: string): IStereotypeApi;
        removeStereotype(nameOrDefinitionId: string): void;
        getMetadata(key: string): string;
        hasMetadata(key: string): boolean;
        addMetadata(key: string, value: string): void;
        setMetadata(key: string, value: string): void;
        removeMetadata(key: string): void;
        getPackage(): IPackageApi;
        /**
         * Expands this package in the designer model.
         */
        expand(): void;
        /**
         * Collapses this package in the designer model.
         */
        collapse(): void;
    }

    interface IDiagramApi {
        /**
         * Return the element that owns this diagram
         */
        getOwner(): IElementApi;

        /**
         * The mouse position of the last user activated event.
         */
        mousePosition: IPoint;

        /**
         * Returns true if a visual with the specified element identifier is in the diagram.
         */
        isVisual: (elementId: string | any) => boolean;
        /**
         * Returns the visual API for the visual with the specified element identifier in the diagram.
         */
        getVisual: (elementId: string | any) => IElementVisualApi;

        /**
         * Automatically lays out the specified elements and associations using the Dagre algorithm around the provided position.
         */
        layoutVisuals: (elementIds: string | string[] | any, position?: IPoint, includeAllChildren?: boolean) => IElementVisualApi[];

        /**
         * Creates an element and adds it to the diagram at the specified position. The parent of the element is determined by the diagram.
         * If no parent can be found, an exception will be thrown.
         *
         * @param specialization The specialization id or name for the element to be created.
         * @param name The name of the element.
         * @param position The position to place the element visual. Will use the last mouse position if the position argument isn't provided.
         */
        createElement: (specialization: string, name: string, position?: IPoint) => IElementApi;

        /**
         * Adds an element visual to the diagram.
         *
         * @param elementId The element's id.
         * @param position The position to place the element visual.
         * @param size The size of the element visual.
         */
        addElement: (elementId: string | any, position: IPoint, size?: { width: number, height: number }) => void;

        /**
         * Adds an association visual to the diagram.
         *
         * @param associationId The association's id.
         * @param targetPrefPoint (optional) The relative point within the target element's visual to align the association.
         * @param fixedPoints (optional) The absolute fixed points that the association must follow.
         */
        addAssociation: (associationId: string | any, targetPrefPoint?: IPoint, fixedPoints?: IPoint[]) => void;

        /**
         * Hides the visual with the specified visual identifier.
         */
        hideVisual: (visualId: string | any) => void;

        /**
         * Finds the nearest empty space, searching vertically up and then down, incrementing by the specified increment (defaults to 100 if not specified).
         * Uses the specified size (defaults to 200 x 100 if not specified) to make sure that the space is empty. The specified point is treated as the center of the size argument.
         */
        findEmptySpace: (point: IPoint, size?: ISize, increment?: number) => IPoint;

        /**
         * Will select and center the diagram on the visuals for the provided visualIds.
         * @param visualIds
         */
        selectVisuals: (visualIds: string | string[]) => void;

        /**
         * Will select and center the diagram on the visuals for the provided elements with elementIds.
         * @param elementIds
         */
        selectVisualsForElements: (elementIds: string | string[]) => void;

        /**
         * Returns the dimensions of the diagram's view port
         */
        getViewPort: () => MacroApi.Context.IDimensions;
    }

    interface IElementVisualApi {
        /**
         * Identifier of the visual 
         */
        id: string;
        getPosition(): IPoint;
        getSize(): ISize;
        getDimensions(): MacroApi.Context.IDimensions;
        isAutoResizeEnabled(): boolean;
    }

    interface IDisplayTextComponent {
        text: string;
        cssClass?: string;
        color?: string;
        targetId?: string
    }

    interface IDimensions {
        left: number;
        right: number;
        top: number;
        bottom: number;
        getTopLeft(): IPoint;
        getTopRight(): IPoint;
        getBottomLeft(): IPoint;
        getBottomRight(): IPoint;
        getCenterTop(): IPoint;
        getCenterBottom(): IPoint;
        getCenterLeft(): IPoint;
        getCenterRight(): IPoint;
        getCenter(): IPoint;
    }

    interface IPoint {
        x: number;
        y: number;
    }

    interface ISize {
        width: number;
        height: number;
    }

    interface IIcon {
        type: string;
        source: string;
    }


    interface IBackwardCompatibleElementApi extends IElementApi {
        name: string;
        comment: string;
        value: string;
        genericTypes: string;
        children(type: string): IBackwardCompatibleElementApi[];
        /**
         * Obsolete - these are globally available methods
         */
        lookup(id: string): IElementApi;
        /**
         * Obsolete - these are globally available methods
         */
        lookupTypesOf(type: string): IElementApi[];
        /**
         * Obsolete - use refresh()
         */
        notifyChanged(): void;
    }

    interface IBackwardCompatibleElementReadOnlyApi extends IElementReadOnlyApi {
        name: string;
        genericTypes: string;
    }

    interface IElementReadOnlyApi {
        /**
         * Returns the display text components of the element as an array.
         */
        getDisplayTextComponents(): MacroApi.Context.IDisplayTextComponent[];
        /**
         * Returns the display name of the element.
         */
        getDisplay(): string;
        /**
         * The human-readable specialization type (e.g. "Class", "Attribute", etc.)
         */
        specialization: string;
        /**
         * The specialization type identifier. This is a more robust way to check the type of the element.
         */
        specializationId: string;
        /**
         * The unique identifier for the element.
         */
        id: string;
        /**
         * Returns true if the element implements a trait identified by the provided traitId
         */
        hasTrait(traitId: string): boolean;
        /**
         * Returns the name of the element.
         */
        getName(): string;
        /**
         * Returns the comment of the element.
         */
        getComment(): string;
        /**
         * Returns the value of the element.
         */
        getValue(): string;
        /**
         * Returns icon of the element
         */
        getIcon(): IIcon;
        /**
         * Returns true if the element has been indicated 'Is Abstract'.
         */
        getIsAbstract(): boolean;
        /**
         * Returns true if the element has been indicated 'Is Static'.
         */
        getIsStatic(): boolean;
        /**
         * Returns the external reference value of the element, if it exists.
         */
        getExternalReference(): string;
        /**
         * Returns the display text of the generic types associated with this element.
         */
        getGenericTypesDisplay(): string;
        /**
         * Returns the order of this element in relation to it's adjacent elements under the same parent.
         */
        getOrder(): number;
        /**
         * Returns true if the element is configured to have a typeReference.
         */
        hasType: boolean;
        /**
         * The typeReference property of the element. Will return null if no typeReference is configured for this element.
         */
        typeReference?: ITypeReference;
        /**
         * Returns all the child elements of this element. If a type argument is provided, the children will
         * be filtered to those that match on specialization. 
         */
        getChildren(type?: string | string[]): IElementReadOnlyApi[];
        /**
         * Finds and returns the first child element that matches the provided function. The function will search all
         * children in the hierarchy if the provided searchHierarchy parameter is true.
         */
        getChild(matchFunction: ((child: MacroApi.Context.IElementReadOnlyApi) => boolean), searchHierarchy?: boolean): IElementApi;
        /**
         * Returns this element's parent
         */
        getParent(type?: string): IElementReadOnlyApi;
        /**
         * Returns all parents for this element, ordered from the root to the immediate parent.
         **/
        getParents(type?: string | ((element: MacroApi.Context.IElementReadOnlyApi) => boolean)): IElementReadOnlyApi[];
        /**
         * Returns parents for this element from the matched root and includes this element itself, ordered from the root to the element.
         **/
        getPath(matchRoot?: (element: MacroApi.Context.IElementReadOnlyApi) => boolean): MacroApi.Context.IElementReadOnlyApi[];
        /**
         * Returns the owning package for this element.
         */
        getPackage(): IPackageReadOnlyApi;
        /**
         * Returns true if this element is mapped.
         */
        isMapped(): boolean;
        /**
         * Returns the mapping model for this element.
         */
        getMapping(): IElementMappingApi;
        /**
         * Returns true if a stereotype that matches the specified name or definition identifier is applied to the element.
         */
        hasStereotype(nameOrDefinitionId: string): boolean;
        /**
         * Returns all stereotypes currently applied to the element.
         */
        getStereotypes(): IStereotypeReadOnlyApi[];
        /**
         * Returns the stereotype that matches the specified name or definition identifier
         */
        getStereotype(nameOrDefinitionId: string): IStereotypeReadOnlyApi;
        /**
         * Returns all the association connected to this element. If a type argument is provided, the associations will
         * be filtered to those that match on specialization. 
         */
        getAssociations(type?: string | string[]): IAssociationReadOnlyApi[];
        /**
         * Returns true if the element is exposed from a package reference and not part of the current package. This will change for the same element depending on context in which this function is called
         */
        isReference(): boolean;
        /**
         * Gets the metadata value for the specified key.
         */
        getMetadata(key: string): string;
        /**
         * Returns true if a metadata value exists for the specified key.
         */
        hasMetadata(key: string): boolean;
    }

    interface IElementApi extends IElementReadOnlyApi {

        /**
         * Sets the name of the element.
         */
        setName(value: string, ensureUnique: boolean): void;
        /**
         * Sets the comment of the element.
         */
        setComment(value: string): void;
        /**
         * Sets the name of the element.
         */
        setValue(value: string): void;
        /**
         * Sets the 'Is Abstract' status of the element.
         */
        setIsAbstract(value: boolean): void;
        /**
         * Sets the 'Is Static' status of the element.
         */
        setIsStatic(value: boolean): void;
        /**
         * Sets the external reference of the element.
         */
        setExternalReference(value: string): void;
        /**
         * Opens the diagram that this element represents, if it configured to support a diagram.
         */
        loadDiagram(): void;
        /**
         * Sets this element's parent.
         */
        setParent(parentId: string): void;
        /**
         * Returns the owning package for this element.
         */
        getPackage(): IPackageApi;
        /**
         * Clears the mapping model of this element.
         */
        clearMapping(): void;
        /**
         * Returns all the child elements of this element. If a type argument is provided, the children will
         * be filtered to those that match on specialization. 
         */
        getChildren(type?: string | string[]): IElementApi[];
        /**
         * Returns this element's parent
         */
        getParent(type?: string): IElementApi;
        /**
         * Sets the mapping for this element.
         * @param elementId The unique identifier of the target element. If the mapping traverses more than one element in a hierarchy,
         * an array of element identifiers must be provided.
         * @param mappingSettingsId The specific mapping settings to use. The first mapping settings available will be used if no
         * value is provided.
         */
        setMapping(elementId: string | string[], mappingSettingsId?: string): void;
        /**
         * Launches the mapping dialog for the element for the provided mappingSettingsId.
         *
         * Must be called with await.
         * @param mappingSettingsId The specific mapping settings to use. The first mapping settings available will be used if no
         * value is provided.
         */
        launchMappingDialog(mappingSettingsId?: string): Promise<void>
        /**
         * Launches the advanced mapping dialog for the specified mappingSettingsId.
         * 
         * If the mappingTypeNameOrId is not provided and there is only one mapping type configured, then that mapping type will
         * be used. Otherwise, an error will be thrown.
         */
        launchAdvancedMappingDialog(mappingSettingsId?: string): Promise<void>
        /**
         * Returns the mapping model for the supplied mapping type name or id. Returns null if the mapping does not exist.
         * 
         * If the mappingTypeNameOrId is not provided and there is only one mapping type configured, then that mapping type will
         * be used. Otherwise, an error will be thrown.
         */
        getAdvancedMapping(mappingTypeNameOrId?: string): MacroApi.Context.IElementToElementMappingApi;
        /**
         * Creates a new element-to-element mapping and returns its API.
         * 
         * If the mappingTypeNameOrId is not provided and there is only one mapping type configured, then that mapping type will
         * be used. Otherwise, an error will be thrown.
         */
        createAdvancedMapping(mappingTypeNameOrId?: string): MacroApi.Context.IElementToElementMappingApi
        /**
         * Applies the stereotype with definition id of stereotypeDefinitionId to this element.
         */
        addStereotype(stereotypeDefinitionId: string): IStereotypeApi;
        /**
         * Removes the stereotype that matches the specified name or definition identifier from this element.
         */
        removeStereotype(nameOrDefinitionId: string): void;
        /**
         * Expands this element in the designer model.
         */
        expand(): void;
        /**
         * Collapses this element in the designer model.
         */
        collapse(): void;
        /**
         * Activates the editing mode for this element.
         */
        enableEditing(): Promise<void>;
        /**
         * Deletes this element.
         */
        delete(): void;
        /**
         * Sets the order index of this element within it's parent.
         */
        setOrder(index: number): void;
        /**
         * Refresh of display text and errors. 
         * This can be useful when you want to force a refresh of the elements state within the designer.
         */
        refresh(): void;
        /**
         * Add the metadata value for the specified key. Throws an error if metadata already exists for the specified key.
         */
        addMetadata(key: string, value: string): void;
        /**
         * Sets the metadata value for the specified key. Adds the metadata if it does not exist.
         */
        setMetadata(key: string, value: string): void;
        /**
         * Removes the metadata value for the specified key.
         */
        removeMetadata(key: string): void;
        /**
         * Returns all stereotypes currently applied to the element.
         */
        getStereotypes(): IStereotypeApi[];
        /**
         * Returns the stereotype that matches the specified name or definition identifier
         */
        getStereotype(nameOrDefinitionId: string): IStereotypeApi;
    }

    interface IMappableElementApi extends IElementApi {
        getMappedToElements(): MacroApi.Context.IMappableElementApi[];
    }

    interface IBackwardCompatibleIAssociationReadOnlyApi extends IAssociationReadOnlyApi {
        name: string;
    }

    interface IBackwardCompatibleIAssociationApi extends IAssociationApi {
        name: string;
        /**
         * DEPRECATED: Use getAdvancedMapping
         */
        getMapping(mappingTypeNameOrId?: string): IElementToElementMappingApi;
        /**
         * DEPRECATED: Use getAdvancedMappings
         */
        getMappings(): IElementToElementMappingApi[];
        /**
         * DEPRECATED: Use createAdvancedMapping
         */
        createMapping(sourceId?: string, targetId?: string, mappingTypeId?: string): IElementToElementMappingApi;
    }

    interface IAssociationReadOnlyApi {
        /**
         * The human-readable specialization type (e.g. "Class", "Attribute", etc.)
         */
        specialization: string;
        /**
         * The specialization type identifier. This is a more robust way to check the type of the element.
         */
        specializationId: string;
        /**
         * The unique identifier for the element.
         */
        id: string;
        /**
         * Returns true if the element implements a trait identified by the provided traitId
         */
        hasTrait(traitId: string): boolean;
        /**
         * Returns the name of the element.
         */
        getName(): string;
        /**
         * Returns the value of the element.
         */
        getValue(): string;
        /**
         * The typeReference property of the element
         */
        typeReference: ITypeReference;
        /**
         * Returns true if this association end is the source-end of the association.
         */
        isSourceEnd(): boolean;
        /**
         * Returns true if this association end is the target-end of the association.
         */
        isTargetEnd(): boolean;
        /**
         * Returns the order of this association in relation to it's adjacent elements under the same parent.
         */
        getOrder(): number;
        /**
         * Returns the other-end of the association.
         */
        getOtherEnd(): IAssociationReadOnlyApi;
        /**
         * Returns this association's parent
         */
        getParent(type?: string): IElementReadOnlyApi;
        /**
         * Returns the owning package for this element.
         */
        getPackage(): IPackageReadOnlyApi;
        /**
         * Returns all stereotypes currently applied to the element.
         */
        hasStereotype(nameOrDefinitionId: string): boolean;
        /**
         * Returns all stereotypes currently applied to the element.
         */
        getStereotypes(): IStereotypeReadOnlyApi[];
        /**
         * Returns the stereotype that matches the specified name or definition identifier
         */
        getStereotype(nameOrDefinitionId: string): IStereotypeReadOnlyApi;
        /**
         * Finds and returns the first child element that matches the provided function. The function will search all
         * children in the hierarchy if the provided searchHierarchy parameter is true.
         */
        getChild(matchFunction: ((child: MacroApi.Context.IElementReadOnlyApi) => boolean), searchHierarchy?: boolean): IElementApi;
        /**
         * Returns all the child elements of this element. If a type argument is provided, the children will
         * be filtered to those that match on specialization. 
         */
        getChildren(type: string): IElementApi[];
        /**
         * Returns true if there are mappings associated with this association end.
         */
        hasMappings(mappingTypeNameOrId?: string): boolean;
        /**
         * Gets the metadata value for the specified key.
         */
        getMetadata(key: string): string;
        /**
         * Returns true if a metadata value exists for the specified key.
         */
        hasMetadata(key: string): boolean;
    }

    interface IAssociationApi extends IAssociationReadOnlyApi {
        /**
         * Sets the name of the element.
         */
        setName(value: string): void;
        /**
         * Sets the value of the element.
         */
        setValue(value: string): void;
        /**
         * Returns the other-end of the association.
         */
        getOtherEnd(): IAssociationApi;
        /**
         * Returns this element's parent
         */
        getParent(type?: string): IElementApi;
        /**
         * Returns the owning package for this element.
         */
        getPackage(): IPackageApi;
        /**
         * Returns all stereotypes currently applied to the element.
         */
        getStereotypes(): IStereotypeApi[];
        /**
         * Returns the stereotype that matches the specified name or definition identifier
         */
        getStereotype(nameOrDefinitionId: string): IStereotypeApi;
        /**
         * Returns all the child elements of this element. If a type argument is provided, the children will
         * be filtered to those that match on specialization. 
         */
        getChildren(type: string): IElementApi[];
        /**
         * Returns true if there are mappings associated with this association end.
         */
        hasMappings(mappingTypeNameOrId?: string): boolean;
        /**
         * Launches the advanced mapping dialog for the specified mappingSettingsId.
         * 
         * If the mappingTypeNameOrId is not provided and there is only one mapping type configured, then that mapping type will
         * be used. Otherwise, an error will be thrown.
         */
        launchAdvancedMappingDialog(mappingSettingsId?: string): Promise<void>
        /**
         * Returns the mapping model for the supplied mapping type name or id. Returns null if the mapping does not exist.
         *          
         * If the mappingTypeNameOrId is not provided and there is only one mapping type configured, then that mapping type will
         * be used. Otherwise, an error will be thrown.
         */
        getAdvancedMapping(mappingTypeNameOrId?: string): IElementToElementMappingApi;
        getAdvancedMappings(): IElementToElementMappingApi[];
        /**
         * Creates a new element-to-element mapping and returns its API. If the sourceId and/or targetId are not provided,
         * then the mapping will by default be between the sourceEnd and targetEnd elements of this association. 
         * 
         * If the mappingTypeId is not provided and there is only one mapping type configured, then that mapping type will
         * be used. Otherwise, an error will be thrown.
         */
        createAdvancedMapping(sourceId?: string, targetId?: string, mappingTypeId?: string): IElementToElementMappingApi;
        /**
         * Activates the editing mode for this association end.
         */
        enableEditing(): Promise<void>;
        /**
         * Gets the metadata value for the specified key.
         */
        getMetadata(key: string): string;
        /**
         * Returns true if a metadata value exists for the specified key.
         */
        hasMetadata(key: string): boolean;
        /**
         * Add the metadata value for the specified key. Throws an error if metadata already exists for the specified key.
         */
        addMetadata(key: string, value: string): void;
        /**
         * Sets the metadata value for the specified key. Adds the metadata if it does not exist.
         */
        setMetadata(key: string, value: string): void;
        /**
         * Removes the metadata value for the specified key.
         */
        removeMetadata(key: string): void;
        /**
         * Deletes this association.
         */
        delete(): void;
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
        installedModules: IModuleIdentifier[];
        isModuleInstalled(moduleId: string): boolean;
    }

    interface IModuleIdentifier {
        id: string,
        version: string
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
