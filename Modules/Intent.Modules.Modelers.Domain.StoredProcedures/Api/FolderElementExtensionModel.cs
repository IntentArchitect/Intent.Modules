using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain.StoredProcedures.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class FolderElementExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FolderElementExtensionModel(IElement element) : base(element)
        {
        }

        public IList<StoredProcedureModel> StoredProcedures => _element.ChildElements
            .GetElementsOfType(StoredProcedureModel.SpecializationTypeId)
            .Select(x => new StoredProcedureModel(x))
            .ToList();

    }
}