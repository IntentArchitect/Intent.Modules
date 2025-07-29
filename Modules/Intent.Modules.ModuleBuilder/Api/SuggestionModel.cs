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
    public class SuggestionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
    {
        public const string SpecializationType = "Suggestion";
        public const string SpecializationTypeId = "0c21ab10-e87b-4e88-ab44-38ea7adf514a";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public SuggestionModel(IElement element, string requiredType = SpecializationTypeId)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
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

        public bool Equals(SuggestionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SuggestionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentIgnore]
        public SuggestionSettingPersistable ToPersistable()
        {
            return new SuggestionSettingPersistable()
            {
                Name = Name,
                SpecializationType = TypeReference.Element.Name,
                SpecializationTypeId = TypeReference.Element.Id,
                DisplayFunction = this.GetSettings().DisplayFunction(),
                FilterFunction = this.GetSettings().FilterFunction(),
                Icon = this.GetSettings().Icon().ToPersistable(),
                OrderPriority = this.GetSettings().OrderPriority() ?? 0,
                Locations = string.Join(", ", this.GetSettings().Locations().Select(x => x.Value.ToLower())),
                Dependencies = this.GetSettings().Dependencies()?.Select(x => new TargetReferencePersistable() { Id = x.Id, Name = x.Name }).ToList() ?? [],
                Script = this.GetSettings().Script()
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class SuggestionModelExtensions
    {

        public static bool IsSuggestionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == SuggestionModel.SpecializationTypeId;
        }

        public static SuggestionModel AsSuggestionModel(this ICanBeReferencedType type)
        {
            return type.IsSuggestionModel() ? new SuggestionModel((IElement)type) : null;
        }
    }
}