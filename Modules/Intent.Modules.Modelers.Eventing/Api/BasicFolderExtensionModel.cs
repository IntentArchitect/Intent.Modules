using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class BasicFolderExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BasicFolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<TypeDefinitionModel> Types => _element.ChildElements
            .GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();

        public IList<EnumModel> Enums => _element.ChildElements
            .GetElementsOfType(EnumModel.SpecializationTypeId)
            .Select(x => new EnumModel(x))
            .ToList();

    }
}