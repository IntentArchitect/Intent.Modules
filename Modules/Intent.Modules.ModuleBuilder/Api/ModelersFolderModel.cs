using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ModelersFolderModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Modelers Folder";
        private readonly IElement _element;

        public ModelersFolderModel(IElement element, string requiredType = SpecializationType)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'ModelersFolderModel' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public IList<DesignerSettingsModel> Modelers => _element.ChildElements
            .Where(x => x.SpecializationType == Api.DesignerSettingsModel.SpecializationType)
            .Select(x => new DesignerSettingsModel(x))
            .ToList<DesignerSettingsModel>();

        protected bool Equals(ModelersFolderModel other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModelersFolderModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}