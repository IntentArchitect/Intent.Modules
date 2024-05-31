class CrudConstants {
    static mapFromDomainMappingSettingId = "1f747d14-681c-4a20-8c68-34223f41b825";
    static mapToDomainConstructorForDtosSettingId = "8d1f6a8a-77c8-43a2-8e60-421559725419";
    static dtoFromEntityMappingId = "1f747d14-681c-4a20-8c68-34223f41b825";
}

class CrudHelper {

    static getName(command: MacroApi.Context.IElementApi, mappedElement: MacroApi.Context.IElementApi, dtoPrefix: string = null): string{
        if (mappedElement.typeReference != null) 
            mappedElement = mappedElement.typeReference.getType();

        let originalVerb = (command.getName().split(/(?=[A-Z])/))[0];
        let domainName = mappedElement.getName();
        let baseName = command.getMetadata("baseName")
            ? `${command.getMetadata("baseName")}${domainName}`
            : domainName;
        let dtoName = `${originalVerb}${baseName}`;
        if (dtoPrefix)
            dtoName = `${dtoPrefix}${dtoName}`;
        return dtoName;
    }

    static getOrCreateCrudDto(
        dtoName: string,
        mappedElement: MacroApi.Context.IElementApi,
        autoAddPrimaryKey: boolean,
        mappingTypeSettingId: string,
        folder: MacroApi.Context.IElementApi,
        inbound: boolean = false,
    ): MacroApi.Context.IElementApi {
    
        let dto = CrudHelper.getOrCreateDto(dtoName, folder);

        //dtoField.typeReference.setType(dto.id);
        const entityCtor: MacroApi.Context.IElementApi = mappedElement
        .getChildren("Class Constructor")
        .sort((a, b) => {
            // In descending order:
            return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
        })[0];
        if (inbound && entityCtor != null) {
            dto.setMapping([mappedElement.id, entityCtor.id], CrudConstants.mapToDomainConstructorForDtosSettingId);
            CrudHelper.addDtoFieldsForCtor(autoAddPrimaryKey, entityCtor, dto, folder);
        } else {
            dto.setMapping(mappedElement.id, mappingTypeSettingId);
            CrudHelper.addDtoFields(autoAddPrimaryKey, mappedElement, dto, folder);
        }

        return dto;
    }

    static getOrCreateDto(elementName: string, parentElement: MacroApi.Context.IElementApi): MacroApi.Context.IElementApi {
        const expectedDtoName = elementName.replace(/Dto$/, "") + "Dto";
        let existingDto = parentElement.getChildren("DTO").filter(x => x.getName() === expectedDtoName)[0];
        if (existingDto) {
            return existingDto;
        }
    
        let dto = createElement("DTO", expectedDtoName, parentElement.id);
        return dto;
    }
    

    static addDtoFieldsForCtor(autoAddPrimaryKey: boolean, ctor: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) {
    
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
            field.setMapping(e.mapPath);
            if (DomainHelper.isComplexTypeById(e.typeId)){
                let dtoName = dto.getName().replace(/Dto$/, "") + field.getName() + "Dto";
                let newDto = CrudHelper.getOrCreateCrudDto(dtoName, field.getMapping().getElement().typeReference.getType(), autoAddPrimaryKey, CrudConstants.mapFromDomainMappingSettingId, folder, true );
                field.typeReference.setType(newDto.id);
            }else{
                field.typeReference.setType(e.typeId);
            }
            field.typeReference.setIsCollection(e.isCollection);
            field.typeReference.setIsNullable(e.isNullable);
        });
    
        dto.collapse();
    }
    
    static addDtoFields(autoAddPrimaryKey: boolean, mappedElement: MacroApi.Context.IElementApi, dto: MacroApi.Context.IElementApi, folder: MacroApi.Context.IElementApi) {
        let dtoUpdated = false;
        let domainElement = mappedElement;
        let attributesWithMapPaths = CrudHelper.getAttributesWithMapPath(domainElement);
        let isCreateMode = dto.getMetadata("originalVerb")?.toLowerCase()?.startsWith("create") == true;
        for (var keyName of Object.keys(attributesWithMapPaths)) {
            let entry = attributesWithMapPaths[keyName];
            if (isCreateMode && CrudHelper.isOwnerForeignKey(entry.name, domainElement)) {
                continue;
            }
            if (dto.getChildren("DTO-Field").some(x => x.getName() == entry.name)) {
                continue;
            }
            let field = createElement("DTO-Field", entry.name, dto.id);
            field.setMapping(entry.mapPath);
            if (DomainHelper.isComplexTypeById(entry.typeId)){
                let dtoName = dto.getName().replace(/Dto$/, "") + field.getName() + "Dto";
                let newDto = CrudHelper.getOrCreateCrudDto(dtoName, field.getMapping().getElement().typeReference.getType(), autoAddPrimaryKey, CrudConstants.mapFromDomainMappingSettingId, folder, true );
                field.typeReference.setType(newDto.id);
            }else{
                field.typeReference.setType(entry.typeId);
            }
            field.typeReference.setIsNullable(entry.isNullable);
            field.typeReference.setIsCollection(entry.isCollection);
            dtoUpdated = true;
        }
    
        if (autoAddPrimaryKey && !isCreateMode) {
            CrudHelper.addPrimaryKeys(dto, domainElement, true);
        }    
    
        if (dtoUpdated) {
            dto.collapse();
        }
    }  

    static isOwnerForeignKey(attributeName: string, domainElement: MacroApi.Context.IElementApi): boolean {
        for (let association of domainElement.getAssociations().filter(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable)) {
            if (attributeName.toLowerCase().indexOf(association.getName().toLowerCase()) >= 0) {
                return true;
            }
        }
        return false;
    }
    
    static addPrimaryKeys(dto: MacroApi.Context.IElementApi, entity: MacroApi.Context.IElementApi, map: boolean): void {
        const primaryKeys = CrudHelper.getPrimaryKeysWithMapPath(entity);
    
        for (const primaryKey of primaryKeys) {
            const name = CrudHelper.getDomainAttributeNameFormat(primaryKey.name);
            if (dto.getChildren("DTO-Field").some(x => x.getName().toLowerCase() == name.toLowerCase())) {
                continue;
            }
    
            const dtoField = createElement("DTO-Field", CrudHelper.getFieldFormat(name), dto.id);
            dtoField.typeReference.setType(primaryKey.typeId)
    
            if (map && primaryKey.mapPath != null) {
                console.log(`Doing mapping for ${dtoField.id}`);
                dtoField.setMapping(primaryKey.mapPath);
            }
        }
    }
        
    static getPrimaryKeysWithMapPath(entity: MacroApi.Context.IElementApi): IAttributeWithMapPath[] {
        let keydict: { [index: string]: IAttributeWithMapPath } = Object.create(null);
        let keys = entity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
        keys.forEach(key => keydict[key.id] = {
            id: key.id,
            name: key.getName(),
            typeId: key.typeReference.typeId,
            mapPath: [key.id],
            isNullable: false,
            isCollection: false
        });
    
        traverseInheritanceHierarchyForPrimaryKeys(keydict, entity, []);
    
        return Object.values(keydict);
    
        function traverseInheritanceHierarchyForPrimaryKeys(
            keydict: { [index: string]: IAttributeWithMapPath },
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
            let baseKeys = nextEntity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
            baseKeys.forEach(key => {
                keydict[key.id] = {
                    id: key.id,
                    name: key.getName(),
                    typeId: key.typeReference.typeId,
                    mapPath: generalizationStack.concat([key.id]),
                    isNullable: key.typeReference.isNullable,
                    isCollection: key.typeReference.isCollection
                };
            });
            traverseInheritanceHierarchyForPrimaryKeys(keydict, nextEntity, generalizationStack);
        }
    }
    
    static getAttributesWithMapPath(entity: MacroApi.Context.IElementApi): { [index: string]: IAttributeWithMapPath } {
        let attrDict: { [index: string]: IAttributeWithMapPath } = Object.create(null);
        let attributes = entity.getChildren("Attribute")
            .filter(x => !x.hasStereotype("Primary Key") &&
                !CrudHelper.legacyPartitionKey(x) &&
                (x["hasMetadata"] && (!x.hasMetadata("set-by-infrastructure") || x.getMetadata("set-by-infrastructure")?.toLocaleLowerCase() != "true")));
        attributes.forEach(attr => attrDict[attr.id] = {
            id: attr.id,
            name: attr.getName(),
            typeId: attr.typeReference.typeId,
            mapPath: [attr.id],
            isNullable: false,
            isCollection: false
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
            let baseKeys = nextEntity.getChildren("Attribute").filter(x => !x.hasStereotype("Primary Key") && !CrudHelper.legacyPartitionKey(x));
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

    static getFieldFormat(str: string): string {
        return toPascalCase(str);
    }
    
    static getDomainAttributeNameFormat(str: string): string {
        let convention = CrudHelper.getDomainAttributeNamingConvention();
    
        switch (convention) {
            case "pascal-case":
                return toPascalCase(str);
            case "camel-case":
                return toCamelCase(str);
            default:
                return str;
        }
    }

    static getDomainAttributeNamingConvention(): "pascal-case" | "camel-case" {
        const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
        return <any>application.getSettings(domainSettingsId)
            ?.getField("Attribute Naming Convention")?.value ?? "pascal-case";
    }
    
    // Just in case someone still uses this convention. Used to filter out those attributes when mapping
    // to domain entities that are within a Cosmos DB paradigm.
    static legacyPartitionKey(attribute: MacroApi.Context.IElementApi): boolean {
        return attribute.hasStereotype("Partition Key") && attribute.getName() === "PartitionKey";
    }
    
}

interface IAttributeWithMapPath {
    id: string,
    name: string,
    typeId: string,
    mapPath: string[],
    isNullable: boolean,
    isCollection: boolean,
}
