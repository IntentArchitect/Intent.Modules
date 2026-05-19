using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modelers.CodebaseStructure.OutputTargets;

internal class FolderOutputTarget : IOutputTargetConfig
{
    private readonly Intent.Modelers.CodebaseStructure.Api.FolderExtensionModel _model;

    public FolderOutputTarget(FolderModel model)
    {
        _model = new Intent.Modelers.CodebaseStructure.Api.FolderExtensionModel(model.InternalElement);

        // this is coming from Intent.Modules.Common.CSharp, but adding here for backwards compatibility
        if (model.GetStereotype("66fd9e66-42c7-4ef9-a778-b68e009272b9")
            ?.TryGetProperty("Namespace Provider", out var nsProvider) ?? false)
        {
            Metadata = new Dictionary<string, object>
            {
                { "Namespace Provider", nsProvider.Value?.Equals("true", StringComparison.InvariantCultureIgnoreCase) ?? true }
            };
        }

    }

    public IEnumerable<IStereotype> Stereotypes => _model.Stereotypes;
    public string Id => _model.Id;
    public string Type => _model.InternalElement.SpecializationType; // Folder
    public string Name => _model.Name;
    public string RelativeLocation => _model.Name;
    public string ParentId => _model.InternalElement.ParentId;

    public IEnumerable<string> SupportedFrameworks => [];
    public IEnumerable<IOutputTargetRole> Roles => _model.OutputAnchors;
    public IEnumerable<IOutputTargetTemplate> Templates => _model.TemplateOutputs.DetectDuplicates();
    public IDictionary<string, object> Metadata { get; }
}
