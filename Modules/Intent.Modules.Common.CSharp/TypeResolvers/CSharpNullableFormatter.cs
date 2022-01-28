using System;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.TypeResolvers
{
    /// <summary>
    /// C# implementation of <see cref="INullableFormatter"/>.
    /// </summary>
    public class CSharpNullableFormatter : INullableFormatter
    {
        private readonly ICSharpProject _project;

        /// <summary>
        /// Creates a new instance of <see cref="CSharpNullableFormatter"/>.
        /// </summary>
        public CSharpNullableFormatter(ICSharpProject project)
        {
            _project = project;
        }

        /// <inheritdoc />
        public string AsNullable(IResolvedTypeInfo typeInfo)
        {
            if (typeInfo.IsNullable &&
                (typeInfo.IsPrimitive || _project.IsNullableAwareContext() || 
                 typeInfo.TypeReference.Element.SpecializationType.Equals("Enum", StringComparison.InvariantCultureIgnoreCase)))
            {
                return $"{typeInfo.Name}?";
            }

            return typeInfo.Name;
        }
    }
}