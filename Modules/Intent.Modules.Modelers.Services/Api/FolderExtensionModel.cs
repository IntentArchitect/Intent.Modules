using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Merge)]
    public class FolderExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Ignore)]
        public FolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<ServiceModel> Services => _element.ChildElements
            .Where(x => x.SpecializationType == ServiceModel.SpecializationType)
            .Select(x => new ServiceModel(x))
            .ToList();

        public IList<DTOModel> DTOs => _element.ChildElements
            .Where(x => x.SpecializationType == DTOModel.SpecializationType)
            .Select(x => new DTOModel(x))
            .ToList();

        public IList<TypeDefinitionModel> Types => _element.ChildElements
            .Where(x => x.SpecializationType == TypeDefinitionModel.SpecializationType)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();

        public IList<EnumModel> Enums => _element.ChildElements
            .Where(x => x.SpecializationType == EnumModel.SpecializationType)
            .Select(x => new EnumModel(x))
            .ToList();

    }
}