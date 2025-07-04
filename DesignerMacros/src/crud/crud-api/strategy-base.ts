/// <reference path="common.ts" />
/// <reference path="crud-dialog.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../../common/crudHelper.ts" />
/// <reference path="../../common/elementManager.ts" />
/// <reference path="project-from-entity.ts" />

abstract class CrudStrategy {

    protected resultDto: IElementApi = null;
    protected owningAggregate?: IElementApi = null;
    protected entity: IElementApi = null;
    protected context: ICrudCreationContext = null;
    protected targetFolder: IElementApi = null;
    protected primaryKeys: IAttributeWithMapPath[] = null;

    private async askUser(element: MacroApi.Context.IElementApi, preselectedClass: MacroApi.Context.IElementApi): Promise<ICrudCreationContext> {
        let dialogOptions: ICrudCreationResult = await presentCrudOptionsDialog(preselectedClass);
        if (dialogOptions == null || dialogOptions.selectedEntity == null)
            return null;

        const primaryKeys = DomainHelper.getPrimaryKeys(dialogOptions.selectedEntity);

        let context = new CrudCreationContext(
            element,
            dialogOptions,
            primaryKeys
        );
        return context;
    }

    private getOrCreateEntityFolder(folderOrPackage: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        if (folderOrPackage.specialization == "Folder") {
            return element;
        }

        const folderName = this.getAggregateRootFolderName(entity);
        const folder = element.getChildren().find(x => x.getName() == pluralize(folderName)) ?? createElement("Folder", pluralize(folderName), element.id);
        return folder;
    }

    private getAggregateRootFolderName(entity: MacroApi.Context.IElementApi) {
        return pluralize(this.owningAggregate != null ? this.owningAggregate.getName() : entity.getName());
    }


    public async execute(element: IElementApi, preselectedClass: IElementApi | undefined): Promise<void> {
        this.context = await this.askUser(element, preselectedClass);
        if (this.context == null) return;

        let dialogOptions = this.context.dialogOptions;
        this.entity = dialogOptions.selectedEntity;
        this.primaryKeys = this.context.primaryKeys;
        let hasPrimaryKey = this.context.hasPrimaryKey();

        if (!await this.validate()) {
            return;
        }

        this.owningAggregate = DomainHelper.getOwningAggregateRecursive(this.entity);
        this.targetFolder = this.getOrCreateEntityFolder(this.context.element, this.entity)

        this.initialize(this.context);

        if (dialogOptions.canQueryById || dialogOptions.canQueryAll) {
            let projector = new EntityProjector();
            this.resultDto = projector.createOrGetDto(this.entity, this.targetFolder);
            if (projector.getMappings().length > 0) {
                this.addBasicMapping(projector.getMappings());
            }
            this.resultDto.collapse();
        }

        if ((!privateSettersOnly || hasConstructor(this.entity)) && dialogOptions.canCreate) {
            let x = this.doCreate();
            if (this.owningAggregate != null) {
                this.AddAggregateKeys(x);
            }
            x.collapse();
        }

        if ((hasPrimaryKey && !privateSettersOnly) && dialogOptions.canUpdate) {
            let x = this.doUpdate();
            if (this.owningAggregate != null) {
                this.AddAggregateKeys(x);
            }
            x.collapse();
        }

        if (hasPrimaryKey && dialogOptions.canQueryById) {
            let x = this.doGetById();
            if (this.owningAggregate != null) {
                this.AddAggregateKeys(x);
            }
            x.collapse();
        }

        if (dialogOptions.canQueryAll) {
            let x = this.doGetAll();
            if (this.owningAggregate != null) {
                this.AddAggregateKeys(x);
            }
            x.collapse();
        }

        if (hasPrimaryKey && dialogOptions.canDelete) {
            let x = this.doDelete();
            if (this.owningAggregate != null) {
                this.AddAggregateKeys(x);
            }
            x.collapse();
        }

        if (dialogOptions.canDomain) {
            const operations = DomainHelper.getCommandOperations(this.entity);
            for (const operation of operations) {
                if (!this.context.dialogOptions.selectedDomainOperationIds.some(x => x == operation.id)) {
                    continue;
                }

                let operationResultDto = null;
                if (operation.typeReference != null) {
                    if (DomainHelper.isComplexType(operation.typeReference?.getType())) {
                        let projector2 = new EntityProjector();
                        let from = lookup(operation.typeReference.getTypeId());
                        operationResultDto = projector2.createOrGetDto(from, this.targetFolder);
                        if (projector2.getMappings().length > 0) {
                            this.addBasicMapping(projector2.getMappings());
                        }
                    }
                }

                let x = this.doOperation(operation, operationResultDto);
                if (this.owningAggregate != null) {
                    this.AddAggregateKeys(x);
                }
                x.collapse();
            }

        }

        this.doAddDiagram();

        await notifyUserOfLimitations(dialogOptions.selectedEntity, dialogOptions);
    }

    protected abstract initialize(context: ICrudCreationContext): void;
    protected abstract doCreate(): IElementApi;
    protected abstract doUpdate(): IElementApi;
    protected abstract doDelete(): IElementApi;
    protected abstract doGetById(): IElementApi;
    protected abstract doGetAll(): IElementApi;
    protected abstract doOperation(operation: IElementApi, operationResultDto?: IElementApi): IElementApi;
    protected abstract doAddDiagram(): void;

    protected async validate(): Promise<boolean> {
        if (DomainHelper.getOwnersRecursive(this.entity).length > 1) {
            const owners = DomainHelper.getOwnersRecursive(this.entity).map(item => item.getName()).join(", ");
            await dialogService.warn(
                `Entity has multiple owners.
The entity '${this.entity.getName()}' has multiple Aggregate owning it [${owners}].

Compositional Entities (black diamond) must have 1 owner. Please adjust the associations accordingly.`);
            return false;
        }
        return true;
    }

    protected doAdvancedMappingCreate(projector: EntityProjector, source: IElementApi) {
        if (projector.getMappings().length > 0) {
            let target = projector.getTarget();
            let action = createAssociation("Create Entity Action", source.id, target.id);
            let mapping = action.createAdvancedMapping(source.id, this.entity.id);
            mapping.addMappedEnd("Invocation Mapping", [source.id], [target.id]);
            this.addAdvancedMappingEnds("Data Mapping", source, mapping, projector.getMappings());
        }
    }

    protected doAdvancedMappingDelete(mappings: IPathMapping[], source: IElementApi) {
        if (mappings.length > 0) {
            let action = createAssociation("Delete Entity Action", source.id, this.entity.id);
            let mapping = action.createAdvancedMapping(source.id, this.entity.id);
            this.addAdvancedMappingEnds("Filter Mapping", source, mapping, mappings);
        }
    }

    protected doAdvancedMappingGetById(mappings: IPathMapping[], source: IElementApi) {
        if (mappings.length > 0) {
            let action = createAssociation("Query Entity Action", source.id, this.entity.id);
            let queryMapping = action.createAdvancedMapping(source.id, this.entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
            this.addAdvancedMappingEnds("Filter Mapping", source, queryMapping, mappings);
        }
    }

    protected doAdvancedMappingUpdate(projector: EntityProjector, source: IElementApi) {
        if (projector.getMappings().length > 0) {

            let action = createAssociation("Update Entity Action", source.id, this.entity.id);

            //remove PKs from Update
            let updateMappingEnds = projector.getMappings().filter(x => {
                const last = x.targetPath[x.targetPath.length - 1];
                return !this.primaryKeys.some(pk => pk.id == last)
            });

            let queryMappingEnds = this.createQueryMappingEnds(source);

            // Query Entity Mapping
            let queryMapping = action.createAdvancedMapping(source.id, this.entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
            this.addAdvancedMappingEnds("Filter Mapping", source, queryMapping, queryMappingEnds);
            // Update Entity Mapping
            let updateMapping = action.createAdvancedMapping(source.id, this.entity.id, "01721b1a-a85d-4320-a5cd-8bd39247196a");
            this.addAdvancedMappingEnds("Data Mapping", source, updateMapping, updateMappingEnds);
        }
    }

    protected doAdvancedMappingGetAll(source: IElementApi) {
        let action = createAssociation("Query Entity Action", source.id, this.entity.id);
        action.typeReference.setIsCollection(true);
        action.createAdvancedMapping(source.id, this.entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
    }

    protected doAdvancedMappingCallOperation(projector: EntityProjector, source: IElementApi) {
        if (projector.getMappings().length > 0) {

            let target = projector.getTarget();
            let action = createAssociation("Update Entity Action", source.id, this.entity.id);

            //remove PKs from Update
            let updateMappingEnds = projector.getMappings().filter(x => {
                const last = x.targetPath[x.targetPath.length - 1];
                return !this.primaryKeys.some(pk => pk.id == last)
            });

            let queryMappingEnds = this.createQueryMappingEnds(source);

            // Query Entity Mapping
            let queryMapping = action.createAdvancedMapping(source.id, this.entity.id, "25f25af9-c38b-4053-9474-b0fabe9d7ea7");
            this.addAdvancedMappingEnds("Filter Mapping", source, queryMapping, queryMappingEnds);
            // Update Entity Mapping
            let updateMapping = action.createAdvancedMapping(source.id, this.entity.id, "01721b1a-a85d-4320-a5cd-8bd39247196a");
            updateMapping.addMappedEnd("Invocation Mapping", [source.id], [target.id]);
            this.addAdvancedMappingEnds("Data Mapping", source, updateMapping, updateMappingEnds);
        }
    }

    protected createQueryMappingEnds(source: IElementApi): IPathMapping[] {
        let queryMappingEnds: IPathMapping[] = [];
        for (const pk of Object.values(this.primaryKeys)) {
            var dtoField = source.getChildren().find(x => x.getName() == pk.name);
            queryMappingEnds.push({ type: MappingType.Navigation, sourcePath: [dtoField.id], targetPath: pk.mapPath, targetPropertyStart: pk.mapPath[0] });
        }
        return queryMappingEnds;
    }

    protected addDiagram(whatToAdd: IElementApi, diagramFolder?: IElementApi) {
        if (diagramFolder == null) {
            diagramFolder = this.targetFolder;
        }
        let entity = this.entity;
        if (DomainHelper.isAggregateRoot(entity)) {
            const aggregateRootFolderName = this.getAggregateRootFolderName(entity);
            const diagramElement = diagramFolder.getChildren("Diagram").find(x => x.getName() == aggregateRootFolderName) ?? createElement("Diagram", aggregateRootFolderName, diagramFolder.id)
            diagramElement.loadDiagram();
            const diagram = getCurrentDiagram();
            diagram.layoutVisuals(whatToAdd, null, true);
        }
    }

    protected addBasicMapping(mappings: IPathMapping[]) {
        mappings.forEach(m => {

            let dtoPart = lookup(m.sourcePath.slice(-1)[0]);
            //Work around for SetMapping clearing type in some scenarios.
            let previousType = dtoPart.typeReference?.getTypeId();

            //Some property paths are multiple entries like "base.id"
            if (m.type == MappingType.Navigation) {
                //console.warn(m.type + ":" + m.sourcePath.map(x => lookup(x).getName()).join('.') + "->" + m.targetPath.map(x => lookup(x).getName()).join('.'));

                const index = m.targetPath.indexOf(m.targetPropertyStart);
                if (index === -1) return; // value not found

                dtoPart.setMapping(m.targetPath.slice(index));
            } else {
                dtoPart.setMapping(m.targetPath.slice(-1)[0]);
            }

            if (previousType != null) {
                dtoPart.typeReference.setType(previousType);
            }
        })
    }


    protected addAdvancedMappingEnds(mappingType: string, element: IElementApi, mapping: MacroApi.Context.IElementToElementMappingApi, mappings: IPathMapping[]) {

        mappings.forEach(m => {
            if (m.type == MappingType.Navigation) {
                //console.warn(m.type + ":" + m.sourcePath.map(x => lookup(x).getName()).join('.') + "->" + m.targetPath.map(x => lookup(x).getName()).join('.'));
                let dtoPart = lookup(m.sourcePath.slice(-1)[0]);
                let mappedElementId = m.targetPath.slice(-1)[0]
                let element = lookup(mappedElementId);
                if (element.specialization == "Class Constructor") {
                    mapping.addMappedEnd("Invocation Mapping", [element.id], m.targetPath);
                } else {
                    if ((dtoPart.typeReference?.getType()?.specialization != "DTO" || dtoPart.typeReference?.getIsCollection())) {
                        mapping.addMappedEnd(mappingType, m.sourcePath, m.targetPath);
                    }
                }
            }
        });
    }

    protected AddAggregateKeys(element: IElementApi): void {
        //Have to do the reverse so setOrder works
        let keys = DomainHelper.getOwningAggregateKeyChain(this.entity).reverse();
        keys.forEach((pk) => {

            if (!element.getChildren().some(x => x.getName().toLowerCase() == pk.expectedName.toLowerCase())) {
                let field = this.addMissingAggregateKey(element, pk.expectedName);
                field.typeReference.setType(pk.attribute.typeReference.getTypeId());
                field.setOrder(0);
            } else {
                let field = element.getChildren().find(x => x.getName().toLowerCase() == pk.expectedName.toLowerCase() );
                field.setOrder(0);
            }
        });
    }

    protected abstract addMissingAggregateKey(element: IElementApi, name: string): IElementApi;
}
