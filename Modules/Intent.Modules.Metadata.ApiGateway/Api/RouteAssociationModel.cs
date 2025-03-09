using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Metadata.ApiGateway.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class RouteAssociationModel : IMetadataModel
    {
        public const string SpecializationType = "Route Association";
        public const string SpecializationTypeId = "dd35ed32-76bb-4abf-a4e0-370f230765cf";
        protected readonly IAssociation _association;
        protected UpstreamEndModel _sourceEnd;
        protected DownstreamEndModel _targetEnd;

        public RouteAssociationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static RouteAssociationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new RouteAssociationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public UpstreamEndModel SourceEnd => _sourceEnd ??= new UpstreamEndModel(_association.SourceEnd, this);

        public DownstreamEndModel TargetEnd => _targetEnd ??= new DownstreamEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(RouteAssociationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RouteAssociationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class UpstreamEndModel : RouteAssociationEndModel
    {
        public const string SpecializationTypeId = "4143dbe5-12ce-4705-ba69-2683c46b1b96";
        public const string SpecializationType = "Upstream End";

        public UpstreamEndModel(IAssociationEnd associationEnd, RouteAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class DownstreamEndModel : RouteAssociationEndModel
    {
        public const string SpecializationTypeId = "17084378-a3d8-4233-b275-c856eea4dd61";
        public const string SpecializationType = "Downstream End";

        public DownstreamEndModel(IAssociationEnd associationEnd, RouteAssociationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class RouteAssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly RouteAssociationModel _association;

        public RouteAssociationEndModel(IAssociationEnd associationEnd, RouteAssociationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static RouteAssociationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new RouteAssociationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (RouteAssociationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public RouteAssociationModel Association => _association;
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

        public RouteAssociationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (RouteAssociationEndModel)_association.TargetEnd : (RouteAssociationEndModel)_association.SourceEnd;
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

        public bool Equals(RouteAssociationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RouteAssociationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class RouteAssociationEndModelExtensions
    {
        public static bool IsRouteAssociationEndModel(this ICanBeReferencedType type)
        {
            return IsDownstreamEndModel(type) || IsUpstreamEndModel(type);
        }

        public static RouteAssociationEndModel AsRouteAssociationEndModel(this ICanBeReferencedType type)
        {
            return (RouteAssociationEndModel)type.AsDownstreamEndModel() ?? (RouteAssociationEndModel)type.AsUpstreamEndModel();
        }

        public static bool IsDownstreamEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == DownstreamEndModel.SpecializationTypeId;
        }

        public static DownstreamEndModel AsDownstreamEndModel(this ICanBeReferencedType type)
        {
            return type.IsDownstreamEndModel() ? new RouteAssociationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsUpstreamEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == UpstreamEndModel.SpecializationTypeId;
        }

        public static UpstreamEndModel AsUpstreamEndModel(this ICanBeReferencedType type)
        {
            return type.IsUpstreamEndModel() ? new RouteAssociationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}