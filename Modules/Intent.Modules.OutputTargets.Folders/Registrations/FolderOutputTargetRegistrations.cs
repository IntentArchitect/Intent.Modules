using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.OutputTargets.Folders.Api;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Modules.OutputTargets.Folders.Registrations
{
    public class FolderOutputTargetRegistrations : IOutputTargetRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public FolderOutputTargetRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }
        public void Register(IOutputTargetRegistry registry, IApplication application)
        {
            var rootFolders = _metadataManager.FolderStructure(application).GetRootLocationPackageModels();
            foreach (var rootFolder in rootFolders)
            {
                registry.RegisterOutputTarget(rootFolder.ToOutputTargetConfig());
            }
            var folders = _metadataManager.FolderStructure(application).GetFolderModels();
            foreach (var folder in folders)
            {
                registry.RegisterOutputTarget(folder.ToOutputTargetConfig());
            }
        }
    }

    public static class ApiMetadataProviderExtensions
    {
        public static IList<EnumModel> GetEnumModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(EnumModel.SpecializationTypeId)
                .Select(x => new EnumModel(x))
                .ToList();
        }

        public static IList<FolderExtensionModel> GetFolderModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(FolderModel.SpecializationTypeId)
                .Select(x => new FolderExtensionModel(x))
                .ToList();
        }

        public static IList<TypeDefinitionModel> GetTypeDefinitionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
                .Select(x => new TypeDefinitionModel(x))
                .ToList();
        }

    }

    public class FolderExtensionModel : FolderModel
    {
        public FolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<RoleModel> Roles => _element.ChildElements
            .Where(x => x.SpecializationType == RoleModel.SpecializationType)
            .Select(x => new RoleModel(x))
            .ToList();


        public IOutputTargetConfig ToOutputTargetConfig()
        {
            return new FolderOutputTarget(this);
        }
    }

    public class RootFolderOutputTarget : IOutputTargetConfig
    {
        private readonly RootLocationPackageModel _model;

        public RootFolderOutputTarget(RootLocationPackageModel model)
        {
            _model = model;
        }

        public IEnumerable<IStereotype> Stereotypes => _model.Stereotypes;
        public string Id => _model.Id;
        public string Type => "Folder";
        public string Name => _model.Name;
        public string RelativeLocation => "";
        public string ParentId => null;
        public IEnumerable<string> SupportedFrameworks => new string[0];
        public IEnumerable<IOutputTargetRole> Roles => _model.Roles.Select(x => new FolderRole(x.Name)).ToList<IOutputTargetRole>();
    }

    public class FolderOutputTarget : IOutputTargetConfig
    {
        private readonly FolderExtensionModel _model;

        public FolderOutputTarget(FolderExtensionModel model)
        {
            _model = model;
        }

        public IEnumerable<IStereotype> Stereotypes => _model.Stereotypes;
        public string Id => _model.Id;
        public string Type => "Folder";
        public string Name => _model.Name;
        public string RelativeLocation => _model.Name;
        public string ParentId => _model.InternalElement.ParentId;
        public IEnumerable<string> SupportedFrameworks => new string[0];
        public IEnumerable<IOutputTargetRole> Roles => _model.Roles.Select(x => new FolderRole(x.Name)).ToList<IOutputTargetRole>();
    }

    public class FolderRole : IOutputTargetRole
    {
        public FolderRole(string id)
        {
            Id = id;
            RelativeLocation = "";
            RequiredFrameworks = new string[0];
        }
        public string Id { get; }
        public string RelativeLocation { get; }
        public IEnumerable<string> RequiredFrameworks { get; }
    }
}
