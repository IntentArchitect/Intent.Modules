using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    [IntentManaged(Mode.Merge)]
    public class ProducerModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Producer";
        public const string SpecializationTypeId = "90c0e85e-54cf-4d6b-b69f-0440e4ace079";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ProducerModel(IElement element, string requiredType = SpecializationType)
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

        public IList<PublishMessageModel> PublishMessages => _element.ChildElements
            .GetElementsOfType(PublishMessageModel.SpecializationTypeId)
            .Select(x => new PublishMessageModel(x))
            .ToList();

        public IList<SendMessageModel> SendMessages => _element.ChildElements
            .GetElementsOfType(SendMessageModel.SpecializationTypeId)
            .Select(x => new SendMessageModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ProducerModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProducerModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}