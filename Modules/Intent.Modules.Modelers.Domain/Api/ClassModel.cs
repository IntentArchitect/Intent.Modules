using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public class ClassModel : IEquatable<ClassModel>
    {
        private IList<IAssociationEnd> _associatedElements;
        private readonly IElement _element;
        private readonly ICollection<ClassModel> _childClasses = new List<ClassModel>();
        private readonly ClassModel _parent;

        public ClassModel(IElement element, IDictionary<string, ClassModel> classCache)
        {
            _element = element;
            classCache.Add(_element.UniqueKey(), this);
            Folder = Api.Folder.SpecializationType.Equals(_element.ParentElement?.SpecializationType, StringComparison.OrdinalIgnoreCase) ? new Folder(_element.ParentElement) : null;

            var generalizedFrom = _element.AssociatedElements
                .Where(x => "Generalization".Equals(x.Association.SpecializationType, StringComparison.OrdinalIgnoreCase) &&
                            x.Association.SourceEnd.Element.Id == _element.Id)
                .ToArray();
            if (generalizedFrom.Length > 1)
            {
                throw new Exception($"[{_element.Name} - {_element.Id}] is generalized from more than one class.");
            }

            var parent = generalizedFrom.SingleOrDefault()?.Element;
            if (parent != null)
            {
                _parent = classCache.ContainsKey(parent.UniqueKey()) ? classCache[parent.UniqueKey()] : new ClassModel(parent, classCache);
                _parent._childClasses.Add(this);
            }

            _associatedElements = element.AssociatedElements
                .Where(x => "Composition".Equals(x.Association.SpecializationType, StringComparison.OrdinalIgnoreCase)
                || "Aggregation".Equals(x.Association.SpecializationType, StringComparison.OrdinalIgnoreCase))
                .Where(end => !(end.Association.TargetEnd.Element.Equals(end.Association.SourceEnd.Element) && end == end.Association.SourceEnd))
                .Select(x =>
                {
                    var association = new Association(x.Association, classCache);
                    return Equals(association.TargetEnd.Class, this) && !association.IsSelfReference() ? association.SourceEnd : association.TargetEnd;
                })
                .ToList();
        }

        public string UniqueKey => $"{_element.Application.Id}_{Id}";
        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public IFolder Folder { get; }
        public string Name => _element.Name;
        public bool IsAbstract => _element.IsAbstract;
        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);
        public ClassModel ParentClass => _parent;
        public IEnumerable<ClassModel> ChildClasses => _childClasses;
        public bool IsMapped => _element.IsMapped;
        public string Comment => _element.Comment;
        public IElementMapping MappedClass => _element.MappedElement;
        public IElementApplication Application => _element.Application;
        public IEnumerable<IAttribute> Attributes => _element.Attributes;
        public IEnumerable<IOperation> Operations => _element.Operations;
        public IEnumerable<IAssociationEnd> AssociatedClasses
        {
            get => _associatedElements;
            set => _associatedElements = (IList<IAssociationEnd>)value;
        }

        public IEnumerable<IAssociation> OwnedAssociations
        {
            get { return AssociatedClasses.Select(x => x.Association).Distinct().Where(x => Equals(x.SourceEnd.Class, this)); }
        }

        public static bool operator ==(ClassModel lhs, ClassModel rhs)
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

        public static bool operator !=(ClassModel lhs, ClassModel rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(ClassModel other)
        {
            return string.Equals(Id, other?.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ClassModel)obj);
        }

        public override int GetHashCode()
        {
            return Id != null ? Id.GetHashCode() : 0;
        }
    }
}