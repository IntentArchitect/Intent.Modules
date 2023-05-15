using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.CSharp.FactoryExtensions
{
    /// <summary>
    /// Cache of known C# types as determined by all templates which are
    /// <see cref="IClassProvider"/>.
    /// </summary>
    public class KnownCSharpTypesCache : FactoryExtensionBase
    {
        /// <inheritdoc />
        public override string Id => "Intent.Modules.Common.CSharp.FactoryExtensions.KnownCSharpTypesCache";

        private static IReadOnlyDictionary<string, ISet<string>> _knownTypesByNamespace;
        private static readonly IReadOnlyDictionary<string, ISet<string>> Empty = new Dictionary<string, ISet<string>>();

        /// <inheritdoc />
        public override int Order { get; set; } = int.MinValue;

        /// <inheritdoc />
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var knownTypesByNamespace = application
                .FindTemplateInstances<IClassProvider>(TemplateDependency.OfType<IClassProvider>())
                .GroupBy(x => string.IsNullOrWhiteSpace(x.Namespace) ? string.Empty : x.Namespace)
                .ToDictionary(x => x.Key, x => (ISet<string>)new HashSet<string>(x.Select(y => y.ClassName)));
            AddCommonKnownTypes(knownTypesByNamespace);
            _knownTypesByNamespace = knownTypesByNamespace;
        }

        private void AddCommonKnownTypes(Dictionary<string, ISet<string>> knownTypesByNamespace)
        {
            AddCommonKnownType(knownTypesByNamespace, "System.Attribute");
            AddCommonKnownType(knownTypesByNamespace, "System.Action");
        }

        private void AddCommonKnownType(Dictionary<string, ISet<string>> knownTypesByNamespace, string fullTypeName)
        {
            int classStartIndex = fullTypeName.LastIndexOf(".");
            var namespaceName = fullTypeName.Substring(0, classStartIndex);
            var className = fullTypeName.Substring(classStartIndex + 1); 

            if (!knownTypesByNamespace.TryGetValue(namespaceName, out var systemKnownTypes))
            {
                systemKnownTypes = new HashSet<string>();
                knownTypesByNamespace.Add(namespaceName, systemKnownTypes);
            }

            systemKnownTypes.Add(className);
        }

        /// <summary>
        /// Returns known types by namespace.
        /// </summary>
        public static IReadOnlyDictionary<string, ISet<string>> GetKnownTypesByNamespace()
        {
            if (_knownTypesByNamespace == null)
            {
                // TODO: Re-add this warning once we've resolved the issue of some decorators calling this during their construction: https://dev.azure.com/intentarchitect/Intent%20Architect/_workitems/edit/1282
                //Logging.Log.Warning($"{nameof(GetKnownTypesByNamespace)} is being called before " +
                //                    "Template Registration has been completed. Ensure that methods " +
                //                    "like GetTypeName and UseType are not being used in template " +
                //                    $"constructors.{Environment.NewLine}{Environment.StackTrace}");

                return Empty;
            }

            return _knownTypesByNamespace;
        }
    }
}
