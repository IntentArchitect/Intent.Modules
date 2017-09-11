using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.AngularJs.Auth.ImplicitAuth.Templates.AuthModule
{
    public interface IAngularImplicitAuthModuleDecorator : ITemplateDecorator
    {
        string AdditionalElseIf();
        void OnIdentityServerAvailable(string baseUrl, string authorityUrl);
    }
}