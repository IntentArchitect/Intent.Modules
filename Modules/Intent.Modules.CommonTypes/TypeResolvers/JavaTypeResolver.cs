using Intent.MetaModel.Common;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.CommonTypes.TypeResolvers
{
    public class JavaTypeResolver : TypeResolverBase, ITypeResolver
    {

        protected override string ResolveType(ITypeReference typeInfo)
        {
            var result = typeInfo.Name;
            if (typeInfo.Stereotypes.Any(x => x.Name == "Java"))
            {
                string typeName = typeInfo.Stereotypes.GetPropertyValue<string>("Java", "Type");
                string @namespace = typeInfo.Stereotypes.GetPropertyValue<string>("Java", "Namespace");
                if (!string.IsNullOrWhiteSpace(@namespace))
                {
                    result = $"{@namespace}.{typeName}";
                }
                else
                {
                    result = typeName;
                }
            }

            return result;
        }
    }
}
