using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Services.CQRS.Api
{
    [IntentManaged(Mode.Merge)]
    public class ServicesPackageExtensionModel : ServicesPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public ServicesPackageExtensionModel(IPackage package) : base(package)
        {
        }

        public IList<CommandModel> Commands => UnderlyingPackage.ChildElements
            .GetElementsOfType(CommandModel.SpecializationTypeId)
            .Select(x => new CommandModel(x))
            .ToList();

        public IList<QueryModel> Queries => UnderlyingPackage.ChildElements
            .GetElementsOfType(QueryModel.SpecializationTypeId)
            .Select(x => new QueryModel(x))
            .ToList();

    }
}