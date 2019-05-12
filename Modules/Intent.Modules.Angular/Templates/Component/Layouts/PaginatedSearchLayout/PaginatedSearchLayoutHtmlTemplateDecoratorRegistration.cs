using System.CodeDom;
using Intent.Engine;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Angular.Templates.Component.Layouts.PaginatedSearchLayout
{
    public class PaginatedSearchLayoutHtmlTemplateDecoratorRegistration : DecoratorRegistration<AngularComponentHtmlTemplate.AngularComponentHtmlTemplate, IOverwriteDecorator>
    {
        private readonly IMetadataManager _metadataManager;
        public override string DecoratorId => PaginatedSearchLayoutHtmlTemplateDecorator.DecoratorId;

        public PaginatedSearchLayoutHtmlTemplateDecoratorRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override IOverwriteDecorator CreateDecoratorInstance(AngularComponentHtmlTemplate.AngularComponentHtmlTemplate template, IApplication application)
        {
            return new PaginatedSearchLayoutHtmlTemplateDecorator(template, _metadataManager);
        }
    }
}