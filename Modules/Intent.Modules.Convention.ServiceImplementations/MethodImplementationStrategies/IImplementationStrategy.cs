using System;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public interface IImplementationStrategy
    {
        bool Match(IClass domainModel, IOperationModel operationModel);
        string GetImplementation(IClass domainModel, IOperationModel operationModel);
    }
}
