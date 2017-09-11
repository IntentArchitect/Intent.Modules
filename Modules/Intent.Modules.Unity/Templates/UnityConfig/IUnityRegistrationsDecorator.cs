using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.Unity.Templates.UnityConfig
{
    public interface IUnityRegistrationsDecorator : ITemplateDecorator, IDeclareUsings
    {
        string Registrations();
    }
}