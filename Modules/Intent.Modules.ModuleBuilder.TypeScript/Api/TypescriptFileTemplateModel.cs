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

namespace Intent.ModuleBuilder.TypeScript.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class TypescriptFileTemplateModel : TemplateRegistrationModel, IMetadataModel, IHasStereotypes, IHasName, IHasFolder
    {
        public new const string SpecializationType = "Typescript File Template";
        public new const string SpecializationTypeId = "e6e41d88-2829-41b6-a791-065d2bb44eb3";

        public TypescriptFileTemplateModel(IElement element) : base(element, SpecializationType)
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

        public bool Equals(TypescriptFileTemplateModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypescriptFileTemplateModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class TypescriptFileTemplateModelExtensions
    {

        public static bool IsTypescriptFileTemplateModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == TypescriptFileTemplateModel.SpecializationTypeId;
        }

        public static TypescriptFileTemplateModel AsTypescriptFileTemplateModel(this ICanBeReferencedType type)
        {
            return type.IsTypescriptFileTemplateModel() ? new TypescriptFileTemplateModel((IElement)type) : null;
        }
    }
}