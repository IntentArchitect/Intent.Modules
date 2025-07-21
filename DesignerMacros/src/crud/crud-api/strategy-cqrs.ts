/// <reference path="strategy-base.ts" />


class CQRSCrudStrategy extends CrudStrategy{

    protected initialize(context: ICrudCreationContext): void {
    }

    protected doCreate(): IElementApi {    
        const commandName = `Create${this.getElementBaseName(this.entity, false)}Command`;
        const existing = this.elementExists(commandName);
        if (existing) return existing;


        let projector = new EntityProjector();
        let command = projector.createCreateCommand(commandName, this.entity, this.targetFolder);

        const surrogateKey = this.primaryKeys.length === 1;
        if (surrogateKey) {
            command.typeReference.setType(this.primaryKeys[0].typeId);
        }

        this.doAdvancedMappingCreate(projector, command);
        return command;
    }

    protected doUpdate(): IElementApi {    
        const commandName = `Update${this.getElementBaseName(this.entity, false)}Command`;
        const existing = this.elementExists(commandName);
        if (existing) return existing;

        let projector = new EntityProjector();
        let command = projector.createUpdateCommand(commandName, this.entity, this.targetFolder);
        this.doAdvancedMappingUpdate(projector, command);
        return command;
    }


    
    protected doDelete(): IElementApi {        
        const commandName = `Delete${this.getElementBaseName(this.entity, false)}Command`;
        const existing = this.elementExists(commandName);
        if (existing) return existing;

        let projector = new EntityProjector();
        let command = projector.createDeleteCommand(commandName, this.entity, this.targetFolder);
        this.doAdvancedMappingDelete(projector.getMappings(), command);
        return command;
    }

    protected doGetById(): IElementApi {       
        const queryName = `Get${this.getElementBaseName(this.entity, false)}ByIdQuery`;
        const existing = this.elementExists(queryName);
        if (existing) return existing;

        let projector = new EntityProjector();
        let query = projector.createFindByIdQuery(queryName, this.entity, this.targetFolder, this.resultDto);
        this.doAdvancedMappingGetById(projector.getMappings(), query);
        return query;
    }

    protected doGetAll(): IElementApi {   
        const queryName = `Get${this.getElementBaseName(this.entity, true)}Query`;
        const existing = this.elementExists(queryName);
        if (existing) return existing;
  
        let projector = new EntityProjector();
        let query = projector.createFindAllQuery(queryName, this.entity, this.targetFolder, this.resultDto);
        this.doAdvancedMappingGetAll(query);
        return query;
    }

    protected doOperation(operation: IElementApi, operationResultDto?: IElementApi): IElementApi {   

        let projector = new EntityProjector();
        let command = projector.createOrGetCallOperation(operation, this.entity, this.targetFolder);

        if (operationResultDto) {
            command.typeReference.setType(operationResultDto.id);
        }

        this.doAdvancedMappingCallOperation(projector, command);
        return command;
    }

    private getElementBaseName( entity: MacroApi.Context.IElementApi, entityIsMany: boolean): string {
        let entityName = entityIsMany ? toPascalCase(pluralize(entity.getName())) : toPascalCase(entity.getName());
        return entityName;
    }
    protected elementExists(elementName: string): IElementApi | undefined {
        return this.targetFolder.getChildren().find(x => x.getName() == elementName)
    }

    protected doAddToDiagram(diagram: IDiagramApi, addAtPoint?: MacroApi.Context.IPoint): void {
        const space = diagram.findEmptySpace(addAtPoint ?? diagram.getViewPort().getCenter(), { width: 500, height: 550 });
        const visuals = diagram.layoutVisuals(this.targetFolder, space, true);
        diagram.selectVisualsForElements(visuals.map(x => x.id))
    }

    protected addMissingAggregateKey(element: IElementApi, name: string):IElementApi{
        return createElement("DTO-Field", name, element.id);
    }
}