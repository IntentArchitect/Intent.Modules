using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.ModuleBuilder.TypeScript.Api
{
    [IntentManaged(Mode.Merge)]
    public class FolderExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Ignore)]
        public FolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<TypescriptFileTemplateModel> TypescriptTemplates => _element.ChildElements
            .Where(x => x.SpecializationType == TypescriptFileTemplateModel.SpecializationType)
            .Select(x => new TypescriptFileTemplateModel(x))
            .ToList();

    }
}