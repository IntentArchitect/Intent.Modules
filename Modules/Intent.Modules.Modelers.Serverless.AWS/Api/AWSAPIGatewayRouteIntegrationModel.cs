using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Serverless.AWS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AWSAPIGatewayRouteIntegrationModel : IMetadataModel
    {
        public const string SpecializationType = "AWS API Gateway Route Integration";
        public const string SpecializationTypeId = "59dc49b0-52e6-4066-a2d8-6678b9adee11";
        protected readonly IAssociation _association;
        protected AWSAPIGatewayRouteIntegrationSourceEndModel _sourceEnd;
        protected AWSAPIGatewayRouteIntegrationTargetEndModel _targetEnd;

        public AWSAPIGatewayRouteIntegrationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static AWSAPIGatewayRouteIntegrationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new AWSAPIGatewayRouteIntegrationModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public AWSAPIGatewayRouteIntegrationSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new AWSAPIGatewayRouteIntegrationSourceEndModel(_association.SourceEnd, this));

        public AWSAPIGatewayRouteIntegrationTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new AWSAPIGatewayRouteIntegrationTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(AWSAPIGatewayRouteIntegrationModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AWSAPIGatewayRouteIntegrationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class AWSAPIGatewayRouteIntegrationSourceEndModel : AWSAPIGatewayRouteIntegrationEndModel
    {
        public const string SpecializationTypeId = "6c91b79e-8a09-4bf5-a479-fb13717f756c";

        public AWSAPIGatewayRouteIntegrationSourceEndModel(IAssociationEnd associationEnd, AWSAPIGatewayRouteIntegrationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class AWSAPIGatewayRouteIntegrationTargetEndModel : AWSAPIGatewayRouteIntegrationEndModel
    {
        public const string SpecializationTypeId = "c9fe4ac2-2b12-4118-93ea-9c3c175d4368";

        public AWSAPIGatewayRouteIntegrationTargetEndModel(IAssociationEnd associationEnd, AWSAPIGatewayRouteIntegrationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class AWSAPIGatewayRouteIntegrationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly AWSAPIGatewayRouteIntegrationModel _association;

        public AWSAPIGatewayRouteIntegrationEndModel(IAssociationEnd associationEnd, AWSAPIGatewayRouteIntegrationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static AWSAPIGatewayRouteIntegrationEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new AWSAPIGatewayRouteIntegrationModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (AWSAPIGatewayRouteIntegrationEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public AWSAPIGatewayRouteIntegrationModel Association => _association;
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

        public AWSAPIGatewayRouteIntegrationEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (AWSAPIGatewayRouteIntegrationEndModel)_association.TargetEnd : (AWSAPIGatewayRouteIntegrationEndModel)_association.SourceEnd;
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

        public bool Equals(AWSAPIGatewayRouteIntegrationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AWSAPIGatewayRouteIntegrationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AWSAPIGatewayRouteIntegrationEndModelExtensions
    {
        public static bool IsAWSAPIGatewayRouteIntegrationEndModel(this ICanBeReferencedType type)
        {
            return IsAWSAPIGatewayRouteIntegrationTargetEndModel(type) || IsAWSAPIGatewayRouteIntegrationSourceEndModel(type);
        }

        public static AWSAPIGatewayRouteIntegrationEndModel AsAWSAPIGatewayRouteIntegrationEndModel(this ICanBeReferencedType type)
        {
            return (AWSAPIGatewayRouteIntegrationEndModel)type.AsAWSAPIGatewayRouteIntegrationTargetEndModel() ?? (AWSAPIGatewayRouteIntegrationEndModel)type.AsAWSAPIGatewayRouteIntegrationSourceEndModel();
        }

        public static bool IsAWSAPIGatewayRouteIntegrationTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == AWSAPIGatewayRouteIntegrationTargetEndModel.SpecializationTypeId;
        }

        public static AWSAPIGatewayRouteIntegrationTargetEndModel AsAWSAPIGatewayRouteIntegrationTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsAWSAPIGatewayRouteIntegrationTargetEndModel() ? new AWSAPIGatewayRouteIntegrationModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsAWSAPIGatewayRouteIntegrationSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == AWSAPIGatewayRouteIntegrationSourceEndModel.SpecializationTypeId;
        }

        public static AWSAPIGatewayRouteIntegrationSourceEndModel AsAWSAPIGatewayRouteIntegrationSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsAWSAPIGatewayRouteIntegrationSourceEndModel() ? new AWSAPIGatewayRouteIntegrationModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}