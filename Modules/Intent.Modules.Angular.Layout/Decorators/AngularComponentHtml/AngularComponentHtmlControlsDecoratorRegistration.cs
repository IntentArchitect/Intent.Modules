using Intent.Engine;
using Intent.Modules.Angular.Templates.Component.AngularComponentHtmlTemplate;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Angular.Layout.Decorators.AngularComponentHtml
{
    public class AngularComponentHtmlControlsDecoratorRegistration : DecoratorRegistration<AngularComponentHtmlTemplate, IOverwriteDecorator>
    {
        public override string DecoratorId => AngularComponentHtmlControlsDecorator.DecoratorId;

        public override IOverwriteDecorator CreateDecoratorInstance(AngularComponentHtmlTemplate template, IApplication application)
        {
            return new AngularComponentHtmlControlsDecorator(template);
        }
    }
}