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
            var stereotype = model.GetStereotype("51a7bcf5-0eb9-4c9a-855e-3ead1048729c");
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

            stereotype = new RelationalDatabase(model.GetStereotype("51a7bcf5-0eb9-4c9a-855e-3ead1048729c"));
            return true;
        }

        public static Schema GetSchema(this DomainPackageModel model)
        {
            var stereotype = model.GetStereotype("c0f17219-ada3-47ac-80c6-7a5750cbd322");
            return stereotype != null ? new Schema(stereotype) : null;
        }


        public static bool HasSchema(this DomainPackageModel model)
        {
            return model.HasStereotype("c0f17219-ada3-47ac-80c6-7a5750cbd322");
        }

        public static bool TryGetSchema(this DomainPackageModel model, out Schema stereotype)
        {
            if (!HasSchema(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Schema(model.GetStereotype("c0f17219-ada3-47ac-80c6-7a5750cbd322"));
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

        public class Schema
        {
            private IStereotype _stereotype;

            public Schema(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

        }

    }
}