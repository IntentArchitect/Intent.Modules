using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.CSharp.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class RazorTemplateModel : TemplateRegistrationModel, IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
    {
        public new const string SpecializationType = "Razor Template";
        public new const string SpecializationTypeId = "05162e94-598b-416c-9779-d42b0b5262a1";

        public RazorTemplateModel(IElement element) : base(element, SpecializationType)
        {
        }

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(RazorTemplateModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RazorTemplateModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class RazorTemplateModelExtensions
    {

        public static bool IsRazorTemplateModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == RazorTemplateModel.SpecializationTypeId;
        }

        public static RazorTemplateModel AsRazorTemplateModel(this ICanBeReferencedType type)
        {
            return type.IsRazorTemplateModel() ? new RazorTemplateModel((IElement)type) : null;
        }
    }
}