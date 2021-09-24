using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.CSharp.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CSharpTemplateModel : TemplateRegistrationModel, IMetadataModel, IHasStereotypes, IHasName, IHasFolder
    {
        public new const string SpecializationType = "C# Template";

        public CSharpTemplateModel(IElement element) : base(element, SpecializationType)
        {
        }


        [IntentManaged(Mode.Fully)]
        public bool Equals(CSharpTemplateModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CSharpTemplateModel)obj);
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
        public new const string SpecializationTypeId = "f6456232-0f1b-4235-b5f8-b4cce548ca59";

        public TemplateDecoratorContractModel DecoratorContract => _element.ChildElements
                    .Where(x => x.SpecializationType == TemplateDecoratorContractModel.SpecializationType)
                    .Select(x => new TemplateDecoratorContractModel(x))
                    .SingleOrDefault();
    }

    [IntentManaged(Mode.Fully)]
    public static class CSharpTemplateModelExtensions
    {

        public static bool IsCSharpTemplateModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == CSharpTemplateModel.SpecializationTypeId;
        }

        public static CSharpTemplateModel AsCSharpTemplateModel(this ICanBeReferencedType type)
        {
            return type.IsCSharpTemplateModel() ? new CSharpTemplateModel((IElement)type) : null;
        }
    }
}