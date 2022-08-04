using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.DependencyInjection
{
    /// <summary>
    /// Request that the Dependency Injection Container register a given type
    /// (optionally with a contract or interface) to be resolved at runtime.
    /// </summary>
    /// <example>
    /// This should produce the equivalent of:
    /// services.AddTransient&lt;IItemRepository, ItemRepository&gt;();
    /// </example>
    public class ContainerRegistrationRequest
    {
        private readonly List<ITemplateDependency> _templateDependencies = new List<ITemplateDependency>();
        private readonly List<string> _requiredNamespaces = new List<string>();

        private ContainerRegistrationRequest(IClassProvider concreteType)
        {
            ConcreteType = concreteType.FullTypeName();
            Lifetime = LifeTime.Transient;
            _templateDependencies.Add(TemplateDependency.OnTemplate(concreteType));
        }

        private ContainerRegistrationRequest(string concreteType)
        {
            ConcreteType = concreteType;
            Lifetime = LifeTime.Transient;
        }

        /// <summary>
        /// Register a given type using the Template that represents that type. 
        /// </summary>
        public static ContainerRegistrationRequest ToRegister(IClassProvider concreteType)
        {
            return new ContainerRegistrationRequest(concreteType);
        }

        /// <summary>
        /// Register a given concrete type using its fully qualified name. 
        /// </summary>
        public static ContainerRegistrationRequest ToRegister(string concreteType)
        {
            return new ContainerRegistrationRequest(concreteType);
        }

        /// <summary>
        /// Associate an interface for the concrete type using the template instance for the interface.
        /// </summary>
        public ContainerRegistrationRequest ForInterface(IClassProvider interfaceType)
        {
            InterfaceType = interfaceType.FullTypeName();
            _templateDependencies.Add(TemplateDependency.OnTemplate(interfaceType));
            return this;
        }

        /// <summary>
        /// Associate an interface for the concrete type using its fully qualified name.
        /// </summary>
        public ContainerRegistrationRequest ForInterface(string interfaceType)
        {
            InterfaceType = interfaceType;
            return this;
        }

        /// <summary>
        /// Declare optionally a container configuration destination. 
        /// </summary>
        /// <example>Application or Infrastructure</example>
        public ContainerRegistrationRequest ForConcern(string concern)
        {
            Concern = concern;
            return this;
        }

        /// <summary>
        /// Set the resolved instance lifetime using the options provided in
        /// <see cref="ContainerRegistrationRequest.LifeTime"/>.
        /// </summary>
        public ContainerRegistrationRequest WithLifeTime(string lifetime)
        {
            Lifetime = lifetime;
            return this;
        }

        /// <summary>
        /// Set the resolved type instance's lifetime to last the duration of a service call. 
        /// </summary>
        public ContainerRegistrationRequest WithPerServiceCallLifeTime()
        {
            Lifetime = LifeTime.PerServiceCall;
            return this;
        }

        /// <summary>
        /// Set the resolved type instance's lifetime to last the duration of the host application.
        /// </summary>
        public ContainerRegistrationRequest WithSingletonLifeTime()
        {
            Lifetime = LifeTime.Singleton;
            return this;
        }

        /// <summary>
        /// Upon resolving the interface type, use the service provider 
        /// to resolve the concrete type as the resolution type. 
        /// </summary>
        public ContainerRegistrationRequest WithResolveFromContainer()
        {
            ResolveFromContainer = true;
            return this;
        }

        /// <summary>
        /// Supply namespaces that will be used for including using directives. 
        /// </summary>
        public ContainerRegistrationRequest RequiresUsingNamespaces(params string[] namespaces)
        {
            _requiredNamespaces.AddRange(namespaces);
            return this;
        }

        /// <summary>
        /// Some resolution type configurations may require additional language types
        /// which can be supplied by making use of the actual template instance that
        /// represents them. 
        /// </summary>
        public ContainerRegistrationRequest HasDependency(ITemplate template)
        {
            _templateDependencies.Add(TemplateDependency.OnTemplate(template));
            return this;
        }

        /// <summary>
        /// Supply a priority that will determine the order in which this gets registered.  
        /// </summary>
        public ContainerRegistrationRequest WithPriority(int priority)
        {
            Priority = priority;
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
        /// Given namespaces to be included in using directives.
        /// </summary>
        public IEnumerable<string> RequiredNamespaces => _requiredNamespaces;

        /// <summary>
        /// Given container configuration destination.
        /// </summary>
        public string Concern { get; private set; }

        /// <summary>
        /// Given interface type used for resolving a concrete type.
        /// </summary>
        public string InterfaceType { get; private set; }

        /// <summary>
        /// Given concrete type used for type resolution.
        /// </summary>
        public string ConcreteType { get; }

        /// <summary>
        /// Given resolution object's lifetime configuration.
        /// </summary>
        public string Lifetime { get; private set; }

        /// <summary>
        /// Given priority that will determine the order in which this request
        /// will be registered in.
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// Given setting where upon resolving the interface type, that the service provider will be used 
        /// to resolve the concrete type as the resolution type. 
        /// </summary>
        public bool ResolveFromContainer { get; private set; }

        /// <summary>
        /// Given additional dependencies.
        /// </summary>
        public IEnumerable<ITemplateDependency> TemplateDependencies => _templateDependencies;

        /// <summary>
        /// Has a Container handler already handled this registration?
        /// </summary>
        public bool IsHandled { get; private set; }

        /// <summary>
        /// Available lifetime options when a type is resolved.
        /// </summary>
        public static class LifeTime
        {
            /// <summary>
            /// Created on each type resolution.
            /// </summary>
            public const string Transient = "Transient";
            /// <summary>
            /// Created once for the duration of the application host.
            /// </summary>
            public const string Singleton = "Singleton";
            /// <summary>
            /// Created once per duration of a service call.
            /// </summary>
            public const string PerServiceCall = "PerServiceCall";
        }
    }
}