using System;
using Intent.Engine;
using Intent.Metadata.Models;
using IClass = Intent.Modelers.Domain.Api.IClass;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public interface IImplementationStrategy
    {
        bool Match(IMetadataManager metadataManager, Engine.IApplication application, IClass domainModel, IOperation operationModel);
        string GetImplementation(IMetadataManager metadataManager, Engine.IApplication application, IClass domainModel, IOperation operationModel);
    }
}
