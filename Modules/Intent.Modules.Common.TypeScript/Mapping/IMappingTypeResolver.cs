#nullable enable
using System;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Intent.Modules.Common.Typescript.Mapping;

public interface IMappingTypeResolver
{
    [Obsolete("Implement the ResolveMappings(MappingModel mappingModel, MappingTypeResolverDelegate next) overload instead.")]
    ITypescriptMapping? ResolveMappings(MappingModel mappingModel)
    {
        return ResolveMappings(mappingModel, null);
    }

    ITypescriptMapping? ResolveMappings(MappingModel mappingModel, MappingTypeResolverDelegate next)
    {
        return ResolveMappings(mappingModel);
    }
}

/// <summary>
/// Delegate to the next registered <see cref="IMappingTypeResolver"/> to resolve the <see cref="ITypescriptMapping"/>.
/// </summary>
/// <param name="model"></param>
/// <returns></returns>
public delegate ITypescriptMapping? MappingTypeResolverDelegate(MappingModel model);