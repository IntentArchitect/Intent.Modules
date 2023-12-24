using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FileTemplateModel : TemplateRegistrationModel, IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
    {
        public FileTemplateModel(IElement element) : base(element, SpecializationType)
        {
        }


        [IntentManaged(Mode.Fully)]
        public bool Equals(FileTemplateModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileTemplateModel)obj);
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

        public new const string SpecializationType = "File Template";
        public new const string SpecializationTypeId = "43eae4bd-4613-4d15-88ac-52b7e743b7b2";

        public IList<TemplateDecoratorContractModel> DecoratorContracts => _element.ChildElements
                    .GetElementsOfType(TemplateDecoratorContractModel.SpecializationTypeId)
                    .Select(x => new TemplateDecoratorContractModel(x))
                    .ToList();

        public TemplateDecoratorContractModel DecoratorContract => _element.ChildElements
            .GetElementsOfType(TemplateDecoratorContractModel.SpecializationTypeId)
            .Select(x => new TemplateDecoratorContractModel(x))
            .SingleOrDefault();
    }

    [IntentManaged(Mode.Fully)]
    public static class FileTemplateModelExtensions
    {

        public static bool IsFileTemplateModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == FileTemplateModel.SpecializationTypeId;
        }

        public static FileTemplateModel AsFileTemplateModel(this ICanBeReferencedType type)
        {
            return type.IsFileTemplateModel() ? new FileTemplateModel((IElement)type) : null;
        }
    }
}