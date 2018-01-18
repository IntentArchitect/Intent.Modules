using Intent.MetaModel.Dto.Old;
using Intent.Modules.Application.Contracts.Legacy.DTO;
using Intent.Modules.Application.Contracts.Legacy.ServiceContract;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Registrations;
using System.Linq;

namespace Intent.Modules.Application.Contracts
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var dtoModels = metaDataManager.GetMetaData<DtoModel>(new MetaDataIdentifier("DtoProjection")).Where(x => x.BoundedContextName == application.ApplicationName).ToList();
            var serviceModels = metaDataManager.GetMetaData<ServiceModel>(new MetaDataIdentifier("Service-Legacy")).Where(x => x.ApplicationName == application.ApplicationName).ToList();

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
