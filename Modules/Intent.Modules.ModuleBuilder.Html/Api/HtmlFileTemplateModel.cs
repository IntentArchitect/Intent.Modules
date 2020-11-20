using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Html.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class HtmlFileTemplateModel : TemplateRegistrationModel, IMetadataModel, IHasStereotypes, IHasName
    {
        public new const string SpecializationType = "Html File Template";

        public HtmlFileTemplateModel(IElement element) : base(element, SpecializationType)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(HtmlFileTemplateModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HtmlFileTemplateModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
        public new const string SpecializationTypeId = "af3fe721-86c5-41e3-9daf-1e79209ed345";
    }
}