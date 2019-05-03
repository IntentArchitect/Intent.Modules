using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    internal class Class : IClass, IEquatable<IClass>
    {
        private IList<IAssociationEnd> _associatedClasses;
        private readonly Metadata.Models.IClass _class;
        private readonly ICollection<IClass> _childClasses = new List<IClass>();
        private readonly Class _parent;

        public Class(Metadata.Models.IClass @class, IDictionary<string, Class> classCache)
        {
            _class = @class;
            classCache.Add(Id, this);
            var parent = _class.AssociatedClasses.FirstOrDefault(x =>
                x.Association.AssociationType == Metadata.Models.AssociationType.Generalization)?.Model;
            if (parent != null)
            {
                _parent = classCache.ContainsKey(parent.Id) ? classCache[parent.Id] : new Class(parent, classCache);
                _parent._childClasses.Add(this);
            }

            _associatedClasses = @class.AssociatedClasses.Select(x =>
            {
                var association = new Association(x.Association, classCache);
                return Equals(association.TargetEnd.Class, this) ? association.SourceEnd : association.TargetEnd;
            }).ToList();
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

        public static bool operator ==(Class lhs, Class rhs)
        {
            // Check for null.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles the case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Class lhs, Class rhs)
        {
            return !(lhs == rhs);
        }

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