using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    [IntentManaged(Mode.Merge)]
    public static class ApiMetadataProviderExtensions
    {
        [IntentManaged(Mode.Ignore)]
        public static IList<IVisualStudioProject> GetAllProjectModels(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.VisualStudio(application).GetASPNETCoreWebApplicationModels().Cast<IVisualStudioProject>()
                .Concat(metadataManager.VisualStudio(application).GetASPNETWebApplicationNETFrameworkModels())
                .Concat(metadataManager.VisualStudio(application).GetClassLibraryNETCoreModels())
                .Concat(metadataManager.VisualStudio(application).GetClassLibraryNETFrameworkModels())
                .Concat(metadataManager.VisualStudio(application).GetConsoleAppNETFrameworkModels())
                .Concat(metadataManager.VisualStudio(application).GetWCFServiceApplicationModels())
                .ToList();
        }


        public static IList<ASPNETCoreWebApplicationModel> GetASPNETCoreWebApplicationModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ASPNETCoreWebApplicationModel.SpecializationTypeId)
                .Select(x => new ASPNETCoreWebApplicationModel(x))
                .ToList();
        }

        public static IList<ASPNETWebApplicationNETFrameworkModel> GetASPNETWebApplicationNETFrameworkModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ASPNETWebApplicationNETFrameworkModel.SpecializationTypeId)
                .Select(x => new ASPNETWebApplicationNETFrameworkModel(x))
                .ToList();
        }

        public static IList<ClassLibraryNETCoreModel> GetClassLibraryNETCoreModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ClassLibraryNETCoreModel.SpecializationTypeId)
                .Select(x => new ClassLibraryNETCoreModel(x))
                .ToList();
        }

        public static IList<ClassLibraryNETFrameworkModel> GetClassLibraryNETFrameworkModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ClassLibraryNETFrameworkModel.SpecializationTypeId)
                .Select(x => new ClassLibraryNETFrameworkModel(x))
                .ToList();
        }

        public static IList<ConsoleAppNETFrameworkModel> GetConsoleAppNETFrameworkModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ConsoleAppNETFrameworkModel.SpecializationTypeId)
                .Select(x => new ConsoleAppNETFrameworkModel(x))
                .ToList();
        }

        public static IList<WCFServiceApplicationModel> GetWCFServiceApplicationModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(WCFServiceApplicationModel.SpecializationTypeId)
                .Select(x => new WCFServiceApplicationModel(x))
                .ToList();
        }

        public static IList<NETCoreVersionModel> GetNETCoreVersionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(NETCoreVersionModel.SpecializationTypeId)
                .Select(x => new NETCoreVersionModel(x))
                .ToList();
        }

        public static IList<NETFrameworkVersionModel> GetNETFrameworkVersionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(NETFrameworkVersionModel.SpecializationTypeId)
                .Select(x => new NETFrameworkVersionModel(x))
                .ToList();
        }

        public static IList<SolutionFolderModel> GetSolutionFolderModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(SolutionFolderModel.SpecializationTypeId)
                .Select(x => new SolutionFolderModel(x))
                .ToList();
        }

        public static IList<FolderModel> GetFolderModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(FolderModel.SpecializationTypeId)
                .Select(x => new FolderModel(x))
                .ToList();
        }

    }
}