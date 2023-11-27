using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OperationModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
    {
        public const string SpecializationType = "Operation";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public OperationModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;

        public bool IsMapped => _element.IsMapped;

        public IElementMapping Mapping => _element.MappedElement;

        public ITypeReference ReturnType => TypeReference?.Element != null ? TypeReference : null;

        public bool IsAbstract => _element.IsAbstract;
        public bool IsStatic => _element.IsStatic;

        [IntentManaged(Mode.Ignore)] public ClassModel ParentClass => new ClassModel(InternalElement.ParentElement);

        [IntentManaged(Mode.Fully)]
        public IList<ParameterModel> Parameters => _element.ChildElements
            .GetElementsOfType(ParameterModel.SpecializationTypeId)
            .Select(x => new ParameterModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public bool Equals(OperationModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OperationModel)obj);
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

        [IntentManaged(Mode.Fully)]
        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);
        public const string SpecializationTypeId = "e042bb67-a1df-480c-9935-b26210f78591";

        public string Comment => _element.Comment;
    }

    [IntentManaged(Mode.Fully)]
    public static class OperationModelExtensions
    {

        public static bool IsOperationModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == OperationModel.SpecializationTypeId;
        }

        public static OperationModel AsOperationModel(this ICanBeReferencedType type)
        {
            return type.IsOperationModel() ? new OperationModel((IElement)type) : null;
        }

        public static bool HasMapOperationMapping(this OperationModel type)
        {
            return type.Mapping?.MappingSettingsId == "7590bd83-04bc-40fe-8b5c-118277c049d4";
        }

        public static IElementMapping GetMapOperationMapping(this OperationModel type)
        {
            return type.HasMapOperationMapping() ? type.Mapping : null;
        }
    }
}