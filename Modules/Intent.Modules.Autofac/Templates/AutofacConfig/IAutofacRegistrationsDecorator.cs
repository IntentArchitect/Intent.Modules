using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.Autofac.Templates.AutofacConfig
{
    public interface IAutofacRegistrationsDecorator : ITemplateDecorator, IDeclareUsings
    {
        string Registrations();
    }
}