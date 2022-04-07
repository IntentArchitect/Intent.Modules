using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.TemplateExtensions
{
    public interface IModuleBuilderTemplate : ITemplateWithModel
    {
        string Id { get; }
        string GetTemplateId();
        string GetModelType();
        string GetRole();
        string TemplateType();
    }

    [FixFor_Version4("Merge with " + nameof(IModuleBuilderTemplate))]
    public interface IModuleBuilderTemplateWithDefaultLocation : IModuleBuilderTemplate
    {
        string GetDefaultLocation();
    }
}