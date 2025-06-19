using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Intent.Modules.Common.CSharp.Mapping;

public class MappingModel
{
    private readonly MappingManagerBase _manager;

    public MappingModel(IElementToElementMapping mapping, MappingManagerBase manager)
        : this(mapping.Type, mapping.TypeId, mapping.TargetElement, mapping.MappedEnds, manager)
    {
    }

    public MappingModel(string mappingType,
        string mappingTypeId,
        IElementToElementMappedEnd mapping,
        MappingManagerBase manager) : this(
        mappingType: mappingType,
            mappingTypeId: mappingTypeId,
            model: mapping.TargetElement,
            mappings: new List<IElementToElementMappedEnd>() { mapping },
            manager: manager,
            level: mapping.TargetPath.Count)
    {
    }

    public MappingModel(string mappingType,
        string mappingTypeId,
        IList<IElementToElementMappedEnd> mappings,
        MappingManagerBase manager) : this(
        mappingType: mappingType,
        mappingTypeId: mappingTypeId,
        model: mappings.First().TargetElement,
        mappings: mappings,
        manager: manager,
        level: mappings.First().TargetPath.Count)
    {
    }

    private MappingModel(
        string mappingType,
        string mappingTypeId,
        ICanBeReferencedType model,
        IList<IElementToElementMappedEnd> mappings,
        MappingManagerBase manager,
        int level = 1)
    {
        var matchedMapping = mappings.Where(x => x.TargetElement == model).ToList();
        if (matchedMapping.Count() > 1)
        {
            throw new Exception($"Illegal Mapping: Multiple mappings were found for element {model.Name} [{model.Id}]");
        }

        MappingType = mappingType;
        MappingTypeId = mappingTypeId;
        _manager = manager;
        Model = model;
        Mapping = matchedMapping.SingleOrDefault();
        Children = mappings.Where(x => x.TargetPath.Count > level)
            .GroupBy(x => x.TargetPath.Skip(level).First(), x => x)
            .Select(x => new MappingModel(mappingType, mappingTypeId, x.Key.Element, x.ToList(), manager, level + 1))
            .OrderBy(x => ((IElement)x.Model).Order)
            .ToList();
        foreach (var child in Children)
        {
            child.Parent = this;
        }
    }

    //This constructor is used for creating artifical mappings for collection items
	private MappingModel(
		MappingModel collection,
		ICanBeReferencedType itemModel,
        Action<MappingModel> configure = null
		)
	{
		MappingType = collection.MappingType;
		MappingTypeId = collection.MappingTypeId;
		_manager = collection._manager;
		Model = itemModel;
		Mapping = null;
		CodeContext = collection.CodeContext;
        Children = collection.Children;
        if (configure != null) 
        {
            configure(this);

		}
    }


	public string MappingType { get; }
    public string MappingTypeId { get; }
    public virtual ICanBeReferencedType Model { get; }
    public IElementToElementMappedEnd Mapping { get; }
    public IList<MappingModel> Children { get; set; }
    public MappingModel Parent { get; private set; }
    public ICSharpCodeContext CodeContext { get; set; }

    public ICSharpMapping GetMapping()
    {
        return _manager.ResolveMappings(this);
    }

    /// <summary>
    /// When you map collections sometimes we want to understand how to map the items in the collection e.g. IList<TItem> vs TItem
    /// This methods creates a mapping based on a collection item mapping adapter which represent the collection Item.
    /// </summary>
    public ICSharpMapping GetCollectionItemMapping()
	{
        if (!this.Model.TypeReference.IsCollection)
        {
            throw new Exception("This method is intended to for resolving Collection Item mappings as opposed to the Collection itself");
        }
		return _manager.ResolveMappings(GetCollectionItemModel());
	}

    public MappingModel GetCollectionItemModel() => new CollectionItemMappingAdapter(this);

    //public IEnumerable<MappingModel> GetAllChildren(Func<MappingModel, bool> predicate = null)
    //{
    //    var result = new List<MappingModel>();
    //    foreach (var mappingModel in Children)
    //    {
    //        if (predicate == null || predicate(mappingModel))
    //        {
    //            result.Add(mappingModel);
    //            result.AddRange(mappingModel.GetAllChildren(predicate));
    //        }
    //    }
    //    return result;
    //}

    public MappingModel GetParent(Func<MappingModel, bool> predicate = null)
    {
        if (predicate == null)
        {
            return Parent;
        }

        var parent = Parent;
        while (parent != null && predicate(parent) == false)
        {
            parent = parent.Parent;
        }
        return parent;
    }

    /// <summary>
    /// This class creates Mapping model which represents the Items in the collection
    /// It assumes all the characteristics of the collection mapping except it changes the TypeReference to not be a collection
    /// </summary>
    private class CollectionItemMappingAdapter : MappingModel
	{
        public CollectionItemMappingAdapter(MappingModel collectionMapping) : base(collectionMapping, new ItemElementWrapper(collectionMapping.Model))
        {
		}

		private class ItemElementWrapper : ICanBeReferencedType, IElement
		{
            private ICanBeReferencedType _collectionModel;
			private IElement _collectionElement;
			private ITypeReference _collectionItemType;

			public ItemElementWrapper(ICanBeReferencedType collectionModel)
            {
                _collectionModel = collectionModel;
                _collectionElement = collectionModel as IElement;
				_collectionItemType = new ItemElementTypeWrapper(collectionModel.TypeReference);
			}

            public string SpecializationType => _collectionModel.SpecializationType;

			public string SpecializationTypeId => _collectionModel.SpecializationTypeId;

			public string Name => _collectionModel.Name;

			public string Comment => _collectionModel.Comment;

			public ITypeReference TypeReference => _collectionItemType;

			public IPackage Package => _collectionModel.Package;

			public string Id => _collectionModel.Id;

			public IEnumerable<IStereotype> Stereotypes => _collectionModel.Stereotypes;

			public bool IsChild => _collectionElement.IsChild;

			public int Order => _collectionElement.Order;

			public string ExternalReference => _collectionElement.ExternalReference;

			public string Value => _collectionElement.Value;

			public bool IsAbstract => _collectionElement.IsAbstract;

			public IEnumerable<IGenericType> GenericTypes => _collectionElement.GenericTypes;

			public string ParentId => _collectionElement.ParentId;

			public IElement ParentElement => _collectionElement.ParentElement;

			public IEnumerable<IElement> ChildElements => _collectionElement.ChildElements;

			public bool IsMapped => _collectionElement.IsMapped;

			public IElementMapping MappedElement => _collectionElement.MappedElement;

			public IElementApplication Application => _collectionElement.Application;

			public IEnumerable<IAssociationEnd> AssociatedElements => _collectionElement.AssociatedElements;

			public IEnumerable<IAssociation> OwnedAssociations => _collectionElement.OwnedAssociations;

			public IDiagram Diagram => _collectionElement.Diagram;

			public IDictionary<string, string> Metadata => _collectionElement.Metadata;
		}

		private class ItemElementTypeWrapper : ITypeReference
        {
			private ITypeReference _collectionType;
			public ItemElementTypeWrapper(ITypeReference collectionType)
			{
				_collectionType = collectionType;
			}

			public bool IsNullable => false;

			public bool IsCollection => false;

			public ICanBeReferencedType Element => _collectionType.Element;

			public IEnumerable<ITypeReference> GenericTypeParameters => _collectionType.GenericTypeParameters;

			public IEnumerable<IStereotype> Stereotypes => _collectionType.Stereotypes;
		}
	}
}

public abstract class MappingManagerBase
{
    private readonly ICSharpTemplate _template;
    private readonly Dictionary<string, string> _fromReplacements = new();
    private List<IMetadataModel> _fromReplacementModels = new(); // Used to maintain backward compatibility
    private readonly Dictionary<string, string> _toReplacements = new();
    private readonly List<IMetadataModel> _toReplacementModels = new(); // Used to maintain backward compatibility
    private readonly List<IMappingTypeResolver> _mappingResolvers = new();

    protected MappingManagerBase(ICSharpTemplate template)
    {
        _template = template;
    }

    public CSharpStatement GenerateCreationStatement(IElementToElementMapping model)
    {
        return GenerateCreationStatement(model, _template.RootCodeContext);
    }

    public CSharpStatement GenerateCreationStatement(IElementToElementMapping model, ICSharpCodeContext context)
    {
        //var mapping = CreateMapping(new MappingModel(model), GetCreateMappingType);
        var mappingModel = new MappingModel(model, this);
        var mapping = ResolveMappings(mappingModel);
        ApplyReplacements(mapping);

        return mapping.GetSourceStatement();
    }

    public IList<CSharpStatement> GenerateUpdateStatements(IElementToElementMapping model)
    {
        return GenerateUpdateStatements(model, _template.RootCodeContext);
    }

    public IList<CSharpStatement> GenerateUpdateStatements(IElementToElementMapping model, ICSharpCodeContext context)
    {
        //var mapping = CreateMapping(new MappingModel(model, this), GetUpdateMappingType);
        //ApplyReplacements(mapping);

        var mappingModel = new MappingModel(model, this);
        var mapping = ResolveMappings(mappingModel);//, new ObjectUpdateMapping(mappingModel, _template));
        ApplyReplacements(mapping);

        return mapping.GetMappingStatements().ToList();
    }

    public CSharpStatement GenerateSourceStatementForMapping(IElementToElementMapping model, IElementToElementMappedEnd mappingEnd) => GenerateSourceStatementForMapping(model, mappingEnd, default);
    public CSharpStatement GenerateSourceStatementForMapping(IElementToElementMapping model, IElementToElementMappedEnd mappingEnd, bool? targetIsNullable)
    {
        //var mapping = CreateMapping(new MappingModel(model, this), GetUpdateMappingType);
        //ApplyReplacements(mapping);
        // Get provided mapping and it's children:
        var mappings = model.MappedEnds.Where(m => mappingEnd.TargetPath.All(t => m.TargetPath.Any(x => x.Id == t.Id)))
            .OrderBy(x => x.TargetPath.Count)
            .ToList();
        var mappingModel = new MappingModel(model.Type, model.TypeId, mappings, this);
        var mapping = ResolveMappings(mappingModel);//, new ObjectUpdateMapping(mappingModel, _template));
        ApplyReplacements(mapping);

        return mapping.GetSourceStatement(targetIsNullable);
    }

    public CSharpStatement GenerateTargetStatementForMapping(IElementToElementMapping model, IElementToElementMappedEnd mappingEnd)
    {
        var mappingModel = new MappingModel(model.Type, model.TypeId, mappingEnd, this);
        var mapping = ResolveMappings(mappingModel);//, new ObjectUpdateMapping(mappingModel, _template));
        ApplyReplacements(mapping);

        return mapping.GetTargetStatement();
    }

    public void SetFromReplacement(IMetadataModel type, string replacement)
    {
        if (_fromReplacements.ContainsKey(type.Id))
        {
            _fromReplacements.Remove(type.Id);
            _fromReplacementModels.Remove(type);
        }
        _fromReplacements.Add(type.Id, replacement);
        _fromReplacementModels.Add(type);
    }

    public string GetFromReplacement(IMetadataModel type)
    {
        return _fromReplacements.TryGetValue(type.Id, out var replacement) ? replacement : null;
    }

    public void SetToReplacement(IMetadataModel type, string replacement)
    {
        if (_toReplacements.ContainsKey(type.Id))
        {
            _toReplacements.Remove(type.Id);
            _toReplacementModels.Remove(type);
        }
        _toReplacements.Add(type.Id, replacement);
        _toReplacementModels.Add(type);
    }

    public string GetToReplacement(IMetadataModel type)
    {
        return _toReplacements.TryGetValue(type.Id, out var replacement) ? replacement : null;
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

        return defaultMapping ?? (model.Mapping != null ? new DefaultCSharpMapping(model, _template) : new MapChildrenMapping(model, _template));
    }


    public void AddMappingResolver(IMappingTypeResolver resolver)
    {
        _mappingResolvers.Add(resolver);
    }

    public void ClearMappingResolvers()
    {
        _mappingResolvers.Clear();
    }

    private ICSharpMapping CreateMapping(
        ICanBeReferencedType model,
        IList<IElementToElementMappedEnd> mappings,
        Func<ICanBeReferencedType, IElementToElementMappedEnd, List<ICSharpMapping>, ICSharpMapping> mappingResolver,
        int level = 1)
    {
        var mapping = mappings.SingleOrDefault(x => x.TargetPath.Last().Element == model);
        var children = mappings.Where(x => x.TargetPath.Count > level)
            .GroupBy(x => x.TargetPath.Skip(level).First(), x => x)
            .Select(x => CreateMapping(x.Key.Element, x.ToList(), mappingResolver, level + 1))
            .OrderBy(x => ((IElement)x.Model).Order)
            .ToList();
        return mappingResolver(model, mapping, children.ToList());
    }

    private void ApplyReplacements(ICSharpMapping mapping)
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

    //private class MetadataModelEqualityWrapper : IEquatable<IMetadataModel>
    //{
    //    public IMetadataModel Model { get; }

    //    public MetadataModelEqualityWrapper(IMetadataModel model)
    //    {
    //        Model = model;
    //    }


    //    public bool Equals(MetadataModelEqualityWrapper other)
    //    {
    //        return Model.Id.Equals(other?.Model.Id);
    //    }

    //    public bool Equals(IMetadataModel other)
    //    {
    //        return Model.Id.Equals(other?.Id);
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (ReferenceEquals(null, obj)) return false;
    //        if (ReferenceEquals(this, obj)) return true;
    //        if (obj.GetType() != this.GetType()) return false;
    //        return Equals((MetadataModelEqualityWrapper)obj);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return (Model != null ? Model.Id.GetHashCode() : 0);
    //    }
    //}
}