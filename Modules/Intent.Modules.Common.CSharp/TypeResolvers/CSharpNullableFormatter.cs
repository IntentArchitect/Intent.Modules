using System;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.TypeResolvers
{
    public class CSharpNullableFormatter : INullableFormatter
    {
        private readonly ICSharpProject _project;

        public CSharpNullableFormatter(ICSharpProject project)
        {
            _project = project;
        }

        public string AsNullable(IResolvedTypeInfo typeInfo)
        {
            if (typeInfo.IsNullable && !IsInterface(typeInfo) &&
                (typeInfo.IsPrimitive || 
                 _project.IsNullableAwareContext() || 
                 typeInfo.TypeReference.Element.SpecializationType.Equals("Enum", StringComparison.InvariantCultureIgnoreCase)))
            {
                return $"{typeInfo.Name}?";
            }

            return typeInfo.Name;
        }

        private bool IsInterface(IResolvedTypeInfo typeInfo)
        {
            return (!typeInfo.IsPrimitive && typeInfo.Name.StartsWith("I") && typeInfo.Name.Length >= 2 && char.IsUpper(typeInfo.Name[1]));
        }
    }
}