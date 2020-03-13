using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IFileTemplate : IMetadataModel, IHasStereotypes, IHasFolder, IModuleBuilderElement
    {
        string Name { get; }
        string FileExtension { get; }
    }
}