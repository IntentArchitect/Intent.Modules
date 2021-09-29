using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    [IntentManaged(Mode.Merge)]
    public class CQRSServiceModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "CQRS Service";
        public const string SpecializationTypeId = "3a2ed4aa-0cf0-4e99-9dfa-a0a34c63214d";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public CQRSServiceModel(IElement element, string requiredType = SpecializationType)
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

        public IList<CommandModel> Commands => _element.ChildElements
            .GetElementsOfType(CommandModel.SpecializationTypeId)
            .Select(x => new CommandModel(x))
            .ToList();

        public IList<QueryModel> Queries => _element.ChildElements
            .GetElementsOfType(QueryModel.SpecializationTypeId)
            .Select(x => new QueryModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(CQRSServiceModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CQRSServiceModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CQRSServiceModelExtensions
    {

        public static bool IsCQRSServiceModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == CQRSServiceModel.SpecializationTypeId;
        }

        public static CQRSServiceModel AsCQRSServiceModel(this ICanBeReferencedType type)
        {
            return type.IsCQRSServiceModel() ? new CQRSServiceModel((IElement)type) : null;
        }
    }
}