using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DesignersFolderModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Designers Folder";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public DesignersFolderModel(IElement element, string requiredType = SpecializationType)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'DesignersFolderModel' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
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
        public IList<DesignerModel> Designers => _element.ChildElements
            .GetElementsOfType(DesignerModel.SpecializationTypeId)
            .Select(x => new DesignerModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public bool Equals(DesignersFolderModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DesignersFolderModel)obj);
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
        public IList<DesignerSettingsModel> DesignerSettings => _element.ChildElements
            .GetElementsOfType(DesignerSettingsModel.SpecializationTypeId)
            .Select(x => new DesignerSettingsModel(x))
            .ToList();
        public const string SpecializationTypeId = "dbb71aa5-0db2-4a2a-97a9-501950f36f99";

        public string Comment => _element.Comment;
    }
}