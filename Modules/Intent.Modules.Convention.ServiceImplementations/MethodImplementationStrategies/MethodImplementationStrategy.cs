using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using IClass = Intent.Modelers.Domain.Api.IClass;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public static class MethodImplementationStrategy
    {
        private static readonly List<IImplementationStrategy> _strategies;

        static MethodImplementationStrategy()
        {
            _strategies = new List<IImplementationStrategy>
            {
                new GetImplementationStrategy(),
                new GetByIdImplementationStrategy(),
                new CreateImplementationStrategy(),
                new UpdateImplementationStrategy(),
                new DeleteImplementationStrategy()
            };
        }

        public static string ImplementOnMatch(IMetadataManager metadataManager, Engine.IApplication application, IClass domainModel, IOperation operationModel)
        {
            foreach (var strategy in _strategies)
            {
                if (strategy.Match(metadataManager, application, domainModel, operationModel))
                {
                    return strategy.GetImplementation(metadataManager, application, domainModel, operationModel);
                }
            }

            return string.Empty;
        }
    }
}
