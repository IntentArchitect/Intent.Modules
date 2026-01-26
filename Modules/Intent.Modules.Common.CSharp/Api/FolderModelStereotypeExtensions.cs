#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Common.CSharp.Api;

/// <summary>
/// Changed to a manually maintained file after the Stereotype Definition's Target Mode was changed to "filter by function".
/// </summary>
public static class FolderModelStereotypeExtensions
{
    public static FolderOptions GetFolderOptions(this FolderModel model)
    {
        var stereotype = model.GetStereotype(FolderOptions.DefinitionId);
        return stereotype != null ? new FolderOptions(stereotype) : null;
    }

    public static bool HasFolderOptions(this FolderModel model)
    {
        return model.HasStereotype(FolderOptions.DefinitionId);
    }

    public static bool TryGetFolderOptions(this FolderModel model, out FolderOptions stereotype)
    {
        if (!model.HasFolderOptions())
        {
            stereotype = null;
            return false;
        }

        stereotype = new FolderOptions(model.GetStereotype(FolderOptions.DefinitionId));
        return true;
    }


    public class FolderOptions
    {
        private readonly IStereotype _stereotype;
        public const string DefinitionId = "66fd9e66-42c7-4ef9-a778-b68e009272b9";

        public FolderOptions(IStereotype stereotype)
        {
            _stereotype = stereotype;
        }

        public string Name => _stereotype.Name;

        public bool NamespaceProvider()
        {
            return _stereotype.GetProperty<bool>("Namespace Provider");
        }

    }

}