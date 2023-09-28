// This should work for Intent.Module.NET's MediatR.CRUD's DTO's "On Mapped" event, haven't yet applied since all the 
// onmapped in there seem to be different and I'm not sure why offhand. Also, ideally none of these scripts should live
// in the .NET repo, they should instead be in the "Intent.Modules" repo.

/// <reference path="../../common/mappingHelper.ts" />

// element = lookup("4ac690d5-2139-4a79-9326-4ef9e53dc433");
const projectMappingSettingId = "942eae46-49f1-450e-9274-a92d40ac35fa";
const originalDtoMappingSettingId = "1f747d14-681c-4a20-8c68-34223f41b825";

let fields = element.getChildren("DTO-Field")
    .filter(x => x.typeReference.getType()?.specialization != "DTO" && x.getMapping().getElement().specialization.startsWith("Association"));

fields.forEach(fieldElement => {
    const mappedElement = fieldElement.getMapping().getElement();

    let originalVerb = "";
    if (element.hasMetadata("originalVerb")) {
        originalVerb = element.getMetadata("originalVerb");
        // In the event that the prefix is no longer the same as the
        // originally called verb, then don't propagate this any further
        // as end users might get confused.
        if (element.getName().toLowerCase().indexOf(originalVerb.toLowerCase()) < 0) {
            originalVerb = "";
        }
    }

    const targetMappingSettingId = (!originalVerb || originalVerb === "")
        ? originalDtoMappingSettingId
        : projectMappingSettingId;
    const domainName = mappedElement.typeReference.getType().getName();
    const baseName = element.getMetadata("baseName")
        ? `${element.getMetadata("baseName")}${domainName}`
        : domainName;
    const elementName = `${originalVerb}${baseName}Dto`;
    
    const existingOriginalVerb = fieldElement
        .getChildren("DTO")
        .filter(x => x.getName().toLowerCase() === elementName.toLowerCase())[0]?.getMetadata("originalVerb");
    const isCreateMode = (existingOriginalVerb ?? originalVerb)?.toLowerCase()?.startsWith("create") === true;

    const dto = MappingHelper.ensureMappedToType({
        mappingSettingsId: targetMappingSettingId,
        property: fieldElement,
        typePropertySpecialization: "DTO-Field",
        typeSpecialization: "DTO",
        includeKeys: !isCreateMode,
        name: elementName
    });

    if (originalVerb !== "") {
        dto.setMetadata("originalVerb", originalVerb);
    }

    dto.setMetadata("baseName", baseName);
});
