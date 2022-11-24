using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Serverless.AWS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class APIGatewayRouteIntegrationModel : IMetadataModel
    {
        public const string SpecializationType = "API Gateway Route Integration";
        public const string SpecializationTypeId = "59dc49b0-52e6-4066-a2d8-6678b9adee11";
        protected readonly IAssociation _association;
        protected APIGatewayRouteIntegrationSourceEndModel _sourceEnd;
        protected APIGatewayRouteIntegrationTargetEndModel _targetEnd;

        public APIGatewayRouteIntegrationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static APIGatewayRouteIntegrationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new APIGatewayRouteIntegrationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public APIGatewayRouteIntegrationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new APIGatewayRouteIntegrationSourceEndModel(_association.SourceEnd, this));

        public APIGatewayRouteIntegrationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new APIGatewayRouteIntegrationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(APIGatewayRouteIntegrationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((APIGatewayRouteIntegrationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class APIGatewayRouteIntegrationSourceEndModel : APIGatewayRouteIntegrationEndModel
    {
        public const string SpecializationTypeId = "6c91b79e-8a09-4bf5-a479-fb13717f756c";

        public APIGatewayRouteIntegrationSourceEndModel(IAssociationEnd associationEnd, APIGatewayRouteIntegrationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class APIGatewayRouteIntegrationTargetEndModel : APIGatewayRouteIntegrationEndModel
    {
        public const string SpecializationTypeId = "c9fe4ac2-2b12-4118-93ea-9c3c175d4368";

        public APIGatewayRouteIntegrationTargetEndModel(IAssociationEnd associationEnd, APIGatewayRouteIntegrationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class APIGatewayRouteIntegrationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly APIGatewayRouteIntegrationModel _association;

        public APIGatewayRouteIntegrationEndModel(IAssociationEnd associationEnd, APIGatewayRouteIntegrationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static APIGatewayRouteIntegrationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new APIGatewayRouteIntegrationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (APIGatewayRouteIntegrationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public APIGatewayRouteIntegrationModel Association => _association;
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

        public APIGatewayRouteIntegrationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (APIGatewayRouteIntegrationEndModel)_association.TargetEnd : (APIGatewayRouteIntegrationEndModel)_association.SourceEnd;
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

        public bool Equals(APIGatewayRouteIntegrationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((APIGatewayRouteIntegrationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class APIGatewayRouteIntegrationEndModelExtensions
    {
        public static bool IsAPIGatewayRouteIntegrationEndModel(this ICanBeReferencedType type)
        {
            return IsAPIGatewayRouteIntegrationTargetEndModel(type) || IsAPIGatewayRouteIntegrationSourceEndModel(type);
        }

        public static APIGatewayRouteIntegrationEndModel AsAPIGatewayRouteIntegrationEndModel(this ICanBeReferencedType type)
        {
            return (APIGatewayRouteIntegrationEndModel)type.AsAPIGatewayRouteIntegrationTargetEndModel() ?? (APIGatewayRouteIntegrationEndModel)type.AsAPIGatewayRouteIntegrationSourceEndModel();
        }

        public static bool IsAPIGatewayRouteIntegrationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == APIGatewayRouteIntegrationTargetEndModel.SpecializationTypeId;
        }

        public static APIGatewayRouteIntegrationTargetEndModel AsAPIGatewayRouteIntegrationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsAPIGatewayRouteIntegrationTargetEndModel() ? new APIGatewayRouteIntegrationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsAPIGatewayRouteIntegrationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == APIGatewayRouteIntegrationSourceEndModel.SpecializationTypeId;
        }

        public static APIGatewayRouteIntegrationSourceEndModel AsAPIGatewayRouteIntegrationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsAPIGatewayRouteIntegrationSourceEndModel() ? new APIGatewayRouteIntegrationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}