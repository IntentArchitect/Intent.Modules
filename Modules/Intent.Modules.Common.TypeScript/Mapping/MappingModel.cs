#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.Typescript.Mapping;

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
        if (matchedMapping.Count > 1)
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

    //This constructor is used for creating artificial mappings for collection items
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
        Children = collection.Children;
        configure?.Invoke(this);
    }


    public string MappingType { get; }
    public string MappingTypeId { get; }
    public virtual ICanBeReferencedType Model { get; }
    public IElementToElementMappedEnd Mapping { get; }
    public IList<MappingModel> Children { get; set; }
    public MappingModel Parent { get; private set; }

    /// <summary>
    /// This method will return the source path to this node, even if it isn't itself mapped.
    /// In the case where it isn't mapped, it will work out it's mapping based on child mappings.
    /// </summary>
    /// <returns></returns>
    public IList<IElementMappingPathTarget> GetSourcePath()
    {
        if (Mapping != null)
        {
            return Mapping.SourcePath;
        }

        var childMappings = GetAllChildren().Where(c => c.Mapping != null).ToList();
        var mapping = childMappings.First().Mapping;
        if (childMappings.Count == 1)
        {
            return mapping.SourcePath.Take(mapping.SourcePath.Count - 1).ToList();
        }

        var toPath = mapping.SourcePath.Take(mapping.SourcePath.IndexOf(
                mapping.SourcePath.Last(x => childMappings.All(c => c.Mapping.SourcePath.Contains(x)))) + 1) // + 1 (inclusive)
            .ToList();
        return toPath;
    }

    public IList<IElementMappingPathTarget> GetTargetPath()
    {
        if (Mapping != null)
        {
            return Mapping.TargetPath;
        }

        var mapping = GetAllChildren().First(x => x.Mapping != null).Mapping;
        var targetPath = mapping.TargetPath.Take(mapping.TargetPath.IndexOf(mapping.TargetPath.Single(x => x.Element.Id == Model.Id)) + 1).ToList();
        return targetPath;
    }

    protected IEnumerable<MappingModel> GetAllChildren()
    {
        return Children.Concat(Children.SelectMany(x => x.GetAllChildren()).ToList());
    }

    public ITypescriptMapping GetMapping()
    {
        return _manager.ResolveMappings(this);
    }

    /// <summary>
    /// When you map collections sometimes we want to understand how to map the items in the collection e.g. <see cref="IList{TItem}"/> vs TItem.
    /// This method creates a mapping based on a collection item mapping adapter which represent the collection Item.
    /// </summary>
    public ITypescriptMapping GetCollectionItemMapping()
    {
        if (!this.Model.TypeReference.IsCollection)
        {
            throw new Exception("This method is intended to for resolving Collection Item mappings as opposed to the Collection itself");
        }
        return _manager.ResolveMappings(GetCollectionItemModel());
    }

    public MappingModel GetCollectionItemModel() => new CollectionItemMappingAdapter(this);

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

        private class ItemElementWrapper : IElement
        {
            private readonly ICanBeReferencedType _collectionModel;
            private readonly IElement _collectionElement;

            public ItemElementWrapper(ICanBeReferencedType collectionModel)
            {
                _collectionModel = collectionModel;
                _collectionElement = collectionModel as IElement;
                TypeReference = new ItemElementTypeWrapper(collectionModel.TypeReference);
            }

            public string SpecializationType => _collectionModel.SpecializationType;

            public string SpecializationTypeId => _collectionModel.SpecializationTypeId;

            public string Name => _collectionModel.Name;

            public string Comment => _collectionModel.Comment;

            public ITypeReference TypeReference { get; }

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
            private readonly ITypeReference _collectionType;

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