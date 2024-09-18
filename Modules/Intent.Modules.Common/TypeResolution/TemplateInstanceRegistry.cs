using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Registrations;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// This is a mechanism for registering up generated types so they can be discovered at code gen time
    /// </summary>
    public class TemplateInstanceRegistry
    {
        private static Dictionary<(string templateIdOrRole, string ModelId), ISet<ITypeInfoResolver>> _registrations = new Dictionary<(string templateIdOrRole, string ModelId), ISet<ITypeInfoResolver>>();

        /// <summary>
        /// Register types which can be resolved
        /// </summary>
        public static void Register(ITypeInfoResolver resolver, string roleName = null)
        {
            var key = (roleName ?? resolver.Template.Id, resolver.Model.Id);
            if (!_registrations.TryGetValue(key, out var templateInstances))
            {
                _registrations.Add(key, templateInstances = new HashSet<ITypeInfoResolver>());
            }
            templateInstances.Add(resolver);
        }

        internal static IResolvedTypeInfo GetTypeInfo(string templateIdOrRole, string modelId, IntentTemplateBase dependentTemplate, bool trackDependency = true)
        {
            var key = (templateIdOrRole, modelId);
            if (!_registrations.TryGetValue(key, out var resolvers))
            {
                return null;
            }

            if (resolvers.Count > 1)
            {
                throw new Exception($"More than one instance of template {templateIdOrRole} was found with modeId {modelId}");
            }

            ITypeInfoResolver resolver = resolvers.First();
            if (trackDependency)
            {
                AddTemplateDependency(dependentTemplate, resolver.Template, resolver.Model);
            }

            return resolver.Resolve();
        }

        public static IEnumerable< IResolvedTypeInfo> GetRegisteredTypes()
        {
            return _registrations.Values.SelectMany(x => x.Select(y => y.Resolve()));
        }

        private static void AddTemplateDependency(IntentTemplateBase hasDependency, ITemplate dependency, IMetadataModel model)
        {
            if (model == null)
            {
                hasDependency.AddTemplateDependency(TemplateDependency.OnTemplate(dependency.Id));
            }
            else
            {
                hasDependency.AddTemplateDependency(dependency.Id, model);
            }
        }
    }

    public interface ITypeInfoResolver
    {
        IResolvedTypeInfo Resolve();
        ITemplate Template { get; }
        IMetadataModel Model { get; }
    }

}
