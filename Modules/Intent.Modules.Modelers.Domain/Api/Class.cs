using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    internal class Class : IClass, IEquatable<IClass>
    {
        private IList<IAssociationEnd> _associatedClasses;
        private readonly IElement _class;
        private readonly ICollection<IClass> _childClasses = new List<IClass>();
        private readonly Class _parent;

        public Class(IElement @class, IDictionary<string, Class> classCache)
        {
            _class = @class;
            classCache.Add(Id, this);
            Folder = Api.Folder.SpecializationType.Equals(_class.ParentElement?.SpecializationType, StringComparison.OrdinalIgnoreCase) ? new Folder(_class.ParentElement) : null;

            var generalizedFrom = _class.AssociatedClasses
                .Where(x => "Generalization".Equals(x.Association.SpecializationType, StringComparison.OrdinalIgnoreCase) &&
                            x.Association.SourceEnd.Element.Id == _class.Id)
                .ToArray();
            if (generalizedFrom.Length > 1)
            {
                throw new Exception($"[{_class.Name} - {_class.Id}] is generalized from more than one class.");
            }

            var parent = generalizedFrom.SingleOrDefault()?.Element;
            if (parent != null)
            {
                _parent = classCache.ContainsKey(parent.Id) ? classCache[parent.Id] : new Class(parent, classCache);
                _parent._childClasses.Add(this);
            }

            _associatedClasses = @class.AssociatedClasses
                .Where(x => "Composition".Equals(x.Association.SpecializationType, StringComparison.OrdinalIgnoreCase)
                || "Aggregation".Equals(x.Association.SpecializationType, StringComparison.OrdinalIgnoreCase))
                .Select(x =>
                {
                    var association = new Association(x.Association, classCache);
                    return Equals(association.TargetEnd.Class, this) ? association.SourceEnd : association.TargetEnd;
                })
                .ToList();
        }

        public string Id => _class.Id;
        public IEnumerable<IStereotype> Stereotypes => _class.Stereotypes;
        public IFolder Folder { get; }
        public string Name => _class.Name;
        public bool IsAbstract => _class.IsAbstract;
        public IEnumerable<string> GenericTypes => _class.GenericTypes.Select(x => x.Name);
        public IClass ParentClass => _parent;
        public IEnumerable<IClass> ChildClasses => _childClasses;
        public bool IsMapped => _class.IsMapped;
        public string Comment => _class.Comment;
        public IElementMapping MappedClass => _class.MappedElement;
        public IElementApplication Application => _class.Application;
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

        public static bool operator ==(Class lhs, Class rhs)
        {
            // Check for null.
            if (ReferenceEquals(lhs, null))
            {
                if (ReferenceEquals(rhs, null))
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
            return string.Equals(Id, other?.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IClass)obj);
        }

        public override int GetHashCode()
        {
            return Id != null ? Id.GetHashCode() : 0;
        }
    }
}