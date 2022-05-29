using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<ICSharpProject, CSharpNullableFormatter> Cache = new();

        private CSharpNullableFormatter(ICSharpProject project)
        {
            _project = project;
        }

        /// <summary>
        /// Returns an instance of <see cref="CSharpNullableFormatter"/> constructed with the
        /// specified <paramref name="project"/>.
        /// </summary>
        /// <remarks>
        /// A cache of <see cref="CSharpNullableFormatter"/> instances is first checked for an
        /// already existing instance, if an instance is found then that is returned, otherwise a new
        /// instance is created, placed in the cache and returned.
        /// </remarks>
        public static INullableFormatter GetOrCreate(ICSharpProject project)
        {
            return Cache.GetOrAdd(
                project,
                _ => new CSharpNullableFormatter(project));
        }

        /// <inheritdoc />
        public string AsNullable(IResolvedTypeInfo typeInfo, string type)
        {
            if (typeInfo.IsNullable &&
                (typeInfo.IsPrimitive || _project.IsNullableAwareContext() || 
                 typeInfo.TypeReference.Element.SpecializationType.EndsWith("Enum", StringComparison.OrdinalIgnoreCase)))
            {
                return $"{type}?";
            }

            return type;
        }
    }
}