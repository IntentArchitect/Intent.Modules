using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class MappableElementReferenceModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Mappable Element Reference";
        public const string SpecializationTypeId = "072515e1-3a1e-4472-a27e-e322a683d299";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public MappableElementReferenceModel(IElement element, string requiredType = SpecializationType)
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

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(MappableElementReferenceModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappableElementReferenceModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentIgnore]
        public MappableElementSettingIdentifierPersistable ToPersistable()
        {
            return new MappableElementSettingIdentifierPersistable()
            {
                Id = this.GetMappableReferenceSettings().ReferenceTarget().Id,
                Name = this.GetMappableReferenceSettings().ReferenceTarget().Name,
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MappableElementReferenceModelExtensions
    {

        public static bool IsMappableElementReferenceModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == MappableElementReferenceModel.SpecializationTypeId;
        }

        public static MappableElementReferenceModel AsMappableElementReferenceModel(this ICanBeReferencedType type)
        {
            return type.IsMappableElementReferenceModel() ? new MappableElementReferenceModel((IElement)type) : null;
        }
    }
}