using System.Linq;
using Intent.MetaModel.Dto.Old;
using Intent.MetaModel.UMLModel;
using Intent.Packages.Mapping.EntityToDto.Templates.DTOMappingProfile;
using Intent.Packages.Mapping.EntityToDto.Templates.EntityMappingExtensions;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Mapping.EntityToDto
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var dtoModels = metaDataManager.GetMetaData<DtoModel>(new MetaDataType("DtoProjection")).Where(x => x.BoundedContextName == application.ApplicationName).ToList();
            var enumModels = metaDataManager.GetMetaData<EnumDefinition>(new MetaDataType("Enums")).Where(x => x.BoundedContext() == application.ApplicationName).ToList();

            var mappingModels = dtoModels.SelectMany((o) => o.Mappings).ToList();
            if (mappingModels.Count > 0)
            {
                RegisterTemplate(DTOMappingTemplate.Identifier, project => new DTOMappingTemplate(project, mappingModels, enumModels));
                RegisterTemplate(EntityMappingExtensionsTemplate.Identifier, project => new EntityMappingExtensionsTemplate(project));
            }
        }
    }
}
