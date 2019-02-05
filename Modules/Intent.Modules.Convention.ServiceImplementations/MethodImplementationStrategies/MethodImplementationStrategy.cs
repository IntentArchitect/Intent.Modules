using System;
using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.SoftwareFactory.Engine;

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

        public static string ImplementOnMatch(IMetaDataManager metaDataManager, SoftwareFactory.Engine.IApplication application, IClass domainModel, IOperationModel operationModel)
        {
            foreach (var strategy in _strategies)
            {
                if (strategy.Match(metaDataManager, application, domainModel, operationModel))
                {
                    return strategy.GetImplementation(metaDataManager, application, domainModel, operationModel);
                }
            }

            return string.Empty;
        }
    }
}
