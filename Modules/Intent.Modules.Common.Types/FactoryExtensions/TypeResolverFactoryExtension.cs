
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Types.Contracts;
using Intent.Templates;
using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Contracts;
using Intent.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.SoftwareFactory;
using Intent.Utils;

namespace Intent.Modules.Common.Types.FactoryExtensions
{
    [Description("Type Resolver (Factory Extension)")]
    public class TypeResolverFactoryExtension : IFactoryExtension, IDiscoverTypes, ITemplateLifeCycle
    {
        private ITypeResolverFactoryRepository _typeResolverFactoryRepository;

        public TypeResolverFactoryExtension()
        {
            Id = "Intent.CommonTypes.TypeResolvers";
            Order = 0;
        }

        public string Id { get; set; }
        public int Order { get; set; }

        public void OnTypesLoaded(ITypeActivator typeActivator, IEnumerable<Type> discoveredTypes)
        {
            var typeResolverFactories = new List<ITypeResolverFactory>();
            foreach (var type in discoveredTypes)
            {
                if (typeof(ITypeResolverFactory).IsAssignableFrom(type))
                {
                    ITypeResolverFactory factory = typeActivator.Activate<ITypeResolverFactory>(type);
                    Logging.Log.Info("Discovered Type Resolver : " + type.GetDescription());
                    typeResolverFactories.Add(factory);
                }
            }
            _typeResolverFactoryRepository = new TypeResolverFactoryRepository(typeResolverFactories.ToArray());
        }

        public void PostConfiguration(ITemplate templateInstance)
        {
            if (templateInstance is IRequireTypeResolver requireTypeResolver && _typeResolverFactoryRepository.GetTypeResolver(templateInstance.GetMetadata()) != null)
            {
                requireTypeResolver.Types =
                    _typeResolverFactoryRepository.GetTypeResolver(templateInstance.GetMetadata())
                    .Create();
            }
        }

        public void PostCreation(ITemplate templateInstance)
        {

        }


    }
}
