using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public class MappingModel
{
    private readonly MappingManagerBase _manager;

    public MappingModel(IElementToElementMapping mapping, MappingManagerBase manager) : this(mapping.MappingType, mapping.MappingTypeId, mapping.ToElement, mapping.Connections, manager)
    {
    }

    private MappingModel(string mappingType,
        string mappingTypeId, 
        ICanBeReferencedType model, 
        IList<IElementToElementMappingConnection> mappings, 
        MappingManagerBase manager, 
        int level = 1)
    {
        MappingType = mappingType;
        MappingTypeId = mappingTypeId;
        _manager = manager;
        Model = model;
        Mapping = mappings.SingleOrDefault(x => x.ToPath.Last().Element == model);
        Children = mappings.Where(x => x.ToPath.Count > level)
            .GroupBy(x => x.ToPath.Skip(level).First(), x => x)
            .Select(x => new MappingModel(mappingType, mappingTypeId, x.Key.Element, x.ToList(), manager, level + 1))
            .OrderBy(x => ((IElement)x.Model).Order)
            .ToList();
    }

    public string MappingType { get; }
    public string MappingTypeId { get; }
    public ICanBeReferencedType Model { get; set; }
    public IElementToElementMappingConnection Mapping { get; set; }
    public IList<MappingModel> Children { get; set; }

    public ICSharpMapping GetMapping()
    {
        return _manager.ResolveMappings(this);
    }
}

public abstract class MappingManagerBase
{
    private readonly ICSharpFileBuilderTemplate _template;
    protected Dictionary<IMetadataModel, string> _fromReplacements = new();
    protected Dictionary<IMetadataModel, string> _toReplacements = new();
    private readonly List<IMappingTypeResolver> _mappingResolvers = new();

    protected MappingManagerBase(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public CSharpStatement GenerateCreationStatement(IElementToElementMapping model)
    {
        //var mapping = CreateMapping(new MappingModel(model), GetCreateMappingType);
        var mappingModel = new MappingModel(model, this);
        var mapping = ResolveMappings(mappingModel, new ObjectInitializationMapping(mappingModel, _template));
        ApplyReplacements(mapping);

        return mapping.GetFromStatement();
    }

    public IList<CSharpStatement> GenerateUpdateStatements(IElementToElementMapping model)
    {
        //var mapping = CreateMapping(new MappingModel(model, this), GetUpdateMappingType);
        //ApplyReplacements(mapping);

        var mappingModel = new MappingModel(model, this);
        var mapping = ResolveMappings(mappingModel);//, new ObjectUpdateMapping(mappingModel, _template));
        ApplyReplacements(mapping);

        return mapping.GetMappingStatements().ToList();
    }

    public void SetFromReplacement(IMetadataModel type, string replacement)
    {
        if (_fromReplacements.ContainsKey(type))
        {
            _fromReplacements.Remove(type);
        }
        _fromReplacements.Add(type, replacement);
    }

    public void SetToReplacement(IMetadataModel type, string replacement)
    {
        if (_toReplacements.ContainsKey(type))
        {
            _toReplacements.Remove(type);
        }
        _toReplacements.Add(type, replacement);
    }

    public ICSharpMapping ResolveMappings(MappingModel model, ICSharpMapping defaultMapping = null)
    {
        foreach (var resolver in _mappingResolvers)
        {
            var found = resolver.ResolveMappings(model);
            if (found != null)
            {
                return found;
            }
        }

        return defaultMapping ?? new DefaultCSharpMapping(model, _template);
        //throw new Exception($"No mapping could be resolved for model: {model.Model.Name} [{model.Model.SpecializationType}] with mapping [{model.MappingType}]");
        //var key = model.Model.TypeReference?.Element?.SpecializationType ?? ""; // what about operations that return?
        //if (!_mappingResolvers.ContainsKey(key))
        //{
        //    key = model.Model.SpecializationType;
        //}
        //if (!_mappingResolvers.ContainsKey(key))
        //{
        //    key = "__default";
        //}
        //if (!_mappingResolvers.ContainsKey(key))
        //{
        //    return null; // throw?
        //}

        //var mapping = _mappingResolvers[key](model);
        //foreach (var replacement in _fromReplacements)
        //{
        //    mapping.SetFromReplacement(replacement.Key, replacement.Value);
        //}
        //foreach (var replacement in _toReplacements)
        //{
        //    mapping.SetToReplacement(replacement.Key, replacement.Value);
        //}
        //return mapping;
    }

    //public ICSharpMapping ResolveUpdateMappings(MappingModel model)
    //{
    //    foreach (var resolver in _mappingResolvers)
    //    {
    //        var found = resolver.ResolveMappings(model);
    //        if (found != null)
    //        {
    //            return found;
    //        }
    //    }

    //    return null;
    //}

    public void AddMappingResolver(IMappingTypeResolver resolver)
    {
        _mappingResolvers.Add(resolver);
    }

    //private ICSharpMapping CreateMapping(MappingModel model, Func<ICanBeReferencedType, IElementToElementMappingConnection, List<ICSharpMapping>, ICSharpMapping> mappingResolver)
    //{
    //    var children = model.Children
    //        .Select(x => mappingResolver(x.Model, x.Mapping, x.Children.Select(c => CreateMapping(c, mappingResolver)).ToList()))
    //        .ToList();
    //    return mappingResolver(model.Model, model.Mapping, children);
    //}

    private ICSharpMapping CreateMapping(
        ICanBeReferencedType model,
        IList<IElementToElementMappingConnection> mappings,
        Func<ICanBeReferencedType, IElementToElementMappingConnection, List<ICSharpMapping>, ICSharpMapping> mappingResolver,
        int level = 1)
    {
        var mapping = mappings.SingleOrDefault(x => x.ToPath.Last().Element == model);
        var children = mappings.Where(x => x.ToPath.Count > level)
            .GroupBy(x => x.ToPath.Skip(level).First(), x => x)
            .Select(x => CreateMapping(x.Key.Element, x.ToList(), mappingResolver, level + 1))
            .OrderBy(x => ((IElement)x.Model).Order)
            .ToList();
        return mappingResolver(model, mapping, children.ToList());
    }

    private void ApplyReplacements(ICSharpMapping mapping)
    {
        foreach (var fromReplacement in _fromReplacements)
        {
            mapping.SetFromReplacement(fromReplacement.Key, fromReplacement.Value);
        }

        foreach (var toReplacement in _toReplacements)
        {
            mapping.SetToReplacement(toReplacement.Key, toReplacement.Value);
        }
    }


}