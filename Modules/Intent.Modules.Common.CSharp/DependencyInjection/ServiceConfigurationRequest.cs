using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.CSharp.DependencyInjection;

public class ServiceConfigurationRequest
{
    private ServiceConfigurationRequest(
        IntentTemplateBase sourceConfigurationTemplate,
        string extensionMethodName,
        IEnumerable<string> extensionMethodParameterList)
    {
        SourceConfigurationTemplate = sourceConfigurationTemplate;
        ExtensionMethodName = extensionMethodName;
        ExtensionMethodParameterList = extensionMethodParameterList ?? Enumerable.Empty<string>();
    }

    public IntentTemplateBase SourceConfigurationTemplate { get; }
    public string ExtensionMethodName { get; }
    public IEnumerable<string> ExtensionMethodParameterList { get; }

    public int Priority { get; private set; }
    public bool IsHandled { get; private set; }

    public static ServiceConfigurationRequest ForExtensionMethod(
        IntentTemplateBase sourceConfigurationTemplate,
        string extensionMethodName,
        params string[] extensionMethodParameterList)
    {
        return new ServiceConfigurationRequest(
            sourceConfigurationTemplate: sourceConfigurationTemplate, 
            extensionMethodName: extensionMethodName,
            extensionMethodParameterList: extensionMethodParameterList);
    }

    public ServiceConfigurationRequest WithPriority(int priority)
    {
        Priority = priority;
        return this;
    }

    public void MarkAsHandled()
    {
        IsHandled = true;
    }
}

public static class ServiceConfigurationParameterType
{
    public const string Configuration = "Microsoft.Extensions.Configuration.IConfiguration";
}