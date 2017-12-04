using Intent.MetaModel.Common;
using Intent.MetaModel.DTO;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using IApplication = Intent.SoftwareFactory.Engine.IApplication;

namespace Intent.Modules.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO
{
    [Description("Intent Typescript ServiceAgent DTO - Remote")]
    public class RemoteRegistrations : ModelTemplateRegistrationBase<DTOModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public RemoteRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;

        }

        public override string TemplateId => TypescriptDtoTemplate.RemoteIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, DTOModel model)
        {
            return new TypescriptDtoTemplate(TemplateId, project, model);
        }

        public override IEnumerable<DTOModel> GetModels(IApplication application)
        {
            var dtoModels = _metaDataManager.GetMetaData<DTOModel>(new MetaDataIdentifier("DTO"));

            dtoModels = dtoModels.Where(x => x.GetPropertyValue("Consumers", "CommaSeperatedList", "").Split(',').Any(a => a.Trim() == application.ApplicationName));

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