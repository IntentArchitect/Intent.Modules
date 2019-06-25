using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.Contracts;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using IClass = Intent.Modelers.Domain.Api.IClass;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public class DeleteImplementationStrategy : IImplementationStrategy
    {
        public bool Match(IMetadataManager metadataManager, Engine.IApplication application, IClass domainModel, IOperation operationModel)
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

        public string GetImplementation(IMetadataManager metadataManager, Engine.IApplication application, IClass domainModel, IOperation operationModel)
        {
            return $@"var existing{domainModel.Name} ={ (operationModel.IsAsync() ? " await" : "") } {domainModel.Name.ToPrivateMember()}Repository.FindById{ (operationModel.IsAsync() ? "Async" : "") }(id);
                {domainModel.Name.ToPrivateMember()}Repository.Remove(existing{domainModel.Name});";
        }
    }
}
