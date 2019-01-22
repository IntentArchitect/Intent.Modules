using System;
using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public static class MethodImplementationStrategy
    {
        private static readonly List<IImplementationStrategy> _strategies;

        static MethodImplementationStrategy()
        {
            _strategies = new List<IImplementationStrategy>
            {
                new GetImplementationStrategy()
            };
        }

        public static string ImplementOnMatch(IClass domainModel, IOperationModel operationModel)
        {
            foreach (var strategy in _strategies)
            {
                if (strategy.Match(domainModel, operationModel))
                {
                    return strategy.GetImplementation(domainModel, operationModel);
                }
            }

            return string.Empty;
        }
    }
}
