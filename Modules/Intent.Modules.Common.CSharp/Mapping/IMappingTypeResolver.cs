namespace Intent.Modules.Common.CSharp.Mapping;

public interface IMappingTypeResolver
{
    ICSharpMapping ResolveMappings(MappingModel mappingModel);
}