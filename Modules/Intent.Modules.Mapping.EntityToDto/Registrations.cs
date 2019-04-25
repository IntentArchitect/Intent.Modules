using Intent.MetaModel.Dto.Old;
using Intent.Modules.Mapping.EntityToDto.Templates.DTOMappingProfile;
using Intent.Modules.Mapping.EntityToDto.Templates.EntityMappingExtensions;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.SoftwareFactory.Registrations;
using System.Linq;
using Intent.Modules.Common.Registrations;
using Intent.Modules.RichDomain;

namespace Intent.Modules.Mapping.EntityToDto
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            var dtoModels = metaDataManager.GetMetaData<DtoModel>(new MetaDataIdentifier("DtoProjection")).Where(x => x.BoundedContextName == application.ApplicationName).ToList();
            var enumModels = metaDataManager.GetMetaData<EnumDefinition>(new MetaDataIdentifier("Enums")).Where(x => x.BoundedContext() == application.ApplicationName).ToList();

            var mappingModels = dtoModels.SelectMany((o) => o.Mappings).ToList();
            if (mappingModels.Count > 0)
            {
                RegisterTemplate(DTOMappingTemplate.Identifier, project => new DTOMappingTemplate(project, mappingModels, enumModels));
                RegisterTemplate(EntityMappingExtensionsTemplate.Identifier, project => new EntityMappingExtensionsTemplate(project));
            }
        }
    }
}
