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

        // Add Aggregate PK to command for Query
        let keys = this.primaryKeys.reverse();
        keys.forEach((pk) => {

            if (!command.getChildren().some(x => x.getName().toLowerCase() == pk.name.toLowerCase())) {
                let field = createElement("DTO-Field", pk.name, command.id);                
                field.typeReference.setType(pk.typeId);
                field.setOrder(0);
            } else {
                let field = command.getChildren().find(x => x.getName().toLowerCase() == pk.name.toLowerCase() );
                field.setOrder(0);
            }
        });

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

    protected doAddElementsToDiagram(diagram: IDiagramApi, addAtPoint?: MacroApi.Context.IPoint): void {
        const space = diagram.findEmptySpace(addAtPoint ?? diagram.getViewPort().getCenter(), { width: 500, height: 550 });
        const visuals = diagram.layoutVisuals(this.targetFolder, space, true);
        diagram.selectVisualsForElements(visuals.map(x => x.id))
    }

    protected doAddElementToDiagram(element: IElementApi, diagram: IDiagramApi): void {
        const existingElements = this.entity.getAssociations()
            .concat(this.entity.getChildren("Operation").map(x => x.getAssociations()).flat())
            .map(x => x.typeReference?.getType())
            .filter(x => x != null && diagram.getVisual(x) != null && (x.specialization === "Command" || x.specialization === "Query"));
        if (existingElements.length > 0) {
            const lowestPlacedElement = existingElements.reduce((lowest, current) => {
                const lowestVisual = diagram.getVisual(lowest.id);
                const currentVisual = diagram.getVisual(current.id);
                return (currentVisual.getPosition().y > lowestVisual.getPosition().y) ? current : lowest;
            });
            const lastVisual = diagram.getVisual(lowestPlacedElement.id);
            let space = lastVisual.getPosition();
            space.y += lastVisual.getSize().height + 100;
            space.x += 100; // Not sure why but its X auto-moved to the left. Let's move it to the right.
            diagram.layoutVisuals([element.id], space);
        }
    }

    protected addMissingAggregateKey(element: IElementApi, name: string):IElementApi{
        return createElement("DTO-Field", name, element.id);
    }
}