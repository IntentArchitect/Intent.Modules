using System;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public interface IImplementationStrategy
    {
        bool Match(IMetadataManager metaDataManager, Engine.IApplication application, IClass domainModel, IOperationModel operationModel);
        string GetImplementation(IMetadataManager metaDataManager, Engine.IApplication application, IClass domainModel, IOperationModel operationModel);
    }
}
