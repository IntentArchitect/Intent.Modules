using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modelers.CodebaseStructure.OutputTargets
{
    public class RootFolderOutputTarget : IOutputTargetConfig
    {
        private readonly RootFolderModel _model;

        public RootFolderOutputTarget(RootFolderModel model)
        {
            _model = model;
        }

        public IEnumerable<IStereotype> Stereotypes => _model.Stereotypes;
        public string Id => _model.Id;
        public string Type => "Folder";
        public string Name => _model.Name;

        // Owned by Intent.Modules.VisualStudio.Projects' "Root Folder Options" stereotype - read generically
        // by stereotype name (rather than a package reference, which would invert the dependency direction)
        // so this Root Folder's own OutputTarget shifts together with everything else that resolves relative
        // to it (Projects, Solution Folders), instead of staying behind at the unshifted location.
        public string RelativeLocation => _model.HasRootFolderOptions() ? _model.GetRootFolderOptions()?.RelativeLocation() : string.Empty;

        public string ParentId => null;
        public IEnumerable<string> SupportedFrameworks => new string[0];
        public IEnumerable<IOutputTargetRole> Roles => _model.OutputAnchors;
        public IEnumerable<IOutputTargetTemplate> Templates => _model.TemplateOutputs;
        public IDictionary<string, object> Metadata { get; }
    }
}
