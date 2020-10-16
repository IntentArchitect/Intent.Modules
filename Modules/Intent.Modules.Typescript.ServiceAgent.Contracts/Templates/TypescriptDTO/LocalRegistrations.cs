using Intent.Modelers.Services.Api;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO
{
    [Description("Intent Typescript ServiceAgent DTO - Local")]
    public class LocalRegistrations : ModelTemplateRegistrationBase<DTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        public LocalRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;

        }

        public override string TemplateId => TypescriptDtoTemplate.LocalIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, DTOModel model)
        {
            return new TypescriptDtoTemplate(TemplateId, project, model);
        }

        public override IEnumerable<DTOModel> GetModels(Engine.IApplication application)
        {
            var dtoModels = _metadataManager.Services(application).GetDTOModels();

            // TODO JL: Temp, filter out ones for server only, will ultimately get replaced with concept of client applications in the future
            dtoModels = dtoModels.Where(x => x.Stereotypes.All(s => s.Name != "ServerOnly") && !FolderOrParentFolderHasStereoType(x.Folder, "ServerOnly")).ToList();

            return dtoModels;
        }

        private static bool FolderOrParentFolderHasStereoType(FolderModel folder, string name)
        {
            if (folder == null)
            {
                return false;
            }

            if (folder.Stereotypes.Any(x => x.Name == name))
            {
                return true;
            }

            return FolderOrParentFolderHasStereoType(folder.Folder, name);
        }
    }
}
