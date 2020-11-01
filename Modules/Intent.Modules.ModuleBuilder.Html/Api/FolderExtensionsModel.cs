using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Html.Api
{
    [IntentManaged(Mode.Merge)]
    public class FolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Ignore)]
        public FolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<HtmlFileTemplateModel> HtmlFiles => _element.ChildElements
            .Where(x => x.SpecializationType == HtmlFileTemplateModel.SpecializationType)
            .Select(x => new HtmlFileTemplateModel(x))
            .ToList();

    }
}