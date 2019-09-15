using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface ITemplateDependencyDefinition
    {
        string TemplateId { get; }
    }

    internal class TemplateDependencyDefinition : ITemplateDependencyDefinition
    {
        public TemplateDependencyDefinition(IElement templateDefinition)
        {
            TemplateId = templateDefinition.Name;
        }

        public string TemplateId { get; }
    }
}