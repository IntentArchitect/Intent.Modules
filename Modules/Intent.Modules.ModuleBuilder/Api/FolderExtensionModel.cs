using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

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

        public IList<TemplateRegistrationModel> TemplateRegistrations => _element.ChildElements
            .GetElementsOfType(TemplateRegistrationModel.SpecializationTypeId)
            .Select(x => new TemplateRegistrationModel(x))
            .ToList();

        public IList<TypeDefinitionModel> Types => _element.ChildElements
            .GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();

    }
}