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

namespace Intent.ModuleBuilder.Java.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class JavaFileTemplateModel : TemplateRegistrationModel, IMetadataModel, IHasStereotypes, IHasName, IHasFolder
    {
        public new const string SpecializationType = "Java File Template";

        public JavaFileTemplateModel(IElement element) : base(element, SpecializationType)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(JavaFileTemplateModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((JavaFileTemplateModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
        public new const string SpecializationTypeId = "e27af6dd-e69e-4e80-a2e1-0bbe9493cf7a";

        public IList<TemplateDecoratorContractModel> Decorators => _element.ChildElements
                    .Where(x => x.SpecializationType == TemplateDecoratorContractModel.SpecializationType)
                    .Select(x => new TemplateDecoratorContractModel(x))
                    .ToList();

        public IList<TemplateDecoratorContractModel> DecoratorContracts => _element.ChildElements
                    .Where(x => x.SpecializationType == TemplateDecoratorContractModel.SpecializationType)
                    .Select(x => new TemplateDecoratorContractModel(x))
                    .ToList();

        public TemplateDecoratorContractModel DecoratorContract => _element.ChildElements
                    .Where(x => x.SpecializationType == TemplateDecoratorContractModel.SpecializationType)
                    .Select(x => new TemplateDecoratorContractModel(x))
                    .SingleOrDefault();
    }

    [IntentManaged(Mode.Fully)]
    public static class JavaFileTemplateModelExtensions
    {

        public static bool IsJavaFileTemplateModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == JavaFileTemplateModel.SpecializationTypeId;
        }

        public static JavaFileTemplateModel AsJavaFileTemplateModel(this ICanBeReferencedType type)
        {
            return type.IsJavaFileTemplateModel() ? new JavaFileTemplateModel((IElement)type) : null;
        }
    }
}