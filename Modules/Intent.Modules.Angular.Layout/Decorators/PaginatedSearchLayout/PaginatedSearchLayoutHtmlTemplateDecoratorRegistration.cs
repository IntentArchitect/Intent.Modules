using Intent.Engine;
using Intent.Modules.Angular.Templates.Component.AngularComponentHtmlTemplate;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Angular.Layout.Decorators.PaginatedSearchLayout
{
    public class PaginatedSearchLayoutHtmlTemplateDecoratorRegistration : DecoratorRegistration<AngularComponentHtmlTemplate, IOverwriteDecorator>
    {
        public override string DecoratorId => PaginatedSearchLayoutHtmlTemplateDecorator.DecoratorId;

        public override IOverwriteDecorator CreateDecoratorInstance(AngularComponentHtmlTemplate template, IApplication application)
        {
            return new PaginatedSearchLayoutHtmlTemplateDecorator(template);
        }
    }
}