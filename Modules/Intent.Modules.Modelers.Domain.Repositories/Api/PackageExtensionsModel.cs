using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Repositories.Api
{
    [IntentManaged(Mode.Merge)]
    public class PackageExtensionsModel : DomainPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public PackageExtensionsModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<RepositoryModel> Repositories => UnderlyingPackage.ChildElements
            .GetElementsOfType(RepositoryModel.SpecializationTypeId)
            .Select(x => new RepositoryModel(x))
            .ToList();

    }
}