using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common
{
    [FixFor_Version4("Align these interfaces with the standard process flow of the SF")]
    public interface ITemplatePostConfigurationHook
    {
        void OnConfigured();
    }
}