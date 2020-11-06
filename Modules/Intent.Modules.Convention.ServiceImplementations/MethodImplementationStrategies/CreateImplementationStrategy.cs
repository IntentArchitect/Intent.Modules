using System;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public class CreateImplementationStrategy : IImplementationStrategy
    {
        public bool Match(IMetadataManager metadataManager, Engine.IApplication application, ClassModel domainModel, OperationModel operationModel)
        {
            if (operationModel.Parameters.Count() != 1)
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
                "post",
                $"post{lowerDomainName}",
                "create",
                $"create{lowerDomainName}",
            }
            .Contains(lowerOperationName);
        }

        public string GetImplementation(IMetadataManager metadataManager, Engine.IApplication application, ClassModel domainModel, OperationModel operationModel)
        {
            return $@"var new{domainModel.Name} = new {domainModel.Name}
                {{
{EmitPropertyAssignments(metadataManager, application, domainModel, operationModel.Parameters.First())}
                }};
                
                {domainModel.Name.ToPrivateMember()}Repository.Add(new{domainModel.Name});";
        }

        private string EmitPropertyAssignments(IMetadataManager metadataManager, Engine.IApplication application, ClassModel domainModel, ParameterModel operationParameterModel)
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
                sb.AppendLine($"                    {domainAttribute.Name.ToPascalCase()} = {operationParameterModel.Name}.{dtoField.Name.ToPascalCase()},");
            }

            return sb.ToString().Trim();
        }
    }
}
