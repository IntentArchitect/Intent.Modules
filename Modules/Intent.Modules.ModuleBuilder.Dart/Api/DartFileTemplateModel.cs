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

namespace Intent.Modules.ModuleBuilder.Dart.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DartFileTemplateModel : TemplateRegistrationModel, IMetadataModel, IHasStereotypes, IHasName, IHasFolder
    {
        public new const string SpecializationType = "Dart File Template";
        public new const string SpecializationTypeId = "755fcfd3-6232-4ed8-9bfc-2d09af503697";

        public DartFileTemplateModel(IElement element) : base(element, SpecializationType)
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

        public bool Equals(DartFileTemplateModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DartFileTemplateModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DartFileTemplateModelExtensions
    {

        public static bool IsDartFileTemplateModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DartFileTemplateModel.SpecializationTypeId;
        }

        public static DartFileTemplateModel AsDartFileTemplateModel(this ICanBeReferencedType type)
        {
            return type.IsDartFileTemplateModel() ? new DartFileTemplateModel((IElement)type) : null;
        }
    }
}