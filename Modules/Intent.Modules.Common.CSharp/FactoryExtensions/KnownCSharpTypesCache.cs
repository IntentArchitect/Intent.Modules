using System;
using System.Collections.Generic;
using System.Linq;
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
            _knownTypesByNamespace = application
                .FindTemplateInstances<IClassProvider>(TemplateDependency.OfType<IClassProvider>())
                .GroupBy(x => string.IsNullOrWhiteSpace(x.Namespace) ? string.Empty : x.Namespace)
                .ToDictionary(x => x.Key, x => (ISet<string>)new HashSet<string>(x.Select(y => y.ClassName)));
        }

        /// <summary>
        /// Returns known types by namespace.
        /// </summary>
        public static IReadOnlyDictionary<string, ISet<string>> GetKnownTypesByNamespace()
        {
            if (_knownTypesByNamespace == null)
            {
                Logging.Log.Warning($"{nameof(GetKnownTypesByNamespace)} is being called before " +
                                    "Template Registration has been completed. Ensure that methods " +
                                    "like GetTypeName and UseType are not being used in template " +
                                    $"constructors.{Environment.NewLine}{Environment.StackTrace}");

                return Empty;
            }

            return _knownTypesByNamespace;
        }
    }
}
