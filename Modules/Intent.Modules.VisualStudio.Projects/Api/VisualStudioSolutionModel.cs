using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    [IntentManaged(Mode.Merge)]
    public class VisualStudioSolutionModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Visual Studio Solution";
        public const string SpecializationTypeId = "07e7b690-a59d-4b72-8440-4308a121d32c";

        [IntentManaged(Mode.Ignore)]
        public VisualStudioSolutionModel(IPackage package)
        {
            if (!SpecializationTypeId.Equals(package.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from package with specialization type '{package.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            UnderlyingPackage = package;
        }

        public IPackage UnderlyingPackage { get; }
        public string Id => UnderlyingPackage.Id;
        public string Name => UnderlyingPackage.Name;
        public IEnumerable<IStereotype> Stereotypes => UnderlyingPackage.Stereotypes;

        public string FileLocation => UnderlyingPackage.FileLocation;

        public IList<ASPNETCoreWebApplicationModel> ASPNETCoreWebApplications => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == ASPNETCoreWebApplicationModel.SpecializationType)
            .Select(x => new ASPNETCoreWebApplicationModel(x))
            .ToList();

        public IList<ASPNETWebApplicationNETFrameworkModel> ASPNETWebApplicationNETFrameworks => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == ASPNETWebApplicationNETFrameworkModel.SpecializationType)
            .Select(x => new ASPNETWebApplicationNETFrameworkModel(x))
            .ToList();

        public IList<ClassLibraryNETCoreModel> ClassLibraryNETCores => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == ClassLibraryNETCoreModel.SpecializationType)
            .Select(x => new ClassLibraryNETCoreModel(x))
            .ToList();

        public IList<ClassLibraryNETFrameworkModel> ClassLibraryNETFrameworks => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == ClassLibraryNETFrameworkModel.SpecializationType)
            .Select(x => new ClassLibraryNETFrameworkModel(x))
            .ToList();

        public IList<SolutionFolderModel> Folders => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == SolutionFolderModel.SpecializationType)
            .Select(x => new SolutionFolderModel(x))
            .ToList();

        public IList<NETCoreVersionModel> NETCoreVersions => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == NETCoreVersionModel.SpecializationType)
            .Select(x => new NETCoreVersionModel(x))
            .ToList();

        public IList<NETFrameworkVersionModel> NETFrameworkVersions => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == NETFrameworkVersionModel.SpecializationType)
            .Select(x => new NETFrameworkVersionModel(x))
            .ToList();

        public IList<RoleModel> Roles => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == RoleModel.SpecializationType)
            .Select(x => new RoleModel(x))
            .ToList();

        public IList<WCFServiceApplicationModel> WCFServiceApplications => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == WCFServiceApplicationModel.SpecializationType)
            .Select(x => new WCFServiceApplicationModel(x))
            .ToList();

        public IList<TemplateOutputModel> TemplateOutputs => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == TemplateOutputModel.SpecializationType)
            .Select(x => new TemplateOutputModel(x))
            .ToList();
    }
}