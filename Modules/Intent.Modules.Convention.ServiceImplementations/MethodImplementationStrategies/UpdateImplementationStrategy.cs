using System;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Common;
using Intent.Engine;
using Intent.Modelers.Services;
using Intent.Modules.Common.Templates;
using IClass = Intent.Modelers.Domain.Api.IClass;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public class UpdateImplementationStrategy : IImplementationStrategy
    {
        public bool Match(IMetadataManager metadataManager, Engine.IApplication application, IClass domainModel, IOperation operationModel)
        {
            if (operationModel.Parameters.Count() != 2)
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
                "put",
                $"put{lowerDomainName}",
                "update",
                $"update{lowerDomainName}",
            }
            .Contains(lowerOperationName);
        }

        public string GetImplementation(IMetadataManager metadataManager, Engine.IApplication application, IClass domainModel, IOperation operationModel)
        {
            var idParam = operationModel.Parameters.First(p => string.Equals(p.Name, "id", StringComparison.OrdinalIgnoreCase));
            var dtoParam = operationModel.Parameters.First(p => !string.Equals(p.Name, "id", StringComparison.OrdinalIgnoreCase));

            return $@"var existing{domainModel.Name} ={ (operationModel.IsAsync() ? " await" : "") } {domainModel.Name.ToPrivateMember()}Repository.FindById{ (operationModel.IsAsync() ? "Async" : "") }({idParam.Name});
                {EmitPropertyAssignments(metadataManager, application, domainModel, "existing"+ domainModel.Name, dtoParam)}";
        }

        private string EmitPropertyAssignments(IMetadataManager metadataManager, Engine.IApplication application, IClass domainModel, string domainVarName, IOperationParameter operationParameterModel)
        {
            var sb = new StringBuilder();
            var dto = metadataManager.GetDTOs(application).First(p => p.Id == operationParameterModel.Type.Id);
            foreach (var domainAttribute in domainModel.Attributes)
            {
                var dtoField = dto.Fields.FirstOrDefault(p => p.Name.Equals(domainAttribute.Name, StringComparison.OrdinalIgnoreCase));
                if (dtoField == null)
                {
                    sb.AppendLine($"                    #warning No matching field found for {domainAttribute.Name}");
                    continue;
                }
                if (domainAttribute.Type.Id != dtoField.Type.Id)
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
