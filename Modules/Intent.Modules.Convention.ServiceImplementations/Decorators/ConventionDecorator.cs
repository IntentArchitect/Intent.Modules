using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.SoftwareFactory.Engine;
using System;

namespace Intent.Modules.Convention.ServiceImplementations.Decorators
{
    public class ConventionDecorator : ServiceImplementationDecoratorBase
    {
        public const string Identifier = "Intent.Conventions.ServiceImplementations";

        private readonly IMetaDataManager _metaDataManager;

        public ConventionDecorator(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }
    }
}
