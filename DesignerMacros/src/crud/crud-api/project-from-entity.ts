/// <reference path="common.ts" />

interface IPathMapping {
    type:MappingType,
    targetPropertyStart: string;
    sourcePath: string[],
    targetPath: string[]
}

enum MappingType{
    Navigation = 1,
    TypeMap = 2
}

interface IPops{
    targetPops : number;
    sourcePops : number;
}

class EntityProjector {

    private target: MacroApi.Context.IElementApi;
    private mappings: IPathMapping[] = [];
    private sourcePath: string[] = []; //Dto
    private targetPath: string[] = []; //Entity
    private isChild = false;
    private addMandatoryRelationships = false;

    private addMapping(type: MappingType, sourceRelative: string[], targetRelative: string[], changeCurrent:boolean  = false) : IPops  {        
        let result : IPops = {targetPops : 0, sourcePops : 0};
        this.mappings.push({ type: type, sourcePath: this.sourcePath.concat(sourceRelative), targetPath: this.targetPath.concat(targetRelative), targetPropertyStart:targetRelative[0] });
        if (type == MappingType.Navigation && changeCurrent){            
            this.sourcePath.push(...sourceRelative);
            this.targetPath.push(...targetRelative);
            result.sourcePops = sourceRelative.length;
            result.targetPops = targetRelative.length;
        }
        return result;
    }

    private popMapping(pops: IPops){
        for (let i = 0; i < pops.sourcePops; i++) {
            this.sourcePath.pop();
        }        
        for (let i = 0; i < pops.targetPops; i++) {
            this.targetPath.pop();
        }        
    }

    constructor() {
    }

    public getMappings(): IPathMapping[] {
        return this.mappings;
    }

    public getTarget(): IElementApi {
        return this.target;
    }


    private getEntityConstructor(entity: MacroApi.Context.IElementApi):MacroApi.Context.IElementApi{
        return entity
            .getChildren("Class Constructor")
            .sort((a, b) => {
                // In descending order:
                return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
            })[0];            
    }

    public createOrGetCreateCommand(entity: IElementApi, folder: IElementApi): IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = this.getBaseNameForElement(owningAggregate, entity, false);
        const commandName = `Create${baseName}Command`;
        let existing = folder.getChildren().find(x => x.getName() == commandName)
        if (existing) {
            return existing;
        }

        this.addMandatoryRelationships = true;

        let dto = this.getOrCreateElement(commandName, "Command", folder);
        this.sourcePath.push(dto.id);
        this.targetPath.push(entity.id);

        const entityCtor: MacroApi.Context.IElementApi = this.getEntityConstructor(entity);
        if (entityCtor != null){
            entity = entityCtor;
            this.targetPath.push(entityCtor.id);
        }

        //Not this is the entity or the ctor depending
        this.target = entity;

        let result = this.populateContract(dto, entity, true, folder);
        
        return result;
    }

    public createOrGetCallOperation(operation: IElementApi, entity: IElementApi, folder: IElementApi) : IElementApi {
        let operationName = operation.getName();
        operationName = removeSuffix(operationName, "Async");
        operationName = toPascalCase(operationName);

        const commandName = `${operationName}${entity.getName()}Command`;
    
        const existing = folder.getChildren().find(x => x.getName() == commandName );
        if (existing) {
            return existing;
        }


        let dto = this.getOrCreateElement(commandName, "Command", folder);
        this.sourcePath.push(dto.id);
        this.targetPath.push(entity.id);
        this.targetPath.push(operation.id);

        //Not this is the entity or the ctor depending
        this.target = operation;

        let result = this.populateContract(dto, operation, false, folder);
        
        return result;

    }

    public createOrGetUpdateCommand(entity: IElementApi, folder: IElementApi): IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = this.getBaseNameForElement(owningAggregate, entity, false);
        const commandName = `Update${baseName}Command`;
        let existing = folder.getChildren().find(x => x.getName() == commandName)
        if (existing) {
            return existing;
        }

        let dto = this.getOrCreateElement(commandName, "Command", folder);
        this.addMandatoryRelationships = true;

        this.sourcePath.push(dto.id);
        this.targetPath.push(entity.id);

        this.target = entity;

        let result = this.populateContract(dto, entity, false, folder);

        return result;
    }


    public createOrGetDeleteCommand(entity: IElementApi, folder: IElementApi): IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = this.getBaseNameForElement(owningAggregate, entity, false);
        const commandName = `Delete${baseName}Command`;

        let existing = folder.getChildren().find(x => x.getName() == commandName)
        if (existing) {
            return existing;
        }

        let dto = this.getOrCreateElement(commandName, "Command", folder);
        
        let attributes =  this.getAttributesWithMapPath(entity, (x) => x.hasStereotype("Primary Key")); 
        this.addDtoFieldsInternal(attributes, false, entity, dto, folder, true);
        dto.collapse();
        return dto;
    }

    public createOrGetFindByIdQuery(entity: IElementApi, folder: IElementApi, resultDto: IElementApi): IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = this.getBaseNameForElement(owningAggregate, entity, false);
        let expectedQueryName = `Get${baseName}ByIdQuery`;

        let existing = folder.getChildren().find(x => x.getName() == expectedQueryName)
        if (existing) {
            return existing;
        }

        let query = createElement("Query", expectedQueryName, folder.id);
        query.typeReference.setType(resultDto.id)

        let attributes =  this.getAttributesWithMapPath(entity, (x) => x.hasStereotype("Primary Key")); 
        this.addDtoFieldsInternal(attributes, false, entity, query, folder, true);

        query.collapse();
        return query;
    }


    public createOrGetFindAllQuery(entity: IElementApi, folder: IElementApi, resultDto: IElementApi): IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = this.getBaseNameForElement(owningAggregate, entity, true);
        let expectedQueryName = `Get${baseName}Query`;

        let existing = folder.getChildren().find(x => x.getName() == expectedQueryName)
        if (existing) {
            return existing;
        }

        let query = createElement("Query", expectedQueryName, folder.id);
        query.typeReference.setType(resultDto.id)
        query.typeReference.setIsCollection(true);

        query.collapse();
        return query;
    }
    
    public createOrGetOperationDto(operationManager:ElementManager, entity: IElementApi, folder: IElementApi, createMode: boolean, inbound:boolean = false, addMandatoryRelationships = false): IElementApi {
        let operation:IElementApi = operationManager.getElement();
        let dtoName = `${operation.getName()}Dto`;

        let existing = folder.getChildren().find(x => x.getName() == dtoName)
        if (existing) {
            return existing;
        }

        this.addMandatoryRelationships = addMandatoryRelationships;
            
        let dto = this.getOrCreateElement(dtoName, "DTO", folder);
        let dtoParam = operationManager.addChild("dto", dto.id);

        this.sourcePath.push(operation.id);
        this.sourcePath.push(dtoParam.id);
        this.targetPath.push(entity.id);
        if (inbound){            
            const entityCtor: MacroApi.Context.IElementApi = this.getEntityConstructor(entity);
            if (entityCtor != null){
                entity = entityCtor;
                this.targetPath.push(entityCtor.id);
            }
        }

        //Not this is the entity or the ctor depending
        this.target = entity;

        let result = this.populateContract(dto, entity, createMode, folder);
        return result;

    }

    public createOrGetDto(entity: IElementApi, folder: IElementApi, inbound = false): IElementApi {
        let owningAggregate = DomainHelper.getOwningAggregate(entity);
        let baseName = this.getBaseNameForElement(owningAggregate, entity, false);
        let dtoName = `${baseName}Dto`;

        let existing = folder.getChildren().find(x => x.getName() == dtoName)
        if (existing) {
            return existing;
        }

        let dto = this.getOrCreateElement(dtoName, "DTO", folder);
        this.sourcePath.push(dto.id);
        this.targetPath.push(entity.id);
        if (inbound){            
            const entityCtor: MacroApi.Context.IElementApi = this.getEntityConstructor(entity);
            if (entityCtor != null){
                entity = entityCtor;
                this.targetPath.push(entityCtor.id);
            }
        }

        //Not this is the entity or the ctor depending
        this.target = entity;

        let result = this.populateContract(dto, entity, false, folder);
        return result;
    }


    private populateContract(
        contract: MacroApi.Context.IElementApi,
        entity: MacroApi.Context.IElementApi,
        createMode: boolean,
        folder: MacroApi.Context.IElementApi,
    ): IElementApi {

        if (entity.specialization == "Class Constructor" || entity.specialization == "Operation") {
            this.addMapping(MappingType.TypeMap, [contract.id], [entity.id]);
            this.addDtoFieldsForCtor(createMode, entity, contract, folder);
        } else {
            this.addMapping(MappingType.TypeMap, [contract.id], [entity.id]);
            this.addDtoFields(createMode, entity, contract, folder, false);
        }

        return contract;
    }


    private getOrCreateContract(
        elementName: string,
        elementType: string,
        entity: MacroApi.Context.IElementApi,
        createMode: boolean,
        folder: MacroApi.Context.IElementApi,
        inbound: boolean = false,
    ): IElementApi {

        let dto = this.getOrCreateElement(elementName, elementType, folder);

        const entityCtor: MacroApi.Context.IElementApi = entity
            .getChildren("Class Constructor")
            .sort((a, b) => {
                // In descending order:
                return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
            })[0];
        if (inbound && entityCtor != null) {
            this.addMapping(MappingType.TypeMap, [dto.id], [entity.id, entityCtor.id]);
            this.addDtoFieldsForCtor(createMode, entityCtor, dto, folder);
        } else {            
            this.addMapping(MappingType.TypeMap, [dto.id], [entity.id]);
            this.addDtoFields(createMode, entity, dto, folder, inbound);
        }

        return dto;
    }

    private addDtoFields(createMode: boolean, entity: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi, inbound: boolean = false) {
        let dtoUpdated = false;
        let domainElement = entity;
        let attributesWithMapPaths = createMode ? 
            this.getAttributesWithMapPath(domainElement, (x) => this.standardAttributeFilter(x) && !this.generatedPKFilter(x)) :
            this.getAttributesWithMapPath(domainElement, this.standardAttributeFilter);

        this.addDtoFieldsInternal(attributesWithMapPaths, createMode, entity, dto, folder, inbound);
    }

    private addDtoFieldsInternal(attributes: { [index: string]: IAttributeWithMapPath }, createMode: boolean, entity: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi, inbound: boolean = false ){
        let dtoUpdated = false;
        let domainElement = entity;

        for (let keyName of Object.keys(attributes)) {
            let entry = attributes[keyName];
            if (createMode && this.isChild == true && CrudHelper.isOwnerForeignKey(entry.name, domainElement)) {
                continue;
            }
            if (dto.getChildren("DTO-Field").some(x => x.getName() == entry.name)) {
                continue;
            }
            let field = createElement("DTO-Field", entry.name, dto.id);
            console.warn("Field : " + entry.name + " , mappath =" + entry.mapPath);
            let pops = this.addMapping(MappingType.Navigation, [field.id], entry.mapPath, DomainHelper.isComplexTypeById(entry.typeId));
            if (DomainHelper.isComplexTypeById(entry.typeId)) {
                let dtoName = dto.getName().replace(/(?:Dto|Command|Query)$/, "") + field.getName() + "Dto";
                let entityField = lookup(entry.id);
                let newDto = this.getOrCreateContract(dtoName, "DTO", entityField.typeReference.getType(), createMode, folder, inbound);
                field.typeReference.setType(newDto.id);
            } else {
                field.typeReference.setType(entry.typeId);
            }
            this.popMapping(pops);

            field.typeReference.setIsNullable(entry.isNullable);
            field.typeReference.setIsCollection(entry.isCollection);
            dtoUpdated = true;
        }

        if (this.addMandatoryRelationships)
        {
            this.isChild = true;
            let requiredAssociations = DomainHelper.getMandatoryAssociationsWithMapPath(entity);
            for (let entry of requiredAssociations) {
                let field = createElement("DTO-Field", entry.name, dto.id);

                let pops = this.addMapping(MappingType.Navigation, [field.id], entry.mapPath, true);
                let dtoName = dto.getName().replace(/(?:Dto|Command|Query)$/, "") + field.getName() + "Dto";
                let entityField = lookup(entry.id);
                let newDto = this.getOrCreateContract(dtoName, "DTO", entityField.typeReference.getType(), createMode, folder, inbound);
                field.typeReference.setType(newDto.id);
                this.popMapping(pops);

                field.typeReference.setIsNullable(entry.isNullable);
                field.typeReference.setIsCollection(entry.isCollection);
                dtoUpdated = true;
            }        
        }

        if (dtoUpdated) {
            dto.collapse();
        }
    }

    private addDtoFieldsForCtor(createMode: boolean, ctor: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) {

        let childrenToAdd = DomainHelper.getChildrenOfType(ctor, "Parameter").filter(x => x.typeId != null && lookup(x.typeId).specialization !== "Domain Service");

        childrenToAdd.forEach(e => {
            if (e.mapPath != null) {
                if (dto.getChildren("Parameter").some(x => x.getMapping()?.getElement()?.id == e.id)) {
                    return;
                }
            }
            else if (ctor.getChildren("Parameter").some(x => x.getName().toLowerCase() === e.name.toLowerCase())) {
                return;
            }

            let field = createElement("DTO-Field", toPascalCase(e.name), dto.id);
            let pops = this.addMapping(MappingType.Navigation, [field.id], e.mapPath, DomainHelper.isComplexTypeById(e.typeId));
            if (DomainHelper.isComplexTypeById(e.typeId)) {
                let dtoName = dto.getName().replace(/(?:Dto|Command|Query)$/, "") + field.getName() + "Dto";
                let entityField = lookup(e.id);

                let newDto = this.getOrCreateContract(dtoName, "DTO", entityField.typeReference.getType(), createMode, folder, false);
                field.typeReference.setType(newDto.id);
            } else {
                field.typeReference.setType(e.typeId);
            }
            this.popMapping(pops);
            field.typeReference.setIsCollection(e.isCollection);
            field.typeReference.setIsNullable(e.isNullable);
        });

        dto.collapse();
    }

    private getBaseNameForElement(owningAggregate: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, entityIsMany: boolean): string {
        // Keeping 'owningAggregate' in case we still need to use it as part of the name one day
        let entityName = entityIsMany ? toPascalCase(pluralize(entity.getName())) : toPascalCase(entity.getName());
        return entityName;
    }
    
    private getOrCreateElement(elementName: string, elementType: string, parentElement: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        let existingDto = parentElement.getChildren(elementType).filter(x => x.getName() === elementName)[0];
        if (existingDto) {
            return existingDto;
        }

        let dto = createElement(elementType, elementName, parentElement.id);
        return dto;
    }

    private standardAttributeFilter(x: MacroApi.Context.IElementApi) : boolean{
        return !CrudHelper.legacyPartitionKey(x) &&
            (x["hasMetadata"] && (!x.hasMetadata("set-by-infrastructure") || x.getMetadata("set-by-infrastructure")?.toLocaleLowerCase() !== "true"));
    }

    private generatedPKFilter(x: MacroApi.Context.IElementApi) : boolean{
        return x.hasStereotype("Primary Key") && (!x.getStereotype("Primary Key").hasProperty("Data source") || x.getStereotype("Primary Key").getProperty("Data source").value != "User supplied"); 
    }

    private getAttributesWithMapPath(entity: MacroApi.Context.IElementApi,
        attributeFilter?: (attr: MacroApi.Context.IElementApi) => boolean
        ): { [index: string]: IAttributeWithMapPath } {
        
        if (attributeFilter == null){
            attributeFilter = (x) => this.standardAttributeFilter(x) && !x.hasStereotype("Primary Key");
        }
        let attrDict: { [index: string]: IAttributeWithMapPath } = Object.create(null);
        let attributes = entity.getChildren("Attribute")
            .filter(attributeFilter);
        attributes.forEach(attr => attrDict[attr.id] = {
            id: attr.id,
            name: attr.getName(),
            typeId: attr.typeReference.typeId,
            mapPath: [attr.id],
            isNullable: attr.typeReference.isNullable,
            isCollection: attr.typeReference.isCollection
        });

        traverseInheritanceHierarchyForAttributes(attrDict, entity, []);

        return attrDict;

        function traverseInheritanceHierarchyForAttributes(attrDict: { [index: string]: IAttributeWithMapPath },
            curEntity: MacroApi.Context.IElementApi,
            generalizationStack: string[]
        ): void {
            if (!curEntity) {
                return;
            }
            let generalizations = curEntity.getAssociations("Generalization").filter(x => x.isTargetEnd());
            if (generalizations.length == 0) {
                return;
            }
            let generalization = generalizations[0];
            generalizationStack.push(generalization.id);
            let nextEntity = generalization.typeReference.getType();
            let baseKeys = nextEntity.getChildren("Attribute")
                .filter(attributeFilter);
            baseKeys.forEach(attr => {
                attrDict[attr.id] = {
                    id: attr.id,
                    name: attr.getName(),
                    typeId: attr.typeReference.typeId,
                    mapPath: generalizationStack.concat([attr.id]),
                    isNullable: attr.typeReference.isNullable,
                    isCollection: attr.typeReference.isCollection
                };
            });
            traverseInheritanceHierarchyForAttributes(attrDict, nextEntity, generalizationStack);
        }
    }
}

