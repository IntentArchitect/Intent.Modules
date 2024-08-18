using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Merge)]
    public class ComponentOperationModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference, IProcessingHandlerModel
    {
        public const string SpecializationType = "Component Operation";
        public const string SpecializationTypeId = "e030c97a-e066-40a7-8188-808c275df3cb";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public ComponentOperationModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);

        public ITypeReference TypeReference => _element.TypeReference;

        public ITypeReference ReturnType => TypeReference?.Element != null ? TypeReference : null;

        public IElement InternalElement => _element;

        public IList<ParameterModel> Parameters => _element.ChildElements
            .GetElementsOfType(ParameterModel.SpecializationTypeId)
            .Select(x => new ParameterModel(x))
            .ToList();

        public IList<InvocationModel> Invocations => _element.ChildElements
            .GetElementsOfType(InvocationModel.SpecializationTypeId)
            .Select(x => new InvocationModel(x))
            .ToList();

        public IList<VariableDeclarationModel> Variables => _element.ChildElements
            .GetElementsOfType(VariableDeclarationModel.SpecializationTypeId)
            .Select(x => new VariableDeclarationModel(x))
            .ToList();

        public ReturnModel Return => _element.ChildElements
            .GetElementsOfType(ReturnModel.SpecializationTypeId)
            .Select(x => new ReturnModel(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ComponentOperationModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComponentOperationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ComponentOperationModelExtensions
    {

        public static bool IsComponentOperationModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ComponentOperationModel.SpecializationTypeId;
        }

        public static ComponentOperationModel AsComponentOperationModel(this ICanBeReferencedType type)
        {
            return type.IsComponentOperationModel() ? new ComponentOperationModel((IElement)type) : null;
        }
    }
}