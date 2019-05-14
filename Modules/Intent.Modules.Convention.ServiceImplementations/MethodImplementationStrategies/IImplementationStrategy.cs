using System;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public interface IImplementationStrategy
    {
        bool Match(IMetaDataManager metaDataManager, SoftwareFactory.Engine.IApplication application, IClass domainModel, IOperationModel operationModel);
        string GetImplementation(IMetaDataManager metaDataManager, SoftwareFactory.Engine.IApplication application, IClass domainModel, IOperationModel operationModel);
    }
}
