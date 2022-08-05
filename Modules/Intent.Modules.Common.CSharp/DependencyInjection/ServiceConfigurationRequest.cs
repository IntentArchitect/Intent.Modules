using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.DependencyInjection;

/// <summary>
/// Request that the Dependency Injection Container be used to configure
/// more advanced use cases other than the type resolution done by
/// <see cref="ContainerRegistrationRequest"/>.
/// </summary>
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

    /// <summary>
    /// Get extension method name.
    /// </summary>
    public string ExtensionMethodName { get; }
    
    // NOTE FOR FUTURE: In the event that we need to include more complex parameters
    // such as lambda expressions, we could always change the type from string to a
    // more complex type and make use of string implicit operators to make it backward
    // compatible.
    // DESIGN NOTE: I would advise we try not to go that route if possible since the
    // extension methods being generated are supposed to do the more complex configuration
    // leaving these extension methods very simple as they are right now.
    // REFERENCE NOTE: This class is very similar to ApplicationBuilderRegistrationRequest;
    // whatever change you make here, see if it is possible to make it there as well.
    
    /// <summary>
    /// Get list of types used to supply to the extension method as input parameters.
    /// </summary>
    public IEnumerable<string> ExtensionMethodParameterList { get; }
    
    /// <summary>
    /// Get priority that will determine the order in which this request
    /// will be registered in.
    /// </summary>
    public int Priority { get; private set; }
    
    /// <summary>
    /// Has a Container handler already handled this registration? If so, ignore it.
    /// </summary>
    public bool IsHandled { get; private set; }
    
    /// <summary>
    /// Get container configuration destination.
    /// </summary>
    public string Concern { get; private set; }
    
    /// <summary>
    /// Get additional dependencies.
    /// </summary>
    public IEnumerable<ITemplateDependency> TemplateDependencies => _templateDependencies;
    
    /// <summary>
    /// Get namespaces to be included in using directives.
    /// </summary>
    public IEnumerable<string> RequiredNamespaces => _requiredNamespaces;

    /// <summary>
    /// Register a given extension method that will configure a given aspect.
    /// </summary>
    /// <param name="extensionMethodName">
    /// The extension method name only.
    /// <example>AddInfrastructure</example>
    /// </param>
    /// <param name="extensionMethodParameterList">
    /// If required, supply a list of expected types that your extension method will require
    /// as input parameters. See (<see cref="ParameterType"/>) to know what is available.
    /// <example>
    /// [ "<see cref="ApplicationBuilderRegistrationRequest.ParameterType.Configuration"/>" ] for inserting the "configuration" variable
    /// as the first and only parameter for the extension method.
    /// </example>
    /// </param>
    public static ServiceConfigurationRequest ToRegister(
        string extensionMethodName,
        params string[] extensionMethodParameterList)
    {
        return new ServiceConfigurationRequest(extensionMethodName, extensionMethodParameterList);
    }
    
    /// <summary>
    /// Supply a priority that will determine the order in which this gets registered.  
    /// </summary>
    public ServiceConfigurationRequest WithPriority(int priority)
    {
        Priority = priority;
        return this;
    }
    
    /// <summary>
    /// Some resolution type configurations may require additional types
    /// which can be supplied by making use of the actual template instance that
    /// represents them. 
    /// </summary>
    public ServiceConfigurationRequest HasDependency(ITemplate template)
    {
        _templateDependencies.Add(TemplateDependency.OnTemplate(template));
        return this;
    }
    
    /// <summary>
    /// Supply namespaces that will be used for including using directives. 
    /// </summary>
    public ServiceConfigurationRequest RequiresUsingNamespaces(params string[] namespaces)
    {
        _requiredNamespaces.AddRange(namespaces);
        return this;
    }
    
    /// <summary>
    /// Declare optionally a container configuration destination. 
    /// </summary>
    /// <example>Application <i>or</i> Infrastructure</example>
    public ServiceConfigurationRequest ForConcern(string concern)
    {
        Concern = concern;
        return this;
    }

    /// <summary>
    /// In the event that another Container registration location has handled
    /// this request, it needs to mark it as handled.
    /// </summary>
    public void MarkAsHandled()
    {
        IsHandled = true;
    }
    
    /// <summary>
    /// Available parameter types to be used for supplying what input your extension method will require.
    /// </summary>
    public static class ParameterType
    {
        /// <summary>
        /// Access the .NET Configuration service.
        /// </summary>
        public const string Configuration = "Microsoft.Extensions.Configuration.IConfiguration";
    }
}