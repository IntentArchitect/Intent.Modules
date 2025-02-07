/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../services-cqrs-crud/_common/onMapDto.ts" />

const domainEntities = lookupTypesOf("Class");

const entities = new Map<string, IElementApi>();
for (const entity of domainEntities) {
    entities.set(entity.getName().toLowerCase(), entity);
}

let dtos = lookupTypesOf("DTO");
for (let dto of dtos) {
    if (dto.getMapping()) { continue; }

    const dtoName = dto.getName().toLowerCase();
    const entityName = dtoName
        .replace(/(response|dto|list|item)$/, "")
        .replace(/(response|dto|list|item)$/, "")
        .replace(/(response|dto|list|item)$/, "")
        .toLowerCase();
    const entity = entities.get(entityName);
    if (!entity) {
        continue;
    }

    let dtoMapped = false;
    for (let field of dto.getChildren("DTO-Field")) {
        let attribute = entity.getChildren("Attribute").filter(x => field.getName() === x.getName())[0];
        if (attribute) {
            if (!dtoMapped) {
                dtoMapped = true;
                dto.setMapping(entity.id);
            }
            field.setMapping(attribute.id);
            continue;
        }
        let association = entity.getAssociations("Association").filter(x => field.getName() === x.getName())[0];
        if (association) {
            if (!dtoMapped) {
                dtoMapped = true;
                dto.setMapping(entity.id);
            }
            field.setMapping([association.id]);
            continue;
        }
    }

    onMapDto(dto, dto.getParent("Folder"));
}