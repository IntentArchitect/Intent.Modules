using Intent.Modules.Common.Java.Events;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.Java.Templates;

public static class IntentTemplateBaseExtensions
{
    public static void ApplyApplicationProperty(
        this IntentTemplateBase template,
        string name,
        string value,
        string profile = null)
    {
        template.ExecutionContext.EventDispatcher.Publish(new ApplicationPropertyRequiredEvent(name, value, profile));
    }
}