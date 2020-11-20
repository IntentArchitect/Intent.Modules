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
    public class ModelersFolder
    {
        public const string SpecializationType = "Modelers Folder";

        private readonly IElement _element;

        public ModelersFolder(IElement element, string requiredType = SpecializationType)
        {
            if (element.SpecializationType != SpecializationType) 
            {
                throw new ArgumentException($"Invalid element type {element.SpecializationType}", nameof(element));
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

        protected bool Equals(ModelersFolder other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModelersFolder)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}