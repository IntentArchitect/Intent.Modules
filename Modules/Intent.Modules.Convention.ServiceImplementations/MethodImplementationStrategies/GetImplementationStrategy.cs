using System;
using System.Linq;
using Humanizer.Inflections;
using Intent.MetaModel;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.Modules.Application.Contracts;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public class GetImplementationStrategy : IImplementationStrategy
    {
        public string GetImplementation(IClass domainModel, IOperationModel operationModel)
        {
            return $@"var elements ={ (operationModel.IsAsync() ? "await" : "") } _{domainModel.Name.ToCamelCase()}Repository.FindAll{ (operationModel.IsAsync() ? "Async" : "") }();
            return elements.MapTo{domainModel.Name.ToPascalCase()}DTOs();";
        }

        public bool Match(IClass domainModel, IOperationModel operationModel)
        {
            if (operationModel.Parameters.Any())
            {
                return false;
            }

            if (!operationModel.ReturnType.TypeReference.IsCollection)
            {
                return false;
            }

            var lowerDomainName = domainModel.Name.ToLower();
            var pluralLowerDomainName = Vocabularies.Default.Pluralize(lowerDomainName);
            var lowerOperationName = operationModel.Name.ToLower();
            return new[]
            {
                "get",
                $"get{lowerDomainName}",
                $"get{pluralLowerDomainName}",
                "getall",
                "find",
                $"find{lowerDomainName}",
                $"find{pluralLowerDomainName}",
                "findall"
            }
            .Contains(lowerOperationName);
        }
    }
}
