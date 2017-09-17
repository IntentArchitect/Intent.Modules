using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.AngularJs.Auth.ImplicitAuth.Templates.AuthModule
{
    public interface IAngularImplicitAuthModuleDecorator : ITemplateDecorator
    {
        string AdditionalElseIf();
        void OnIdentityServerAvailable(string baseUrl, string authorityUrl);
    }
}