using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    [IntentManaged(Mode.Merge)]
    public class CommandModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IHasFolder
    {
        public const string SpecializationType = "Command";
        public const string SpecializationTypeId = "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public CommandModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Folder = _element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId ? new FolderModel(_element.ParentElement) : null;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public ITypeReference TypeReference => _element.TypeReference;

        public IElement InternalElement => _element;

        [IntentManaged(Mode.Ignore)]
        public string GetConceptName()
        {
            return Name.RemoveSuffix("Command");
        }

        public IList<DTOFieldModel> Properties => _element.ChildElements
            .GetElementsOfType(DTOFieldModel.SpecializationTypeId)
            .Select(x => new DTOFieldModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(CommandModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CommandModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public string Comment => _element.Comment;

        public FolderModel Folder { get; }

        public bool IsMapped => _element.IsMapped;

        public IElementMapping Mapping => _element.MappedElement;
    }

    [IntentManaged(Mode.Fully)]
    public static class CommandModelExtensions
    {

        public static bool IsCommandModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == CommandModel.SpecializationTypeId;
        }

        public static CommandModel AsCommandModel(this ICanBeReferencedType type)
        {
            return type.IsCommandModel() ? new CommandModel((IElement)type) : null;
        }

        public static bool HasMapToDomainDataMapping(this CommandModel type)
        {
            return type.Mapping?.MappingSettingsId == "735c87d0-06fc-4491-8b5f-5adc6f953c54";
        }

        public static IElementMapping GetMapToDomainDataMapping(this CommandModel type)
        {
            return type.HasMapToDomainDataMapping() ? type.Mapping : null;
        }

        public static bool HasMapToDomainOperationMapping(this CommandModel type)
        {
            return type.Mapping?.MappingSettingsId == "7c31c459-6229-4f10-bf13-507348cd8828";
        }

        public static IElementMapping GetMapToDomainOperationMapping(this CommandModel type)
        {
            return type.HasMapToDomainOperationMapping() ? type.Mapping : null;
        }
    }
}