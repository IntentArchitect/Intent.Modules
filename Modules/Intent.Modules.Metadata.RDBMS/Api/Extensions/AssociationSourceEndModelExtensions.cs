using System;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;

namespace Intent.Metadata.RDBMS.Api
{
    [Obsolete("Replaced by AssociationSourceEndModelStereotypeExtensions")]
    public static class AssociationSourceEndModelExtensions
    {
        [Obsolete]
        public static ForeignKey GetForeignKey(AssociationSourceEndModel model)
        {
            var stereotype = model.GetStereotype("Foreign Key");
            return stereotype != null ? new ForeignKey(stereotype) : null;
        }

        [Obsolete]
        public static bool HasForeignKey(AssociationSourceEndModel model)
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