using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{
    [IntentManaged(Mode.Merge)]
    public class ClassExtensionModel : ClassModel
    {
        [IntentManaged(Mode.Ignore)]
        public ClassExtensionModel(IElement element) : base(element)
        {
        }

        public IList<IndexModel> Indices => _element.ChildElements
            .GetElementsOfType(IndexModel.SpecializationTypeId)
            .Select(x => new IndexModel(x))
            .ToList();

        public IList<IndexModel> Indexes => _element.ChildElements
            .GetElementsOfType(IndexModel.SpecializationTypeId)
            .Select(x => new IndexModel(x))
            .ToList();
    }
}