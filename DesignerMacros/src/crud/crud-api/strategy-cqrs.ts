/// <reference path="strategy-base.ts" />


class CQRSCrudStrategy extends CrudStrategy{

    protected initialize(context: ICrudCreationContext): void {
    }

    protected doCreate(): IElementApi {    
        let projector = new EntityProjector();
        let command = projector.createOrGetCreateCommand(this.entity, this.targetFolder);

        const surrogateKey = this.primaryKeys.length === 1;
        if (surrogateKey) {
            command.typeReference.setType(this.primaryKeys[0].typeId);
        }

        this.doAdvancedMappingCreate(projector, command);
        return command;
    }

    protected doUpdate(): IElementApi {        
        let projector = new EntityProjector();
        let command = projector.createOrGetUpdateCommand(this.entity, this.targetFolder);

        this.doAdvancedMappingUpdate(projector, command);
        return command;
    }

    protected doDelete(): IElementApi {        
        let projector = new EntityProjector();
        let command = projector.createOrGetDeleteCommand(this.entity, this.targetFolder);
        this.doAdvancedMappingDelete(projector.getMappings(), command);
        return command;
    }

    protected doGetById(): IElementApi {        
        let projector = new EntityProjector();
        let query = projector.createOrGetFindByIdQuery(this.entity, this.targetFolder, this.resultDto);
        this.doAdvancedMappingGetById(projector.getMappings(), query);
        return query;
    }

    protected doGetAll(): IElementApi {     
        let projector = new EntityProjector();
        let query = projector.createOrGetFindAllQuery(this.entity, this.targetFolder, this.resultDto);
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

    protected doAddDiagram(): void {        
        this.addDiagram(this.targetFolder);    
    }

    protected addMissingAggregateKey(element: IElementApi, name: string):IElementApi{
        return createElement("DTO-Field", name, element.id);
    }
}