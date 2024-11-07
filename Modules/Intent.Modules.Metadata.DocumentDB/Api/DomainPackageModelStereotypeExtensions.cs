using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.DocumentDB.Api
{
    public static class DomainPackageModelStereotypeExtensions
    {
        public static DocumentDatabase GetDocumentDatabase(this DomainPackageModel model)
        {
            var stereotype = model.GetStereotype(DocumentDatabase.DefinitionId);
            return stereotype != null ? new DocumentDatabase(stereotype) : null;
        }


        public static bool HasDocumentDatabase(this DomainPackageModel model)
        {
            return model.HasStereotype(DocumentDatabase.DefinitionId);
        }

        public static bool TryGetDocumentDatabase(this DomainPackageModel model, out DocumentDatabase stereotype)
        {
            if (!HasDocumentDatabase(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DocumentDatabase(model.GetStereotype(DocumentDatabase.DefinitionId));
            return true;
        }

        public class DocumentDatabase
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "8b68020c-6652-484b-85e8-6c33e1d8031f";

            public DocumentDatabase(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement Provider()
            {
                return _stereotype.GetProperty<IElement>("Provider");
            }

        }

    }
}