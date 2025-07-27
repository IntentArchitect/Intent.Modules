#nullable enable
using System;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Intent.Modules.Common.CSharp.Mapping;

public interface IMappingTypeResolver
{
    ICSharpMapping? ResolveMappings(MappingModel mappingModel)
    {
        return ResolveMappings(mappingModel, null);
    }

    ICSharpMapping? ResolveMappings(MappingModel mappingModel, MappingTypeResolverDelegate next)
    {
        return ResolveMappings(mappingModel);
    }
}

/// <summary>
/// Delegate to the next registered <see cref="IMappingTypeResolver"/> to resolve the <see cref="ICSharpMapping"/>.
/// </summary>
/// <param name="model"></param>
/// <returns></returns>
public delegate ICSharpMapping? MappingTypeResolverDelegate(MappingModel model);