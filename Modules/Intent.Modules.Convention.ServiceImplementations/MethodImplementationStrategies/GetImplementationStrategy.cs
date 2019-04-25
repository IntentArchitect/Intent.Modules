using System;
using System.Linq;
using Humanizer.Inflections;
using Intent.MetaModel;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.Modules.Application.Contracts;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public class GetImplementationStrategy : IImplementationStrategy
    {
        public bool Match(IMetadataManager metaDataManager, Engine.IApplication application, IClass domainModel, IOperationModel operationModel)
        {
            if (operationModel.Parameters.Any())
            {
                return false;
            }

            if (!(operationModel?.ReturnType?.TypeReference?.IsCollection ?? false))
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

        public string GetImplementation(IMetadataManager metaDataManager, Engine.IApplication application, IClass domainModel, IOperationModel operationModel)
        {
            return $@"var elements ={ (operationModel.IsAsync() ? "await" : "") } {domainModel.Name.ToPrivateMember()}Repository.FindAll{ (operationModel.IsAsync() ? "Async" : "") }();
            return elements.MapTo{domainModel.Name.ToPascalCase()}DTOs();";
        }
    }
}
