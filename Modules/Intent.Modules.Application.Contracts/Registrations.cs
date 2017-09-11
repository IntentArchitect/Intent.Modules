using System.Linq;
using Intent.MetaModel.Dto.Old;
using Intent.Packages.Application.Contracts.Legacy.DTO;
using Intent.Packages.Application.Contracts.Legacy.ServiceContract;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Application.Contracts
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var dtoModels = metaDataManager.GetMetaData<DtoModel>(new MetaDataType("DtoProjection")).Where(x => x.BoundedContextName == application.ApplicationName).ToList();
            var serviceModels = metaDataManager.GetMetaData<ServiceModel>(new MetaDataType("Service-Legacy")).Where(x => x.ApplicationName == application.ApplicationName).ToList();

            foreach (var model in dtoModels)
            {
                RegisterTemplate(DTOTemplate.Identifier, project => new DTOTemplate(project, model));
            }

            foreach (var serviceModel in serviceModels)
            {
                RegisterTemplate(ServiceContractTemplate.Identifier, project => new ServiceContractTemplate(serviceModel, project));
            }
        }
    }
}
