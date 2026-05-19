using System.Collections.Generic;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Registrations;

namespace Intent.Modelers.CodebaseStructure.OutputTargets
{
    public class OutputTargetRegistration : IOutputTargetRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public OutputTargetRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void Register(IOutputTargetRegistry registry, IApplication application)
        {
            var rootFolders = _metadataManager.CodebaseStructure(application).GetRootFolderModels();
            foreach (var rootFolder in rootFolders)
            {
                registry.RegisterOutputTarget(rootFolder.ToOutputTargetConfig());
                foreach (var f in rootFolder.Folders)
                {
                    Register(registry, f);
                }
            }
        }

        private static void Register(IOutputTargetRegistry registry, FolderModel folder)
        {
            var outputTargetConfig = folder.ToOutputTargetConfig();

            registry.RegisterOutputTarget(outputTargetConfig);
            foreach (var child in folder.Folders.DetectDuplicates())
            {
                Register(registry, child);
            }
        }
    }

    
}
