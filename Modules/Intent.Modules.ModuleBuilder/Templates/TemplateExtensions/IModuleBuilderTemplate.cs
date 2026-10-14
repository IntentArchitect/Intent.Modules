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
        [System.Obsolete("Implement GetReviewImportance() instead.")]
        string GetSeverity() => null;
#pragma warning disable CS0618 // Falls back to templates built against the older API
        string GetReviewImportance() => GetSeverity();
#pragma warning restore CS0618
        string GetClassification() => null;
    }
}
