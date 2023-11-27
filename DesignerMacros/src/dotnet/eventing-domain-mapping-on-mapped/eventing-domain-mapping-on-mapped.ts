/**
 * Used by Intent.Modules.NET\Modules\Intent.Modules.Eventing.Contracts.DomainMapping by the "On Mapped" event for both the DTOs and Messages.
 *
 * Source code here:
 * https://github.com/IntentSoftware/Intent.Modules/blob/master/DesignerMacros/dotnet/eventing-domain-mapping-on-mapped/eventing-domain-mapping-on-mapped.ts
 */

//element = lookup("f94b6b5a-514f-45f6-be31-92499ea8173b");
/// <reference path="../../common/mappingHelper.ts" />

const mappingSettingId = "e437007c-33fd-46d5-9293-d4529b4b82e6";

let properties = element.getChildren()
    .filter(x => x.typeReference.getType()?.specialization != "Eventing DTO" && x.getMapping().getElement().specialization.startsWith("Association"));

for (const property of properties) {
    MappingHelper.ensureMappedToType({
        mappingSettingsId: mappingSettingId,
        property: property,
        typePropertySpecialization: "Eventing DTO-Field",
        typeSpecialization: "Eventing DTO",
        name: `${property.getMapping().getElement().typeReference.getType().getName()}Dto`
    });
}
