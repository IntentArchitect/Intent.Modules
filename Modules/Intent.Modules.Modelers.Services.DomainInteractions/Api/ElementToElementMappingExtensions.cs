using Intent.Metadata.Models;

namespace Intent.Modelers.Services.DomainInteractions.Api;

public static class ElementToElementMappingExtensions
{
    public static bool IsCommandToClassCreationMapping(this IElementToElementMapping mapping)
    {
        return mapping.MappingTypeId == "5f172141-fdba-426b-980e-163e782ff53e";
    }

    public static bool IsCommandToClassUpdateMapping(this IElementToElementMapping mapping)
    {
        return mapping.MappingTypeId == "01721b1a-a85d-4320-a5cd-8bd39247196a";
    }
}