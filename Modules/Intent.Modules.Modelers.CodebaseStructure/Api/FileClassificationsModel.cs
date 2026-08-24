using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.CodebaseStructure.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class FileClassificationsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "File Classifications";
        public const string SpecializationTypeId = "81b0c54f-8f28-47f0-b0c1-6e8fe1212f50";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public FileClassificationsModel(IElement element, string requiredType = SpecializationTypeId)
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

        public IElement InternalElement => _element;

        public IList<FileClassificationModel> FileClassifications => _element.ChildElements
            .GetElementsOfType(FileClassificationModel.SpecializationTypeId)
            .Select(x => new FileClassificationModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(FileClassificationsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileClassificationsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class FileClassificationsModelExtensions
    {

        public static bool IsFileClassificationsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == FileClassificationsModel.SpecializationTypeId;
        }

        public static FileClassificationsModel AsFileClassificationsModel(this ICanBeReferencedType type)
        {
            return type.IsFileClassificationsModel() ? new FileClassificationsModel((IElement)type) : null;
        }
    }
}