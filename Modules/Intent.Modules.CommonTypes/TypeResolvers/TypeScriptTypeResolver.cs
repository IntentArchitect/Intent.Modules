
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Common;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.CommonTypes.TypeResolvers
{
    public class TypeScriptTypeResolver : TypeResolverBase, ITypeResolver
    {
        protected override string ResolveType(ITypeReference typeInfo)
        {
            if (typeInfo.HasStereotype("TypeScript"))
            {
                return typeInfo.GetStereotypeProperty<string>("TypeScript", "Type");
            }

            return typeInfo.Name;
        }

    }
}
