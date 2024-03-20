using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain.StoredProcedures.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class StoredProcedureModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
    {
        public const string SpecializationType = "Stored Procedure";
        public const string SpecializationTypeId = "575edd35-9438-406d-b0a7-b99d6f29b560";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public StoredProcedureModel(IElement element, string requiredType = SpecializationType)
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

        public ITypeReference TypeReference => _element.TypeReference;

        public ITypeReference ReturnType => TypeReference?.Element != null ? TypeReference : null;

        public IElement InternalElement => _element;

        public IList<StoredProcedureParameterModel> Parameters => _element.ChildElements
            .GetElementsOfType(StoredProcedureParameterModel.SpecializationTypeId)
            .Select(x => new StoredProcedureParameterModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(StoredProcedureModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StoredProcedureModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class StoredProcedureModelExtensions
    {

        public static bool IsStoredProcedureModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == StoredProcedureModel.SpecializationTypeId;
        }

        public static StoredProcedureModel AsStoredProcedureModel(this ICanBeReferencedType type)
        {
            return type.IsStoredProcedureModel() ? new StoredProcedureModel((IElement)type) : null;
        }
    }
}