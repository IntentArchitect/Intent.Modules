using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.DependencyInjection;

public class ApplicationBuilderRegistrationRequest
{
    private readonly List<ITemplateDependency> _templateDependencies = new List<ITemplateDependency>();
    private readonly List<string> _requiredNamespaces = new List<string>();

    private ApplicationBuilderRegistrationRequest(string extensionMethodName,
        IEnumerable<string> extensionMethodParameterList)
    {
        ExtensionMethodName = extensionMethodName;
        ExtensionMethodParameterList = extensionMethodParameterList ?? Enumerable.Empty<string>();
    }
    
    public string ExtensionMethodName { get; }
    public IEnumerable<string> ExtensionMethodParameterList { get; }
    public int Priority { get; private set; }
    public bool IsHandled { get; private set; }
    public IEnumerable<ITemplateDependency> TemplateDependencies => _templateDependencies;
    public IEnumerable<string> RequiredNamespaces => _requiredNamespaces;
    
    public static ApplicationBuilderRegistrationRequest ToRegister(
        string extensionMethodName,
        params string[] extensionMethodParameterList)
    {
        return new ApplicationBuilderRegistrationRequest(extensionMethodName, extensionMethodParameterList);
    }
    
    public ApplicationBuilderRegistrationRequest WithPriority(int priority)
    {
        Priority = priority;
        return this;
    }
    
    public ApplicationBuilderRegistrationRequest HasDependency(ITemplate template)
    {
        _templateDependencies.Add(TemplateDependency.OnTemplate(template));
        return this;
    }
    
    public ApplicationBuilderRegistrationRequest RequiresUsingNamespaces(params string[] namespaces)
    {
        _requiredNamespaces.AddRange(namespaces);
        return this;
    }
    
    public void MarkAsHandled()
    {
        IsHandled = true;
    }

    public static class ParameterType
    {
        public const string Configuration = "Microsoft.Extensions.Configuration.IConfiguration";
        public const string WebHostEnvironment = "Microsoft.AspNetCore.Hosting.IWebHostEnvironment";
        public const string HostEnvironment = "Microsoft.Extensions.Hosting.IHostEnvironment";
    }
}