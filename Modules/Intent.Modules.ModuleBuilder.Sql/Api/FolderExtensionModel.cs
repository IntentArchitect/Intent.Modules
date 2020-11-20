using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Sql.Api
{
    [IntentManaged(Mode.Merge)]
    public class FolderExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Ignore)]
        public FolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<SqlTemplateModel> SqlTemplates => _element.ChildElements
            .Where(x => x.SpecializationType == SqlTemplateModel.SpecializationType)
            .Select(x => new SqlTemplateModel(x))
            .ToList();

    }
}