
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.CommonTypes.Contracts;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Plugins;
using Intent.SoftwareFactory.Plugins.FactoryExtensions;
using Intent.SoftwareFactory.Templates;
using System.ComponentModel;
using Intent.Modules.Common;
using Intent.Modules.Common.TypeResolution;
using Intent.SoftwareFactory;

namespace Intent.Modules.CommonTypes.FactoryExtensions
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
            if (templateInstance is IRequireTypeResolver)
            {
                var requireTypeResolver = templateInstance as IRequireTypeResolver;
                if (templateInstance is ITypeResolverFactoryResolution)
                {
                    var resolverFactory = (templateInstance as ITypeResolverFactoryResolution).DetermineTypeResolver(_typeResolverFactoryRepository);
                    requireTypeResolver.Types = resolverFactory.Create();
                }
                else
                {
                    requireTypeResolver.Types =
                        _typeResolverFactoryRepository.GetTypeResolver(templateInstance.GetMetaData())
                        .Create();
                }
            }
        }

        public void PostCreation(ITemplate templateInstance)
        {

        }


    }
}
