using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modelers.CodebaseStructure.Api;
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
        public string RelativeLocation => "";
        public string ParentId => null;
        public IEnumerable<string> SupportedFrameworks => new string[0];
        public IEnumerable<IOutputTargetRole> Roles => _model.OutputAnchors;
        public IEnumerable<IOutputTargetTemplate> Templates => _model.TemplateOutputs;
        public IDictionary<string, object> Metadata { get; }
    }
}
