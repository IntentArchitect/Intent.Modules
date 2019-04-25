using System;
using System.Linq;
using Intent.MetaModel;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.Modules.Application.Contracts;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public class DeleteImplementationStrategy : IImplementationStrategy
    {
        public bool Match(IMetadataManager metaDataManager, Engine.IApplication application, IClass domainModel, IOperationModel operationModel)
        {
            if (operationModel.Parameters.Count() != 1)
            {
                return false;
            }

            if (!operationModel.Parameters.Any(p => string.Equals(p.Name, "id", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            if (operationModel.ReturnType != null)
            {
                return false;
            }

            var lowerDomainName = domainModel.Name.ToLower();
            var lowerOperationName = operationModel.Name.ToLower();
            return new[]
            {
                "delete",
                $"delete{lowerDomainName}"
            }
            .Contains(lowerOperationName);
        }

        public string GetImplementation(IMetadataManager metaDataManager, Engine.IApplication application, IClass domainModel, IOperationModel operationModel)
        {
            return $@"var existing{domainModel.Name} ={ (operationModel.IsAsync() ? " await" : "") } {domainModel.Name.ToPrivateMember()}Repository.FindById{ (operationModel.IsAsync() ? "Async" : "") }(id);
                {domainModel.Name.ToPrivateMember()}Repository.Remove(existing{domainModel.Name});";
        }
    }
}
