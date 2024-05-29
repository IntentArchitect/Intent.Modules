using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.ModuleBuilder.CSharp.Api
{
    [IntentManaged(Mode.Merge)]
    public class FolderExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Ignore)]
        public FolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<CSharpTemplateModel> CSharpTemplates => _element.ChildElements
            .GetElementsOfType(CSharpTemplateModel.SpecializationTypeId)
            .Select(x => new CSharpTemplateModel(x))
            .ToList();

        public IList<RazorTemplateModel> RazorTemplates => _element.ChildElements
            .GetElementsOfType(RazorTemplateModel.SpecializationTypeId)
            .Select(x => new RazorTemplateModel(x))
            .ToList();

    }
}