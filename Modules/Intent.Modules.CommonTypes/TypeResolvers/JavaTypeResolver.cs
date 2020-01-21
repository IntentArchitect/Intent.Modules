using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.TypeResolvers;

namespace Intent.Modules.Common.Types.TypeResolvers
{
    public class JavaTypeResolver : TypeResolverBase, ITypeResolver
    {
        public override string DefaultCollectionFormat { get; set; } = "{0}[]";

        protected override string ResolveType(ITypeReference typeInfo, string collectionFormat = null)
        {
            var result = typeInfo.Element.Name;
            if (typeInfo.Element.HasStereotype("Java"))
            {
                string typeName = typeInfo.Element.GetStereotypeProperty<string>("Java", "Type");
                string @namespace = typeInfo.Element.GetStereotypeProperty<string>("Java", "Namespace");
                result = !string.IsNullOrWhiteSpace(@namespace) ? $"{@namespace}.{typeName}" : typeName;
            }

            return result;
        }
    }
}
