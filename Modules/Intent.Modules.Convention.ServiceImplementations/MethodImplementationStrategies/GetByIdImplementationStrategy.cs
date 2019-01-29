using System;
using System.Linq;
using Humanizer.Inflections;
using Intent.MetaModel;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.Modules.Application.Contracts;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public class GetByIdImplementationStrategy : IImplementationStrategy
    {
        public string GetImplementation(IClass domainModel, IOperationModel operationModel)
        {
            return $@"var element ={ (operationModel.IsAsync() ? "await" : "") } _{domainModel.Name.ToCamelCase()}Repository.FindById{ (operationModel.IsAsync() ? "Async" : "") }({operationModel.Parameters.First().Name.ToCamelCase()});
            return element.MapTo{domainModel.Name.ToPascalCase()}DTO();";
        }

        public bool Match(IClass domainModel, IOperationModel operationModel)
        {
            if (operationModel.Parameters.Count() != 1)
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
    }
}
