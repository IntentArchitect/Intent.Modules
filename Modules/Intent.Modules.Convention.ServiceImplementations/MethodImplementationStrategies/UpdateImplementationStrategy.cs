using System;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Common;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public class UpdateImplementationStrategy : IImplementationStrategy
    {
        public bool Match(IMetadataManager metadataManager, Engine.IApplication application, ClassModel domainModel, OperationModel operationModel)
        {
            if (operationModel.Parameters.Count() != 2)
            {
                return false;
            }

            if (!operationModel.Parameters.Any(p => string.Equals(p.Name, "id", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            if (operationModel.TypeReference.Element != null)
            {
                return false;
            }

            var lowerDomainName = domainModel.Name.ToLower();
            var lowerOperationName = operationModel.Name.ToLower();
            return new[]
            {
                "put",
                $"put{lowerDomainName}",
                "update",
                $"update{lowerDomainName}",
            }
            .Contains(lowerOperationName);
        }

        public string GetImplementation(IMetadataManager metadataManager, Engine.IApplication application, ClassModel domainModel, OperationModel operationModel)
        {
            var idParam = operationModel.Parameters.First(p => string.Equals(p.Name, "id", StringComparison.OrdinalIgnoreCase));
            var dtoParam = operationModel.Parameters.First(p => !string.Equals(p.Name, "id", StringComparison.OrdinalIgnoreCase));

            return $@"var existing{domainModel.Name} ={ (operationModel.IsAsync() ? " await" : "") } {domainModel.Name.ToPrivateMember()}Repository.FindById{ (operationModel.IsAsync() ? "Async" : "") }({idParam.Name});
                {EmitPropertyAssignments(metadataManager, application, domainModel, "existing"+ domainModel.Name, dtoParam)}";
        }

        private string EmitPropertyAssignments(IMetadataManager metadataManager, Engine.IApplication application, ClassModel domainModel, string domainVarName, ParameterModel operationParameterModel)
        {
            var sb = new StringBuilder();
            var dto = metadataManager.Services(application).GetDTOModels().First(p => p.Id == operationParameterModel.TypeReference.Element.Id);
            foreach (var domainAttribute in domainModel.Attributes)
            {
                var dtoField = dto.Fields.FirstOrDefault(p => p.Name.Equals(domainAttribute.Name, StringComparison.OrdinalIgnoreCase));
                if (dtoField == null)
                {
                    sb.AppendLine($"                    #warning No matching field found for {domainAttribute.Name}");
                    continue;
                }
                if (domainAttribute.Type.Element.Id != dtoField.TypeReference.Element.Id)
                {
                    sb.AppendLine($"                    #warning No matching type for Domain: {domainAttribute.Name} and DTO: {dtoField.Name}");
                    continue;
                }
                sb.AppendLine($"                    {domainVarName}.{domainAttribute.Name.ToPascalCase()} = {operationParameterModel.Name}.{dtoField.Name.ToPascalCase()};");
            }

            return sb.ToString().Trim();
        }
    }
}
