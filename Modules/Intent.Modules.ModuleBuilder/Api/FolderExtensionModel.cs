using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge)]
    public class FolderExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Ignore)]
        public FolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<FileTemplateModel> FileTemplates => _element.ChildElements
            .GetElementsOfType(FileTemplateModel.SpecializationTypeId)
            .Select(x => new FileTemplateModel(x))
            .ToList();

        public IList<StaticContentTemplateModel> StaticContentTemplates => _element.ChildElements
            .GetElementsOfType(StaticContentTemplateModel.SpecializationTypeId)
            .Select(x => new StaticContentTemplateModel(x))
            .ToList();

        public IList<TemplateRegistrationModel> TemplateRegistrations => _element.ChildElements
            .GetElementsOfType(TemplateRegistrationModel.SpecializationTypeId)
            .Select(x => new TemplateRegistrationModel(x))
            .ToList();

        public IList<TypeDefinitionModel> Types => _element.ChildElements
            .GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();

        public IList<EnumModel> Enums => _element.ChildElements
            .GetElementsOfType(EnumModel.SpecializationTypeId)
            .Select(x => new EnumModel(x))
            .ToList();

        public IList<FactoryExtensionModel> FactoryExtensions => _element.ChildElements
            .GetElementsOfType(FactoryExtensionModel.SpecializationTypeId)
            .Select(x => new FactoryExtensionModel(x))
            .ToList();

        public IList<TemplateDecoratorModel> TemplateDecorators => _element.ChildElements
            .GetElementsOfType(TemplateDecoratorModel.SpecializationTypeId)
            .Select(x => new TemplateDecoratorModel(x))
            .ToList();

    }
}