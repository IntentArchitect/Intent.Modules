using System;
using System.Linq;
using System.Text;
using Intent.MetaModel;
using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.Modules.Common;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies
{
    public class CreateImplementationStrategy : IImplementationStrategy
    {
        public bool Match(IMetaDataManager metaDataManager, SoftwareFactory.Engine.IApplication application, IClass domainModel, IOperationModel operationModel)
        {
            if (operationModel.Parameters.Count() != 1)
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
                "post",
                $"post{lowerDomainName}",
                "create",
                $"create{lowerDomainName}",
            }
            .Contains(lowerOperationName);
        }

        public string GetImplementation(IMetaDataManager metaDataManager, SoftwareFactory.Engine.IApplication application, IClass domainModel, IOperationModel operationModel)
        {
            return $@"var new{domainModel.Name} = new {domainModel.Name}
                {{
{EmitPropertyAssignments(metaDataManager, application, domainModel, operationModel.Parameters.First())}
                }};
                
                {domainModel.Name.ToPrivateMember()}Repository.Add(new{domainModel.Name});";
        }

        private string EmitPropertyAssignments(IMetaDataManager metaDataManager, SoftwareFactory.Engine.IApplication application, IClass domainModel, IOperationParameterModel operationParameterModel)
        {
            var sb = new StringBuilder();
            var dto = metaDataManager.GetDTOModels(application).First(p => p.Id == operationParameterModel.TypeReference.Id);
            foreach (var domainAttribute in domainModel.Attributes)
            {
                var dtoField = dto.Fields.FirstOrDefault(p => p.Name.Equals(domainAttribute.Name, StringComparison.OrdinalIgnoreCase));
                if (dtoField == null)
                {
                    sb.AppendLine($"                    #warning No matching field found for {domainAttribute.Name}");
                    continue;
                }
                if (domainAttribute.Type.Id != dtoField.TypeReference.Id)
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
