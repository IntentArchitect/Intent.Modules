using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

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

        public static string ImplementOnMatch(IMetadataManager metadataManager, Engine.IApplication application, ClassModel domainModel, OperationModel operationModel)
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
