using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GeneralizationModel : IMetadataModel
    {
        [IntentManaged(Mode.Fully)]
        public const string SpecializationType = "Generalization";
        [IntentManaged(Mode.Fully)]
        protected readonly IAssociation _association;

        public GeneralizationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
            SourceEnd = new GeneralizationEndModel(association.SourceEnd, this);
            TargetEnd = new GeneralizationEndModel(association.TargetEnd, this);
        }

        [IntentManaged(Mode.Fully)]
        public static GeneralizationEndModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new GeneralizationModel(associationEnd.Association);
            return associationEnd.IsSourceEnd() ? association.SourceEnd : association.TargetEnd;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _association.Id;

        [IntentManaged(Mode.Fully)]
        public GeneralizationEndModel SourceEnd { get; }

        [IntentManaged(Mode.Fully)]
        public GeneralizationEndModel TargetEnd { get; }

        [IntentManaged(Mode.Fully)]
        public bool Equals(GeneralizationModel other)
        {
            return Equals(_association, other._association);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GeneralizationModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _association.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IAssociation InternalAssociation => _association;
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GeneralizationEndModel : IAssociationEnd
    {
        protected readonly IAssociationEnd _associationEnd;

        public GeneralizationEndModel(IAssociationEnd associationEnd, GeneralizationModel association)
        {
            _associationEnd = associationEnd;
        }

        public string Id => _associationEnd.Id;

        public string Name => _associationEnd.Name;

        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;

        public ITypeReference TypeReference => _associationEnd;

        [IntentManaged(Mode.Fully)]
        public bool Equals(GeneralizationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GeneralizationEndModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
        private readonly GeneralizationModel _association;

        public GeneralizationModel Association => _association;

        IAssociation IAssociationEnd.Association => _association.InternalAssociation;

        public string Comment => _associationEnd.Comment;

        public IElement Element => _associationEnd.Element;

        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.GenericTypeParameters;

        public bool IsCollection => _associationEnd.IsCollection;

        public bool IsNavigable => _associationEnd.IsNavigable;

        public bool IsNullable => _associationEnd.IsNullable;

        public bool IsSourceEnd()
        {
            return _associationEnd.IsSourceEnd();
        }

        public bool IsTargetEnd()
        {
            return _associationEnd.IsTargetEnd();
        }

        public IAssociationEnd OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? _association.TargetEnd : _association.SourceEnd;
        }

        public override string ToString()
        {
            return _associationEnd.ToString();
        }
    }
}