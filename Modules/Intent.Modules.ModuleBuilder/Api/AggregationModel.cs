using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.ApiAssociationModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AggregationModel : IMetadataModel
    {
        public const string SpecializationType = "Aggregation";
        protected readonly IAssociation _association;

        public AggregationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
            SourceEnd = new AggregationEndModel(association.SourceEnd, this);
            TargetEnd = new AggregationEndModel(association.TargetEnd, this);
        }

        public string Id => _association.Id;

        public AggregationEndModel SourceEnd { get; }
        public AggregationEndModel TargetEnd { get; }

        [IntentManaged(Mode.Fully)]
        public bool Equals(AggregationModel other)
        {
            return Equals(_association, other._association);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AggregationModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AggregationEndModel : IHasStereotypes, IMetadataModel
    {
        protected readonly IAssociationEnd _associationEnd;

        public AggregationEndModel(IAssociationEnd associationEnd, AggregationModel association)
        {
            _associationEnd = associationEnd;
        }

        public string Id => _associationEnd.Id;

        public string Name => _associationEnd.Name;

        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public bool Equals(AggregationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AggregationEndModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }
}