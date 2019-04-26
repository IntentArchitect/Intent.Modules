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

namespace Intent.Modules.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO
{
    [Description("Intent Typescript ServiceAgent DTO - Local")]
    public class LocalRegistrations : ModelTemplateRegistrationBase<IDTOModel>
    {
        private readonly IMetadataManager _metaDataManager;

        public LocalRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;

        }

        public override string TemplateId => TypescriptDtoTemplate.LocalIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, IDTOModel model)
        {
            return new TypescriptDtoTemplate(TemplateId, project, model);
        }

        public override IEnumerable<IDTOModel> GetModels(Engine.IApplication application)
        {
            var dtoModels = _metaDataManager.GetDTOs(application);

            // TODO JL: Temp, filter out ones for server only, will ultimately get replaced with concept of client applications in the future
            dtoModels = dtoModels.Where(x => x.Stereotypes.All(s => s.Name != "ServerOnly") && !FolderOrParentFolderHasStereoType(x.Folder, "ServerOnly"));

            return dtoModels.ToList();
        }

        private static bool FolderOrParentFolderHasStereoType(IFolder folder, string name)
        {
            if (folder == null)
            {
                return false;
            }

            if (folder.Stereotypes.Any(x => x.Name == name))
            {
                return true;
            }

            return FolderOrParentFolderHasStereoType(folder.ParentFolder, name);
        }
    }
}
