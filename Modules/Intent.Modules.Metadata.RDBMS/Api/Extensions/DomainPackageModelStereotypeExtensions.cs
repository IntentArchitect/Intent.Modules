using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{
    public static class DomainPackageModelStereotypeExtensions
    {
        public static RelationalDatabase GetRelationalDatabase(this DomainPackageModel model)
        {
            var stereotype = model.GetStereotype("Relational Database");
            return stereotype != null ? new RelationalDatabase(stereotype) : null;
        }


        [IntentManaged(Mode.Ignore)]
        public static bool HasRelationalDatabase(this DomainPackageModel model)
        {
            // For backward compatibility with < 3.5.0. Can safely be removed next year (2024) as all users will (almost) certainly have upgraded to the new paradigm by then.
            return model.HasStereotype("Relational Database") || !model.Stereotypes.Any() || model.HasStereotype("Cosmos DB Container Settings");
        }

        public static bool TryGetRelationalDatabase(this DomainPackageModel model, out RelationalDatabase stereotype)
        {
            if (!HasRelationalDatabase(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new RelationalDatabase(model.GetStereotype("Relational Database"));
            return true;
        }

        public class RelationalDatabase
        {
            private IStereotype _stereotype;

            public RelationalDatabase(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}