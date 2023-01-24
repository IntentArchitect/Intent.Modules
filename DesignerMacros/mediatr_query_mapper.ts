declare var globals;
(async () => {
//let element = lookup("29a81107-c71c-45b4-ba7b-982be63277a1")

const projectMappingSettingId = "942eae46-49f1-450e-9274-a92d40ac35fa";

var globals = initGlobals();

let fields = element.getChildren("DTO-Field")
    .filter(x => x.typeReference.getType() == null && x.getMapping().getElement().specialization === "Association");

fields.forEach(f => {
    let mappedElement = f.getMapping().getElement();

    let domainName = mappedElement.typeReference.getType().getName();
    let baseName = element.getMetadata("baseName") 
        ? `${element.getMetadata("baseName")}${domainName}`
        : `${domainName}`;
    let dtoName =  baseName;
    let dto = getOrCreateDto(dtoName, element.getParent());
    dto.setMapping(mappedElement.typeReference.getTypeId(), projectMappingSettingId);
    dto.setMetadata("baseName", baseName);

    ensureDtoFields(mappedElement, dto);
    
    f.typeReference.setType(dto.id);
});

function getFieldFormat(str) : string {
    return toPascalCase(str);
}

function getDomainAttributeNameFormat(str) : string {
    let convention = getDomainAttributeNamingConvention();
    switch (convention) {
        case "pascal-case":
            return toPascalCase(str);
        case "camel-case":
            return toCamelCase(str);
    }
    return str;
}

function initGlobals() {
    return {
        PKSpecialization: {
            Implicit: "implicit",
            Explicit: "explicit",
            ExplicitComposite: "explicit_composite",
            Unknown: "unknown"
        },
        FKSpecialization: {
            Implicit: "implicit",
            Explicit: "explicit",
        }
    };
}

function getOrCreateDto(elementName, parentElement) {
    const expectedQueryName = `${elementName}Dto`;
    let existingDto = parentElement.getChildren("DTO").filter(x => x.name === expectedQueryName)[0];
    if (existingDto) {
        return existingDto;
    }

    let dto = createElement("DTO", expectedQueryName, parentElement.id);
    return dto;
}

function ensureDtoFields(mappedElement, dto) {
    let dtoUpdated = false;
    let mappedElementAttributes = mappedElement
        .typeReference
        .getType()
        .getChildren("Attribute");
    let dtoFields = dto.getChildren("DTO-Field");
    for (let attribute of mappedElementAttributes.filter(x => ! dtoFields.some(y => x.name === y.name))) {
        let field = createElement("DTO-Field", attribute.name, dto.id);
        field.typeReference.setType(attribute.typeReference.typeId);
        field.typeReference.setIsNullable(attribute.typeReference.isNullable);
        field.typeReference.setIsCollection(attribute.typeReference.isCollection);
        field.setMapping(attribute.id);
        dtoUpdated = true;
    }

    if (dtoUpdated) {
        dto.collapse();
    }
}

function getDomainAttributeNamingConvention() {
    const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
    return application.getSettings(domainSettingsId)
        ?.getField("Attribute Naming Convention")?.value ?? "pascal-case";
}

// Just in case someone still uses this convention. Used to filter out those attributes when mapping
// to domain entities that are within a Cosmos DB paradigm.
function legacyPartitionKey(attribute) {
    return attribute.hasStereotype("Partition Key") && attribute.name === "PartitionKey";
}
});