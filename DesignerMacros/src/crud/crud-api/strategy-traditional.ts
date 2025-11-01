/// <reference path="strategy-base.ts" />

class TraditionalServicesStrategy extends CrudStrategy {
    private service: IElementApi;

    protected initialize(context: ICrudCreationContext): void {
        const intentPackage = context.element.getPackage();
        let entity = context.dialogOptions.selectedEntity;
        const owningAggregate = DomainHelper.getOwningAggregateRecursive(this.entity);

        const serviceName = `${toPascalCase(pluralize(owningAggregate != null ? owningAggregate.getName() : entity.getName()))}Service`;
        const existingService = context.element.specialization == "Service" ? context.element : intentPackage.getChildren("Service").find(x => x.getName() == serviceName);
        this.service = existingService ?? createElement("Service", serviceName, intentPackage.id);
    }

    protected doCreate(): IElementApi {     

        let operationName = `Create${this.entity.getName()}`;

        const existing = this.operationExists(operationName);
        if (existing) {
            existing.typeReference.setType(this.primaryKeys[0].typeId);
            return existing;
        }

        let operationManager = new ElementManager(createElement("Operation", operationName, this.service.id), {
            childSpecialization: "Parameter",
            childType: "parameter"
        });        

        let projector = new EntityProjector();
        projector.createOrGetOperationDto(operationManager, this.entity, this.targetFolder, true, true, true);        

        if (this.primaryKeys.length == 1) {
            operationManager.setReturnType(this.primaryKeys[0].typeId);
        }

        this.doAdvancedMappingCreate(projector, operationManager.getElement());

        return operationManager.getElement();
    }

    protected doUpdate(): IElementApi {   

        let operationName = `Update${this.entity.getName()}`;

        const existing = this.operationExists(operationName);
        if (existing) return existing;

        let operationManager = new ElementManager(createElement("Operation", operationName, this.service.id), {
            childSpecialization: "Parameter",
            childType: "parameter"
        });

        let operation = operationManager.getElement();

        for (const pk of Object.values(this.primaryKeys)) {
            var param = createElement("Parameter", pk.name, operation.id);
            param.typeReference.setType(pk.typeId);
        }     

        let projector = new EntityProjector();
        projector.createOrGetOperationDto(operationManager, this.entity, this.targetFolder, true, true, true);        

        this.doAdvancedMappingUpdate(projector, operationManager.getElement());
        
        return operationManager.getElement();
    }

    protected doDelete(): IElementApi {       
        let operationName = `Delete${this.entity.getName()}`;

        const existing = this.operationExists(operationName);
        if (existing) return existing;

        let operation = createElement("Operation", operationName, this.service.id);

        let mappings = this.addPkParameters(operation);

        this.doAdvancedMappingDelete(mappings, operation);

        return operation;
    }

    protected doGetById(): IElementApi {        
        let operationName = `Find${this.entity.getName()}ById`;

        const existing = this.operationExists(operationName);
        if (existing) return existing;

        let operation = createElement("Operation", operationName, this.service.id);

        let mappings = this.addPkParameters(operation);

        operation.typeReference.setType(this.resultDto.id);

        this.doAdvancedMappingGetById(mappings, operation);
        
        return operation;
    }

    protected doGetAll(): IElementApi {    
        let operationName = `Find${pluralize(this.entity.getName())}`;

        const existing = this.operationExists(operationName);
        if (existing) return existing;

        let operation = createElement("Operation", operationName, this.service.id);

        operation.typeReference.setType(this.resultDto.id);
        operation.typeReference.setIsCollection(true);

        this.doAdvancedMappingGetAll(operation);

        return operation;
    }

    protected doOperation(domainOperation: IElementApi, operationResultDto?: IElementApi): IElementApi {        
        let operationName = domainOperation.getName();

        const existing = this.operationExists(operationName);
        if (existing) return existing;

        let operationManager = new ElementManager(createElement("Operation", operationName, this.service.id), {
            childSpecialization: "Parameter",
            childType: "parameter"
        });        

        let operation = operationManager.getElement();
        for (const pk of Object.values(this.primaryKeys)) {
            var param = createElement("Parameter", pk.name, operation.id);
            param.typeReference.setType(pk.typeId);
        }

        let projector = new EntityProjector();
        projector.createOrGetOperationDto(operationManager, domainOperation, this.targetFolder, false, true);        

        if (operationResultDto) {
            operationManager.setReturnType(operationResultDto.id);
        }

        this.doAdvancedMappingCallOperation(projector, operationManager.getElement());

        return operation;
    }

    protected addPkParameters(operation: IElementApi): IPathMapping[] {
        let mappings : IPathMapping[] = [];
        for (const pk of Object.values(this.primaryKeys)) {
            var param = createElement("Parameter", pk.name, operation.id);
            param.typeReference.setType(pk.typeId);
            mappings.push({ type : MappingType.Navigation, sourcePath:  [param.id], targetPath : pk.mapPath, targetPropertyStart:pk.mapPath[0]});
        }  
        return mappings;      
    }

    protected operationExists(operationName: string): IElementApi | undefined {
        return this.service.getChildren().find(x => x.getName() === operationName);
    }

    protected doAddElementsToDiagram(diagram: IDiagramApi, addAtPoint?: MacroApi.Context.IPoint): void {        
        const space = diagram.findEmptySpace(addAtPoint ?? diagram.getViewPort().getCenter(), { width: 500, height: 200 });
        const visuals = diagram.layoutVisuals(this.service, space, true);
        diagram.selectVisualsForElements(visuals.map(x => x.id))
    }

    protected doAddElementToDiagram(element: IElementApi, diagram: IDiagramApi): void {
        this.doAddElementsToDiagram(diagram, diagram.mousePosition);
    }

    protected addMissingAggregateKey(element: IElementApi, name: string):IElementApi{
        return createElement("Parameter", toCamelCase(name), element.id);
    }
}