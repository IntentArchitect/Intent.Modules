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
    public class AssociationModel : IMetadataModel
    {
        [IntentManaged(Mode.Fully)]
        public const string SpecializationType = "Association";
        [IntentManaged(Mode.Fully)]
        protected readonly IAssociation _association;

        public AssociationModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
            SourceEnd = new AssociationEndModel(association.SourceEnd, this);
            TargetEnd = new AssociationEndModel(association.TargetEnd, this);
        }

        [IntentManaged(Mode.Fully)]
        public static AssociationEndModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new AssociationModel(associationEnd.Association);
            return associationEnd.IsSourceEnd() ? association.SourceEnd : association.TargetEnd;
        }


        [IntentManaged(Mode.Fully)]
        public string Id => _association.Id;

        [IntentManaged(Mode.Fully)]
        public AssociationEndModel SourceEnd { get; }

        [IntentManaged(Mode.Fully)]
        public AssociationEndModel TargetEnd { get; }

        [IntentManaged(Mode.Fully)]
        public IAssociation InternalAssociation => _association;

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _association.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(AssociationModel other)
        {
            return Equals(_association, other._association);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AssociationEndModel : IAssociationEnd
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly AssociationModel _association;

        public AssociationEndModel(IAssociationEnd associationEnd, AssociationModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public string Id => _associationEnd.Id;
        public string Name => _associationEnd.Name;
        public AssociationModel Association => _association;
        IAssociation IAssociationEnd.Association => _association.InternalAssociation;
        public bool IsNavigable => _associationEnd.IsNavigable;
        public bool IsNullable => _associationEnd.IsNullable;
        public bool IsCollection => _associationEnd.IsCollection;
        public IElement Element => _associationEnd.Element;
        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.GenericTypeParameters;
        public string Comment => _associationEnd.Comment;
        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;

        public IAssociationEnd OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? _association.TargetEnd : _association.SourceEnd;
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

        public bool Equals(AssociationEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }
}