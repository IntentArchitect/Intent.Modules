using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.Unity.Templates.UnityConfig
{
    public interface IUnityRegistrationsDecorator : ITemplateDecorator, IDeclareUsings
    {
        string Registrations();
    }
}