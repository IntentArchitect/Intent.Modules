using System;
using System.Linq;
using Humanizer.Inflections;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.Contracts;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public class GetImplementationStrategy : IImplementationStrategy
    {
        public bool Match(IMetadataManager metadataManager, Engine.IApplication application, ClassModel domainModel, OperationModel operationModel)
        {
            if (operationModel.Parameters.Any())
            {
                return false;
            }

            if (!(operationModel?.TypeReference?.IsCollection ?? false))
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

        public string GetImplementation(IMetadataManager metadataManager, Engine.IApplication application, ClassModel domainModel, OperationModel operationModel)
        {
            return $@"var elements ={ (operationModel.IsAsync() ? "await" : "") } {domainModel.Name.ToPrivateMember()}Repository.FindAll{ (operationModel.IsAsync() ? "Async" : "") }();
            return elements.MapTo{domainModel.Name.ToPascalCase()}DTOs();";
        }
    }
}
