using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModulePackageFolder : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        IElement Parent { get; }
        string FolderPath { get; }
    }
}