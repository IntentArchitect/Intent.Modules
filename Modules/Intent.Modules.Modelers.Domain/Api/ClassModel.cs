using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modelers.Domain.Api
{
    public static class GeneralizationAssociationExtensions
    {
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class ClassModel : IHasStereotypes, IMetadataModel, IHasFolder, IHasFolder<IFolder>, IHasName
    {
        public const string SpecializationType = "Class";
        public const string SpecializationTypeId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10";

        private IList<AssociationEndModel> _associatedElements;
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ClassModel(IElement element, string requiredType = SpecializationType)
        {
            _element = element;
            Folder = element.ParentElement?.SpecializationType == FolderModel.SpecializationType ? new FolderModel(element.ParentElement) : null;

            _associatedElements = this.AssociatedToClasses().Cast<AssociationEndModel>()
                .Concat(this.AssociatedFromClasses())
                .ToList();
        }

        public string UniqueKey => $"{_element.Application.Id}_{Id}";

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public FolderModel Folder { get; }
        IFolder IHasFolder<IFolder>.Folder => Folder;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;
        public bool IsAbstract => _element.IsAbstract;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);

        public ClassModel ParentClass => this.Generalizations().Select(x => new ClassModel((IElement)x.Element)).SingleOrDefault();

        public ITypeReference ParentClassTypeReference => this.Generalizations().SingleOrDefault()?.TypeReference;
      
        public IEnumerable<ClassModel> ChildClasses => this.Specializations().Select(x => new ClassModel((IElement)x.Element)).ToList();

        public string Comment => _element.Comment;

        public IElementApplication Application => _element.Application;

        [IntentManaged(Mode.Fully)]
        public IList<ClassConstructorModel> Constructors => _element.ChildElements
            .GetElementsOfType(ClassConstructorModel.SpecializationTypeId)
            .Select(x => new ClassConstructorModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<AttributeModel> Attributes => _element.ChildElements
            .GetElementsOfType(AttributeModel.SpecializationTypeId)
            .Select(x => new AttributeModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<OperationModel> Operations => _element.ChildElements
            .GetElementsOfType(OperationModel.SpecializationTypeId)
            .Select(x => new OperationModel(x))
            .ToList();

        public bool IsSubclassOf(ClassModel @class)
        {
            return GetTypesInHierarchy().Any(x => x.Equals(@class));
        }

        public bool IsSuperclassOf(ClassModel @class)
        {
            return @class.GetTypesInHierarchy().Any(x => x.Equals(this));
        }

        public IEnumerable<ClassModel> GetTypesInHierarchy()
        {
            yield return this;
            var parent = ParentClass;
            while (parent != null)
            {
                yield return parent;
                parent = parent.ParentClass;
            }
        }

        public IEnumerable<AssociationEndModel> AssociatedClasses
        {
            get => _associatedElements;
            set => _associatedElements = (IList<AssociationEndModel>)value;
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

        [IntentManaged(Mode.Fully)]
        public bool Equals(ClassModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClassModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;
    }

    [IntentManaged(Mode.Fully)]
    public static class ClassModelExtensions
    {

        public static bool IsClassModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ClassModel.SpecializationTypeId;
        }

        public static ClassModel AsClassModel(this ICanBeReferencedType type)
        {
            return type.IsClassModel() ? new ClassModel((IElement)type) : null;
        }
    }
}