using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
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
    public class CSharpTypesCache : FactoryExtensionBase
    {
        /// <inheritdoc />
        public override string Id => "Intent.Modules.Common.CSharp.FactoryExtensions.KnownCSharpTypesCache";

        private static TypeRegistry _knownTypes;
        private static TypeRegistry _outputTargetNames;
        private static readonly TypeRegistry Empty = new(Enumerable.Empty<string>());

        /// <inheritdoc />
        public override int Order { get; set; } = int.MinValue;

        /// <inheritdoc />
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var knownTypesByNamespace = application
                .FindTemplateInstances<IClassProvider>(TemplateDependency.OfType<IClassProvider>())
                .Select(x => string.IsNullOrWhiteSpace(x.Namespace) ? x.ClassName : $"{x.Namespace}.{x.ClassName}");
            var commonKnownTypes = GetCommonKnownTypes();

            var knownTypes = knownTypesByNamespace.Union(commonKnownTypes).ToArray();
            var outputTargetNames = application.OutputTargets.Select(x => x.Name).ToArray();

            _knownTypes = new TypeRegistry(knownTypes);
            _outputTargetNames = new TypeRegistry(outputTargetNames);
        }

        private static IEnumerable<string> GetCommonKnownTypes()
        {
            // This forces these assemblies to be loaded prior to the call to
            // AppDomain.CurrentDomain.GetAssemblies()
            var additionalTypesToLoad = new[]
                {
                    typeof(Transaction)
                }
                .Select(x => x.Assembly)
                .Distinct();
            foreach (var type in additionalTypesToLoad)
            {
                Logging.Log.Debug($"Ensured types {type.GetName().Name} will be scanned for known types.");
            }

            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.GetName().Name?.Split('.')[0] == "System")
                .SelectMany(c => c.GetExportedTypes())
                .Where(x => !x.ContainsGenericParameters)
                .Select(x => x.FullName)
                .OrderBy(x => x)
                .ToArray();
        }

        internal static TypeRegistry GetKnownTypes()
        {
            if (_knownTypes == null)
            {
                // TODO: Re-add this warning once we've resolved the issue of some decorators calling this during their construction: https://dev.azure.com/intentarchitect/Intent%20Architect/_workitems/edit/1282
                //Logging.Log.Warning($"{nameof(GetKnownTypesByNamespace)} is being called before " +
                //                    "Template Registration has been completed. Ensure that methods " +
                //                    "like GetTypeName and UseType are not being used in template " +
                //                    $"constructors.{Environment.NewLine}{Environment.StackTrace}");

                return Empty;
            }

            return _knownTypes;
        }

        internal static TypeRegistry GetOutputTargetNames()
        {
            if (_knownTypes == null)
            {
                // TODO: Re-add this warning once we've resolved the issue of some decorators calling this during their construction: https://dev.azure.com/intentarchitect/Intent%20Architect/_workitems/edit/1282
                //Logging.Log.Warning($"{nameof(GetKnownTypesByNamespace)} is being called before " +
                //                    "Template Registration has been completed. Ensure that methods " +
                //                    "like GetTypeName and UseType are not being used in template " +
                //                    $"constructors.{Environment.NewLine}{Environment.StackTrace}");

                return Empty;
            }

            return _outputTargetNames;
        }
    }
}
