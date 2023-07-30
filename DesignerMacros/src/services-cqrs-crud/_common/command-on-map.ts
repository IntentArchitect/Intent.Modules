/// <reference path="./on-map-functions.ts" />
/// <reference path="../../common/domainHelper.ts" />
/// <reference path="../../common/servicesHelper.ts" />
/// <reference path="../../common/getParent.ts" />
function onMapCommand(element: MacroApi.Context.IElementApi, isForCrudScript: boolean): void {
    const projectMappingSettingId = "942eae46-49f1-450e-9274-a92d40ac35fa";
    const mapFromDomainMappingSettingId = "1f747d14-681c-4a20-8c68-34223f41b825";
    const mappedElement = element.getMapping()?.getElement();
    if (mappedElement == null) {
        return;
    }

    const mappedElementSpecialization: "Class" | "Operation" | "Constructor" = element.getMapping().getElement().specialization as any;

    let entity = mappedElement;
    if (entity.specialization !== "Class") {
        entity = getParent(entity, "Class");
    }

    // Add pks and fks for CRUD implementations to be able to work:
    if (isForCrudScript) {
        const owningAggregate = DomainHelper.getOwningAggregate(entity);
        if (owningAggregate != null) {
            const fks = DomainHelper.getForeignKeys(entity, owningAggregate).map(fk => {
                if (mappedElementSpecialization !== "Class") {
                    fk.mapPath = null
                }

                return fk;
            });

            const command = new ElementManager(element, { childSpecialization: "DTO-Field" });
            command.addChildrenFrom(fks);
        }

        if (!element.getName().toLowerCase().startsWith("create")) {
            const pksShouldBeMapped = mappedElementSpecialization === "Class";
            addPrimaryKeys(element, entity, pksShouldBeMapped);
        }
    }

    if (mappedElementSpecialization === "Operation" &&
        isComplexType(element.getMapping()?.getElement()?.typeReference?.getType())
    ) {
        getOrCreateCommandCrudDto(element, element, false, mapFromDomainMappingSettingId);
    }

    let fields = element.getChildren("DTO-Field")
        .filter(x => x.typeReference.getType() == null && x.getMapping().getElement().specialization === "Association");

    fields.forEach(f => {
        getOrCreateCommandCrudDto(element, f, true, projectMappingSettingId);
    });

    let complexFields = element.getChildren("DTO-Field")
        .filter(x =>
            x.typeReference.getType() == null &&
            isComplexType(x.getMapping()?.getElement()?.typeReference?.getType()));

    complexFields.forEach(cf => {
        getOrCreateCommandCrudDto(element, cf, false, projectMappingSettingId);
    });

    function isComplexType(element: MacroApi.Context.IElementApi): boolean {
        return element?.specialization === "Data Contract" ||
            element?.specialization === "Value Object";
    }
}

function getOrCreateCommandCrudDto(command: MacroApi.Context.IElementApi, dtoField: MacroApi.Context.IElementApi, autoAddPrimaryKey: boolean, mappingTypeSettingId: string) {
    let mappedElement = dtoField.getMapping().getElement();
    if (mappedElement.typeReference == null) throw new Error("TypeReference is undefined");

    let originalVerb = (command.getName().split(/(?=[A-Z])/))[0];
    let domainName = mappedElement.typeReference.getType().getName();
    let baseName = command.getMetadata("baseName")
        ? `${command.getMetadata("baseName")}${domainName}`
        : domainName;
    let dtoName = `${originalVerb}${baseName}`;
    let dto = getOrCreateDto(dtoName, command.getParent());
    dto.setMetadata("originalVerb", originalVerb);
    dto.setMetadata("baseName", baseName);
    dto.setMapping(mappedElement.typeReference.getTypeId(), mappingTypeSettingId);
    ensureDtoFields(autoAddPrimaryKey, mappedElement, dto);
    dtoField.typeReference.setType(dto.id);
}
