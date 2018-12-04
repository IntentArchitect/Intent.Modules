using Intent.MetaModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Common
{
    public static class UmlModelExtensions
    {
        public static string GetPropertyName(this IAssociationEnd associationEnd)
        {
            if (string.IsNullOrEmpty(associationEnd.Name))
            {
                var className = associationEnd.Class.Name;
                if (associationEnd.Multiplicity == Multiplicity.Many)
                {
                    return className.Pluralize(false);
                }
                return className;
            }

            return associationEnd.Name;
        }
    }
}
