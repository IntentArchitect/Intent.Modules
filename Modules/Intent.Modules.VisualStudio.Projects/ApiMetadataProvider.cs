using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProvider", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public class ApiMetadataProvider
    {
        private readonly IMetadataManager _metadataManager;

        public ApiMetadataProvider(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public IList<ASPNETCoreWebApplicationModel> GetASPNETCoreWebApplicationModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Visual Studio", application.Id)
                .Where(x => x.SpecializationType == ASPNETCoreWebApplicationModel.SpecializationType)
                .Select(x => new ASPNETCoreWebApplicationModel(x))
                .ToList<ASPNETCoreWebApplicationModel>();
            return models;
        }

        public IList<ASPNETWebApplicationNETFrameworkModel> GetASPNETWebApplicationNETFrameworkModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Visual Studio", application.Id)
                .Where(x => x.SpecializationType == ASPNETWebApplicationNETFrameworkModel.SpecializationType)
                .Select(x => new ASPNETWebApplicationNETFrameworkModel(x))
                .ToList<ASPNETWebApplicationNETFrameworkModel>();
            return models;
        }

        public IList<ClassLibraryNETCoreModel> GetClassLibraryNETCoreModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Visual Studio", application.Id)
                .Where(x => x.SpecializationType == ClassLibraryNETCoreModel.SpecializationType)
                .Select(x => new ClassLibraryNETCoreModel(x))
                .ToList<ClassLibraryNETCoreModel>();
            return models;
        }

        public IList<ClassLibraryNETFrameworkModel> GetClassLibraryNETFrameworkModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Visual Studio", application.Id)
                .Where(x => x.SpecializationType == ClassLibraryNETFrameworkModel.SpecializationType)
                .Select(x => new ClassLibraryNETFrameworkModel(x))
                .ToList<ClassLibraryNETFrameworkModel>();
            return models;
        }

        public IList<ConsoleAppNETFrameworkModel> GetConsoleAppNETFrameworkModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Visual Studio", application.Id)
                .Where(x => x.SpecializationType == ConsoleAppNETFrameworkModel.SpecializationType)
                .Select(x => new ConsoleAppNETFrameworkModel(x))
                .ToList<ConsoleAppNETFrameworkModel>();
            return models;
        }

        public IList<FolderModel> GetFolderModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Visual Studio", application.Id)
                .Where(x => x.SpecializationType == FolderModel.SpecializationType)
                .Select(x => new FolderModel(x))
                .ToList<FolderModel>();
            return models;
        }

        public IList<RoleModel> GetRoleModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Visual Studio", application.Id)
                .Where(x => x.SpecializationType == RoleModel.SpecializationType)
                .Select(x => new RoleModel(x))
                .ToList<RoleModel>();
            return models;
        }

        public IList<WCFServiceApplicationModel> GetWCFServiceApplicationModels(IApplication application)
        {
            var models = _metadataManager.GetMetadata<IElement>("Visual Studio", application.Id)
                .Where(x => x.SpecializationType == WCFServiceApplicationModel.SpecializationType)
                .Select(x => new WCFServiceApplicationModel(x))
                .ToList<WCFServiceApplicationModel>();
            return models;
        }

    }
}