using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.DependencyInjection;

/// <summary>
/// Request that the Hosting application's application builder
/// register a given extension method call that will delegate
/// certain pipeline setup responsibility to that method. 
/// </summary>
/// <example>
/// This will produce the equivalent of:
/// app.UseHttpsRedirection();
/// </example>
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
    // REFERENCE NOTE: This class is very similar to ServiceConfigurationRequest; whatever
    // change you make here, see if it is possible to make it there as well.
    
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
    /// Get additional dependencies.
    /// </summary>
    public IEnumerable<ITemplateDependency> TemplateDependencies => _templateDependencies;
    
    /// <summary>
    /// Get namespaces to be included in using directives.
    /// </summary>
    public IEnumerable<string> RequiredNamespaces => _requiredNamespaces;
    
    /// <summary>
    /// Register a given extension method that will configure the application pipeline
    /// for a given aspect.
    /// </summary>
    /// <param name="extensionMethodName">
    /// The extension method name only.
    /// <example>UseHttpsRedirection</example>
    /// </param>
    /// <param name="extensionMethodParameterList">
    /// If required, supply a list of expected types that your extension method will require
    /// as input parameters. See (<see cref="ParameterType"/>) to know what is available.
    /// <example>
    /// [ "<see cref="ApplicationBuilderRegistrationRequest.ParameterType.Configuration"/>" ] for inserting the "configuration" variable
    /// as the first and only parameter for the extension method.
    /// </example>
    /// </param>
    public static ApplicationBuilderRegistrationRequest ToRegister(
        string extensionMethodName,
        params string[] extensionMethodParameterList)
    {
        return new ApplicationBuilderRegistrationRequest(extensionMethodName, extensionMethodParameterList);
    }
    
    /// <summary>
    /// Supply a priority that will determine the order in which this gets registered.  
    /// </summary>
    public ApplicationBuilderRegistrationRequest WithPriority(int priority)
    {
        Priority = priority;
        return this;
    }
    
    /// <summary>
    /// Extension methods may require the template instance that represents the class
    /// that holds the extension method in question in order to properly import it in the file
    /// where the application builder is being used. 
    /// </summary>
    public ApplicationBuilderRegistrationRequest HasDependency(ITemplate template)
    {
        _templateDependencies.Add(TemplateDependency.OnTemplate(template));
        return this;
    }
    
    /// <summary>
    /// Supply namespaces that will be used for including using directives. 
    /// </summary>
    public ApplicationBuilderRegistrationRequest RequiresUsingNamespaces(params string[] namespaces)
    {
        _requiredNamespaces.AddRange(namespaces);
        return this;
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
        /// <summary>
        /// Access the AspNet Core's Web Host Environment variables service.
        /// </summary>
        public const string WebHostEnvironment = "Microsoft.AspNetCore.Hosting.IWebHostEnvironment";
        /// <summary>
        /// Access the Generic Host's Environment variables service.
        /// </summary>
        public const string HostEnvironment = "Microsoft.Extensions.Hosting.IHostEnvironment";
    }
}