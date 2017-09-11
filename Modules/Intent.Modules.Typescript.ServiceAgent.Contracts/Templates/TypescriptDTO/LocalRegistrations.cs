using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.Common;
using Intent.MetaModel.DTO;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using IApplication = Intent.SoftwareFactory.Engine.IApplication;

namespace Intent.Packages.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO
{
    [Description("Intent Typescript ServiceAgent DTO - Local")]
    public class LocalRegistrations : ModelTemplateRegistrationBase<DTOModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public LocalRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;

        }

        public override string TemplateId => TypescriptDtoTemplate.LocalIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, DTOModel model)
        {
            return new TypescriptDtoTemplate(TemplateId, project, model);
        }

        public override IEnumerable<DTOModel> GetModels(IApplication application)
        {
            var dtoModels = _metaDataManager.GetMetaData<DTOModel>(new MetaDataType("DTO")).Where(x => x.Application.Name == application.ApplicationName);

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
