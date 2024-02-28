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

namespace Intent.ModuleBuilder.Kotlin.Api
{
    [IntentManaged(Mode.Merge)]
    public class KotlinFileTemplateModel : TemplateRegistrationModel, IMetadataModel, IHasStereotypes, IHasName, IHasFolder, IElementWrapper
    {
        public new const string SpecializationType = "Kotlin File Template";
        public new const string SpecializationTypeId = "fd5375d4-5380-4a04-b164-0617a8d64624";

        public KotlinFileTemplateModel(IElement element) : base(element, SpecializationType)
        {
        }

        public TemplateDecoratorContractModel DecoratorContract => _element.ChildElements
            .GetElementsOfType(TemplateDecoratorContractModel.SpecializationTypeId)
            .Select(x => new TemplateDecoratorContractModel(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(KotlinFileTemplateModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((KotlinFileTemplateModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class KotlinFileTemplateModelExtensions
    {

        public static bool IsKotlinFileTemplateModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == KotlinFileTemplateModel.SpecializationTypeId;
        }

        public static KotlinFileTemplateModel AsKotlinFileTemplateModel(this ICanBeReferencedType type)
        {
            return type.IsKotlinFileTemplateModel() ? new KotlinFileTemplateModel((IElement)type) : null;
        }
    }
}