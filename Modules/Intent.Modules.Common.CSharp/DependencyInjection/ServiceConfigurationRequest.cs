using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.DependencyInjection;

public class ServiceConfigurationRequest
{
    private readonly List<ITemplateDependency> _templateDependencies = new List<ITemplateDependency>();
    private readonly List<string> _requiredNamespaces = new List<string>();
    
    private ServiceConfigurationRequest(
        string extensionMethodName,
        IEnumerable<string> extensionMethodParameterList)
    {
        ExtensionMethodName = extensionMethodName;
        ExtensionMethodParameterList = extensionMethodParameterList ?? Enumerable.Empty<string>();
    }

    public string ExtensionMethodName { get; }
    public IEnumerable<string> ExtensionMethodParameterList { get; }
    public int Priority { get; private set; }
    public bool IsHandled { get; private set; }
    public string Concern { get; private set; }
    public IEnumerable<ITemplateDependency> TemplateDependencies => _templateDependencies;
    public IEnumerable<string> RequiredNamespaces => _requiredNamespaces;

    public static ServiceConfigurationRequest ToRegister(
        string extensionMethodName,
        params string[] extensionMethodParameterList)
    {
        return new ServiceConfigurationRequest(extensionMethodName, extensionMethodParameterList);
    }
    
    public ServiceConfigurationRequest WithPriority(int priority)
    {
        Priority = priority;
        return this;
    }
    
    public ServiceConfigurationRequest HasDependency(ITemplate template)
    {
        _templateDependencies.Add(TemplateDependency.OnTemplate(template));
        return this;
    }
    
    public ServiceConfigurationRequest RequiresUsingNamespaces(params string[] namespaces)
    {
        _requiredNamespaces.AddRange(namespaces);
        return this;
    }
    
    public ServiceConfigurationRequest ForConcern(string concern)
    {
        Concern = concern;
        return this;
    }

    public void MarkAsHandled()
    {
        IsHandled = true;
    }
    
    public static class ParameterType
    {
        public const string Configuration = "Microsoft.Extensions.Configuration.IConfiguration";
    }
}