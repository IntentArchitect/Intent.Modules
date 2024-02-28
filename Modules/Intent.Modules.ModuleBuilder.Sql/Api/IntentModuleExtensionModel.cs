using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Sql.Api
{
    [IntentManaged(Mode.Merge)]
    public class IntentModuleExtensionModel : IntentModuleModel
    {
        [IntentManaged(Mode.Ignore)]
        public IntentModuleExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<SqlTemplateModel> SqlTemplates => UnderlyingPackage.ChildElements
            .GetElementsOfType(SqlTemplateModel.SpecializationTypeId)
            .Select(x => new SqlTemplateModel(x))
            .ToList();

    }
}