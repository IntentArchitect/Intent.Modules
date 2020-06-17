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
        }

        [IntentManaged(Mode.Fully)]
        public static GeneralizationModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new GeneralizationModel(associationEnd.Association);
            return association;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _association.Id;

        [IntentManaged(Mode.Fully)]
        public GeneralizationSourceEndModel SourceEnd => new GeneralizationSourceEndModel(_association.SourceEnd, this);

        [IntentManaged(Mode.Fully)]
        public GeneralizationTargetEndModel TargetEnd => new GeneralizationTargetEndModel(_association.TargetEnd, this);

        [IntentManaged(Mode.Fully)]
        public bool Equals(GeneralizationModel other)
        {
            return Equals(_association, other?._association);
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
        [IntentManaged(Mode.Fully)]
        protected readonly IAssociationEnd _associationEnd;

        [IntentManaged(Mode.Fully)]
        public GeneralizationEndModel(IAssociationEnd associationEnd, GeneralizationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _associationEnd.Id;
        [IntentManaged(Mode.Fully)]
        public string Name => _associationEnd.Name;
        [IntentManaged(Mode.Fully)]
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
        [IntentManaged(Mode.Fully)]
        private readonly GeneralizationModel _association;
        [IntentManaged(Mode.Fully)]
        public GeneralizationModel Association => _association;
        [IntentManaged(Mode.Fully)]
        IAssociation IAssociationEnd.Association => _association.InternalAssociation;
        [IntentManaged(Mode.Fully)]
        public string Comment => _associationEnd.Comment;
        [IntentManaged(Mode.Fully)]
        public IElement Element => _associationEnd.Element;
        [IntentManaged(Mode.Fully)]
        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.GenericTypeParameters;
        [IntentManaged(Mode.Fully)]
        public bool IsCollection => _associationEnd.IsCollection;
        [IntentManaged(Mode.Fully)]
        public bool IsNavigable => _associationEnd.IsNavigable;
        [IntentManaged(Mode.Fully)]
        public bool IsNullable => _associationEnd.IsNullable;

        [IntentManaged(Mode.Fully)]
        public bool IsSourceEnd()
        {
            return _associationEnd.IsSourceEnd();
        }

        [IntentManaged(Mode.Fully)]
        public bool IsTargetEnd()
        {
            return _associationEnd.IsTargetEnd();
        }

        [IntentManaged(Mode.Fully)]
        IAssociationEnd IAssociationEnd.OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (IAssociationEnd)_association.TargetEnd : (IAssociationEnd)_association.SourceEnd;
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _associationEnd.ToString();
        }
    }

    [IntentManaged(Mode.Fully)]
    public class GeneralizationSourceEndModel : GeneralizationEndModel
    {
        public GeneralizationSourceEndModel(IAssociationEnd associationEnd, GeneralizationModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class GeneralizationTargetEndModel : GeneralizationEndModel
    {
        public GeneralizationTargetEndModel(IAssociationEnd associationEnd, GeneralizationModel association) : base(associationEnd, association)
        {
        }
    }
}