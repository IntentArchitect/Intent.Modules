using System;
using System.Linq;
using Intent.MetaModel;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.Modules.Application.Contracts;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public class GetByIdImplementationStrategy : IImplementationStrategy
    {
        public bool Match(IMetaDataManager metaDataManager, SoftwareFactory.Engine.IApplication application, IClass domainModel, IOperationModel operationModel)
        {
            if (operationModel.Parameters.Count() != 1)
            {
                return false;
            }

            if (!operationModel.Parameters.Any(p => string.Equals(p.Name, "id", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            if (operationModel?.ReturnType?.TypeReference?.IsCollection ?? false)
            {
                return false;
            }

            var lowerDomainName = domainModel.Name.ToLower();
            var lowerOperationName = operationModel.Name.ToLower();
            return new[]
            {
                "get",
                $"get{lowerDomainName}",
                "find",
                $"find{lowerDomainName}",
                lowerDomainName
            }
            .Contains(lowerOperationName);
        }

        public string GetImplementation(IMetaDataManager metaDataManager, SoftwareFactory.Engine.IApplication application, IClass domainModel, IOperationModel operationModel)
        {
            return $@"var element ={ (operationModel.IsAsync() ? " await" : "") } {domainModel.Name.ToPrivateMember()}Repository.FindById{ (operationModel.IsAsync() ? "Async" : "") }({operationModel.Parameters.First().Name.ToCamelCase()});
            return element.MapTo{domainModel.Name.ToPascalCase()}DTO();";
        }
    }
}
