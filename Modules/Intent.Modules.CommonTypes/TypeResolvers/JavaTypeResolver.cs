using Intent.MetaModel.Common;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Types.TypeResolvers;

namespace Intent.Modules.CommonTypes.TypeResolvers
{
    public class JavaTypeResolver : TypeResolverBase, ITypeResolver
    {

        protected override string ResolveType(ITypeReference typeInfo)
        {
            var result = typeInfo.Name;
            if (typeInfo.Stereotypes.Any(x => x.Name == "Java"))
            {
                string typeName = typeInfo.GetStereotypeProperty<string>("Java", "Type");
                string @namespace = typeInfo.GetStereotypeProperty<string>("Java", "Namespace");
                result = !string.IsNullOrWhiteSpace(@namespace) ? $"{@namespace}.{typeName}" : typeName;
            }

            return result;
        }
    }
}
