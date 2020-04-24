using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DesignerExtensionModel : DesignerModel, IHasStereotypes, IMetadataModel
    {
        public new const string SpecializationType = "Designer Extension";

        public DesignerExtensionModel(IElement element) : base(element, SpecializationType)
        {
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(DesignerExtensionModel other)
        {
            return Equals(_element, other._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DesignerExtensionModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IList<ElementExtensionModel> ElementExtensions => _element.ChildElements
            .Where(x => x.SpecializationType == ElementExtensionModel.SpecializationType)
            .Select(x => new ElementExtensionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }
    }
}