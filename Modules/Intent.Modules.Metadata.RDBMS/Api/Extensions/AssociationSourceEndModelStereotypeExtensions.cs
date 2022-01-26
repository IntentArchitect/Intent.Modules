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

    public static class AssociationSourceEndModelStereotypeExtensions
    {
        public static ForeignKey GetForeignKey(this AssociationSourceEndModel model)
        {
            var stereotype = model.GetStereotype("Foreign Key");
            return stereotype != null ? new ForeignKey(stereotype) : null;
        }

        public static IReadOnlyCollection<ForeignKey> GetForeignKeys(this AssociationSourceEndModel model)
        {
            var stereotypes = model
                .GetStereotypes("Foreign Key")
                .Select(stereotype => new ForeignKey(stereotype))
                .ToArray();

            return stereotypes;
        }

        public static bool HasForeignKey(this AssociationSourceEndModel model)
        {
            return model.HasStereotype("Foreign Key");
        }


        public class ForeignKey
        {
            private IStereotype _stereotype;

            public ForeignKey(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string ColumnName()
            {
                return _stereotype.GetProperty<string>("Column Name");
            }

        }

    }
}