using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Intent.Configuration;
using Intent.Engine;
using Intent.Exceptions;
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
            foreach (var folder in folders.DetectDuplicates())
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
        public IEnumerable<IOutputTargetRole> Roles => _model.Roles;
        public IEnumerable<IOutputTargetTemplate> Templates => _model.TemplateOutputs;
        public IDictionary<string, object> Metadata { get; }
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
        public IEnumerable<IOutputTargetRole> Roles => _model.Roles;
        public IEnumerable<IOutputTargetTemplate> Templates => _model.TemplateOutputs;
        public IDictionary<string, object> Metadata { get; }
    }
    
    internal static class OutputTargetRegistrationExtensions
    {
        public static IEnumerable<FolderExtensionModel> DetectDuplicates(this IEnumerable<FolderExtensionModel> sequence)
        {
            var folderNameSet = new HashSet<string>();

            foreach (var folderModel in sequence)
            {
                if (!folderNameSet.Add($"{folderModel.Folder?.Id ?? string.Empty}-{folderModel.Name}"))
                {
                    throw new ElementException(folderModel.InternalElement, $"Duplicate Folder found at same location.");
                }

                yield return folderModel;
            }
        }
    }
}
