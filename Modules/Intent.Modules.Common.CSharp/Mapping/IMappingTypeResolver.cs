#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Intent.Modules.Common.CSharp.Mapping;

public interface IMappingTypeResolver
{
    ICSharpMapping? ResolveMappings(MappingModel mappingModel);
}