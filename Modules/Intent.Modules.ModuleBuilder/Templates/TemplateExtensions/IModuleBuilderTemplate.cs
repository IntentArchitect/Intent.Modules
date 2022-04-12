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
        string GetDefaultLocation();
    }
}