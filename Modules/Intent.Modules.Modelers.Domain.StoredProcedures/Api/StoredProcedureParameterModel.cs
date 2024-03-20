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
    public class StoredProcedureParameterModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
    {
        public const string SpecializationType = "Stored Procedure Parameter";
        public const string SpecializationTypeId = "5823b192-eb03-47c8-90d8-5501c922e9a5";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public StoredProcedureParameterModel(IElement element, string requiredType = SpecializationType)
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


        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(StoredProcedureParameterModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StoredProcedureParameterModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class StoredProcedureParameterModelExtensions
    {

        public static bool IsStoredProcedureParameterModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == StoredProcedureParameterModel.SpecializationTypeId;
        }

        public static StoredProcedureParameterModel AsStoredProcedureParameterModel(this ICanBeReferencedType type)
        {
            return type.IsStoredProcedureParameterModel() ? new StoredProcedureParameterModel((IElement)type) : null;
        }
    }
}