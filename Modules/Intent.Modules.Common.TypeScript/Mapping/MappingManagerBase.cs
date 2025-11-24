#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Typescript.Mapping;
using Intent.Modules.Common.TypeScript.Builder;
using Intent.Modules.Common.TypeScript.Templates;

namespace Intent.Modules.Common.Typescript.Mapping;

public abstract class MappingManagerBase
{
    private readonly ITypescriptTemplate _template;
    private readonly Dictionary<string, string> _fromReplacements = new();
    private readonly List<IMetadataModel> _fromReplacementModels = []; // Used to maintain backward compatibility
    private readonly Dictionary<string, string> _toReplacements = new();
    private readonly List<IMetadataModel> _toReplacementModels = []; // Used to maintain backward compatibility
    private readonly List<(IMappingTypeResolver Resolver, int Priority)> _mappingResolvers = [];

    protected MappingManagerBase(ITypescriptTemplate template)
    {
        _template = template;
    }

    public TypescriptStatement GenerateCreationStatement(IElementToElementMapping model)
    {
        var mappingModel = new MappingModel(model, this);
        var mapping = ResolveMappings(mappingModel);
        ApplyReplacements(mapping);

        return mapping.GetSourceStatement();
    }

    public IList<TypescriptStatement> GenerateUpdateStatements(IElementToElementMapping model)
    {
        var mappingModel = new MappingModel(model, this);
        var mapping = ResolveMappings(mappingModel);
        ApplyReplacements(mapping);

        return mapping.GetMappingStatements().ToList();
    }

    public TypescriptStatement GenerateSourceStatementForMapping(IElementToElementMapping model, IElementToElementMappedEnd mappingEnd) => GenerateSourceStatementForMapping(model, mappingEnd, null);
    public TypescriptStatement GenerateSourceStatementForMapping(IElementToElementMapping model, IElementToElementMappedEnd mappingEnd, bool? targetIsNullable)
    {
        // Get provided mapping and its children:
        var mappings = model.MappedEnds.Where(m => mappingEnd.TargetPath.All(t => m.TargetPath.Any(x => x.Id == t.Id)))
            .OrderBy(x => x.TargetPath.Count)
            .ToList();
        var mappingModel = new MappingModel(model.Type, model.TypeId, mappings, this);
        var mapping = ResolveMappings(mappingModel);
        ApplyReplacements(mapping);

        return mapping.GetSourceStatement(targetIsNullable);
    }

    public TypescriptStatement GenerateTargetStatementForMapping(IElementToElementMapping model, IElementToElementMappedEnd mappingEnd)
    {
        var mappingModel = new MappingModel(model.Type, model.TypeId, mappingEnd, this);
        var mapping = ResolveMappings(mappingModel);
        ApplyReplacements(mapping);

        //TODO
        return new TypescriptStatement("");
        //return mapping.GetTargetStatement();
    }

    public void SetFromReplacement(IMetadataModel type, string replacement)
    {
        if (_fromReplacements.Remove(type.Id))
        {
            _fromReplacementModels.Remove(type);
        }

        _fromReplacements.Add(type.Id, replacement);
        _fromReplacementModels.Add(type);
    }

    public string GetFromReplacement(IMetadataModel type)
    {
        return _fromReplacements.GetValueOrDefault(type.Id);
    }

    public void SetToReplacement(IMetadataModel type, string replacement)
    {
        if (_toReplacements.Remove(type.Id))
        {
            _toReplacementModels.Remove(type);
        }

        _toReplacements.Add(type.Id, replacement);
        _toReplacementModels.Add(type);
    }

    public string GetToReplacement(IMetadataModel type)
    {
        return _toReplacements.GetValueOrDefault(type.Id);
    }

    public ITypescriptMapping ResolveMappings(MappingModel model, ITypescriptMapping defaultMapping = null)
    {
        var orderedResolvers = _mappingResolvers.OrderBy(x => x.Priority).Select(x => x.Resolver).ToList();
        //foreach (var resolver in orderedResolvers)
        //{
        //    var found = resolver.ResolveMappings(model);
        //    if (found != null)
        //    {
        //        return found;
        //    }
        //}

        var result = BuildPipeline(orderedResolvers)(model);
        if (result != null)
        {
            return result;
        }

        return defaultMapping ?? (model.Mapping != null ? new DefaultTypescriptMapping(model, _template) : new MapChildrenMapping(model, _template));
    }

    private static MappingTypeResolverDelegate BuildPipeline(
        List<IMappingTypeResolver> resolvers,
        int index = 0)
    {
        // Base case: end of the chain
        if (index >= resolvers.Count)
        {
            return _ => null!;
        }

        var currentResolver = resolvers[index];
        var next = BuildPipeline(resolvers, index + 1);

        return model =>
        {
            // Prefer ResolveMappings with delegate support
            var result = currentResolver.ResolveMappings(model, next);

            // Fallback to legacy method (optional)
            if (result == null)
            {
                for (int i = index + 1; i < resolvers.Count; i++)
                {
                    result = resolvers[i].ResolveMappings(model);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return result;
        };
    }

    public void AddMappingResolver(IMappingTypeResolver resolver) => AddMappingResolver(resolver, priority: 0);

    public void AddMappingResolver(IMappingTypeResolver resolver, int priority)
    {
        _mappingResolvers.Add((resolver, priority));
    }

    public void ClearMappingResolvers()
    {
        _mappingResolvers.Clear();
    }

    private void ApplyReplacements(ITypescriptMapping mapping)
    {
        foreach (var model in _fromReplacementModels)
        {
            mapping.SetSourceReplacement(model, GetFromReplacement(model));
        }

        foreach (var model in _toReplacementModels)
        {
            mapping.SetTargetReplacement(model, GetToReplacement(model));
        }
    }
}