using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Modelers.UI.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class NavigationModel : IMetadataModel
    {
        public const string SpecializationType = "Navigation";
        public const string SpecializationTypeId = "6d2b2070-c1cb-4cd2-88b4-4e5f8414bd9e";
        protected readonly IAssociation _association;
        protected NavigationSourceEndModel _sourceEnd;
        protected NavigationTargetEndModel _targetEnd;

        public NavigationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static NavigationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new NavigationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public NavigationSourceEndModel SourceEnd => _sourceEnd ??= new NavigationSourceEndModel(_association.SourceEnd, this);

        public NavigationTargetEndModel TargetEnd => _targetEnd ??= new NavigationTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(NavigationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NavigationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class NavigationSourceEndModel : NavigationEndModel
    {
        public const string SpecializationTypeId = "97a3de8a-c9bf-4cf2-bc0a-b8692b02211b";
        public const string SpecializationType = "Navigation Source End";

        public NavigationSourceEndModel(IAssociationEnd associationEnd, NavigationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class NavigationTargetEndModel : NavigationEndModel, IInvokableModel
    {
        public const string SpecializationTypeId = "2b191288-ecae-4743-b069-cbdd927ef349";
        public const string SpecializationType = "Navigation Target End";

        public NavigationTargetEndModel(IAssociationEnd associationEnd, NavigationModel association) : base(associationEnd, association)
        {
        }
        public IList<ParameterModel> Parameters => InternalElement.ChildElements
            .GetElementsOfType(ParameterModel.SpecializationTypeId)
            .Select(x => new ParameterModel(x))
            .ToList();


        public IEnumerable<IElementToElementMapping> Mappings => _associationEnd.Mappings;
    }

    [IntentManaged(Mode.Fully)]
    public class NavigationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly NavigationModel _association;

        public NavigationEndModel(IAssociationEnd associationEnd, NavigationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static NavigationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new NavigationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (NavigationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public NavigationModel Association => _association;
        public IElement InternalElement => _associationEnd;
        public IAssociationEnd InternalAssociationEnd => _associationEnd;
        public IAssociation InternalAssociation => _association.InternalAssociation;
        public bool IsNavigable => _associationEnd.IsNavigable;
        public bool IsNullable => _associationEnd.TypeReference.IsNullable;
        public bool IsCollection => _associationEnd.TypeReference.IsCollection;
        public ICanBeReferencedType Element => _associationEnd.TypeReference.Element;
        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.TypeReference.GenericTypeParameters;
        public ITypeReference TypeReference => this;
        public IPackage Package => Element?.Package;
        public string Comment => _associationEnd.Comment;
        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;

        public NavigationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (NavigationEndModel)_association.TargetEnd : (NavigationEndModel)_association.SourceEnd;
        }

        public bool IsTargetEnd()
        {
            return _associationEnd.IsTargetEnd();
        }

        public bool IsSourceEnd()
        {
            return _associationEnd.IsSourceEnd();
        }

        public override string ToString()
        {
            return _associationEnd.ToString();
        }

        public bool Equals(NavigationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NavigationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class NavigationEndModelExtensions
    {
        public static bool IsNavigationEndModel(this ICanBeReferencedType type)
        {
            return IsNavigationTargetEndModel(type) || IsNavigationSourceEndModel(type);
        }

        public static NavigationEndModel AsNavigationEndModel(this ICanBeReferencedType type)
        {
            return (NavigationEndModel)type.AsNavigationTargetEndModel() ?? (NavigationEndModel)type.AsNavigationSourceEndModel();
        }

        public static bool IsNavigationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == NavigationTargetEndModel.SpecializationTypeId;
        }

        public static NavigationTargetEndModel AsNavigationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsNavigationTargetEndModel() ? new NavigationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsNavigationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == NavigationSourceEndModel.SpecializationTypeId;
        }

        public static NavigationSourceEndModel AsNavigationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsNavigationSourceEndModel() ? new NavigationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}