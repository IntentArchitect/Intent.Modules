using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    internal class Class : IClass, IEquatable<IClass>
    {
        private IList<IAssociationEnd> _associatedClasses = new List<IAssociationEnd>();
        private readonly Metadata.Models.IClass _class;
        private readonly ICollection<IClass> _childClasses = new List<IClass>();
        private Class _parent;

        public Class(Metadata.Models.IClass @class)
        {
            _class = @class;
            var parent = _class.AssociatedClasses.FirstOrDefault(x =>
                x.Association.AssociationType == Metadata.Models.AssociationType.Generalization)?.Class;
            if (parent != null)
            {
                _parent = new Class(parent);
                _parent._childClasses.Add(this);
            }

            _associatedClasses = @class.AssociatedClasses.Select(x => new Association(x.Association).TargetEnd).ToList();
        }

        public string Id => _class.Id;
        public IEnumerable<IStereotype> Stereotypes => _class.Stereotypes;
        public IFolder Folder => _class.Folder;
        public string Name => _class.Name;
        public bool IsAbstract => _class.IsAbstract;
        public IEnumerable<string> GenericTypes => _class.GenericTypes;
        public IClass ParentClass => _parent;
        public IEnumerable<IClass> ChildClasses => _childClasses;
        public bool IsMapped => _class.IsMapped;
        public IClassMapping MappedClass => _class.MappedClass;
        public IApplication Application => _class.Application;
        public IEnumerable<IAttribute> Attributes => _class.Attributes;
        public IEnumerable<IOperation> Operations => _class.Operations;
        public IEnumerable<IAssociationEnd> AssociatedClasses
        {
            get => _associatedClasses;
            set => _associatedClasses = (IList<IAssociationEnd>)value;
        }

        public IEnumerable<IAssociation> OwnedAssociations
        {
            get { return AssociatedClasses.Select(x => x.Association).Distinct().Where(x => Equals(x.SourceEnd.Class, this)); }
        }

        public string Comment => _class.Id;

        public bool Equals(IClass other)
        {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IClass)obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}