using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Eventing;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Unity.Templates.PerServiceCallLifetimeManager;

namespace Intent.Modules.Unity.Templates.UnityConfig
{
    partial class UnityConfigTemplate : CSharpTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasDecorators<IUnityRegistrationsDecorator>, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.Unity.Config";

        private IList<IUnityRegistrationsDecorator> _decorators = new List<IUnityRegistrationsDecorator>();
        private readonly IList<ContainerRegistration> _registrations = new List<ContainerRegistration>();

        public UnityConfigTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, null)
        {
            eventDispatcher.Subscribe(Constants.ContainerRegistrationEvent.EventId, Handle);
        }

        public IEnumerable<IOutputTarget> ApplicationProjects => ExecutionContext.OutputTargets.Select(x => x.GetTargetPath()[0]).Distinct();

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"UnityConfig",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
        
        public override string DependencyUsings => "";

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _registrations
                .Where(x => x.InterfaceType != null && x.InterfaceTypeTemplateDependency != null)
                .Select(x => x.InterfaceTypeTemplateDependency)
                .Union(_registrations
                    .Where(x => x.ConcreteTypeTemplateDependency != null)
                    .Select(x => x.ConcreteTypeTemplateDependency))
                .ToList();
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new INugetPackageInfo[]
            {
                NugetPackages.Unity,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public string Registrations()
        {
            var registrations = _registrations
                .Where(x => x.InterfaceType != null || !x.Lifetime.Equals(Constants.ContainerRegistrationEvent.TransientLifetime, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            var output = registrations.Any() ? registrations.Select(GetRegistrationString).Aggregate((x, y) => x + y) : string.Empty;

            return output + Environment.NewLine + GetDecorators().Aggregate(x => x.Registrations());
        }

        private string GetRegistrationString(ContainerRegistration x)
        {
            return x.InterfaceType != null 
                ? $"{Environment.NewLine}            container.RegisterType<{NormalizeNamespace(x.InterfaceType)}, {NormalizeNamespace(x.ConcreteType)}>({GetLifetimeManager(x)});" 
                : $"{Environment.NewLine}            container.RegisterType<{NormalizeNamespace(x.ConcreteType)}>({GetLifetimeManager(x)});";
        }

        private string GetLifetimeManager(ContainerRegistration registration)
        {
            switch (registration.Lifetime)
            {
                case Constants.ContainerRegistrationEvent.SingletonLifetime:
                    return "new ContainerControlledLifetimeManager()";
                case Constants.ContainerRegistrationEvent.PerServiceCallLifetime:
                    return $"new {Project.Application.FindTemplateInstance<IClassProvider>(TemplateDependency.OnTemplate(PerServiceCallLifetimeManagerTemplate.Identifier)).ClassName}()";
                case Constants.ContainerRegistrationEvent.TransientLifetime:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }

        public void AddDecorator(IUnityRegistrationsDecorator decorator)
        {
            _decorators.Add(decorator);

        }

        public IEnumerable<IUnityRegistrationsDecorator> GetDecorators()
        {
            return _decorators;
        }

        public string GetUsingsFromDecorators()
        {
            return string.Join(Environment.NewLine, _decorators
                .SelectMany(s => s.DeclareUsings())
                .Distinct()
                .Select(s => $"using {s};"));
        }

        private void Handle(ApplicationEvent @event)
        {
            _registrations.Add(new ContainerRegistration(
                interfaceType: @event.TryGetValue(ContainerRegistrationEvent.InterfaceTypeKey),
                concreteType: @event.GetValue(ContainerRegistrationEvent.ConcreteTypeKey),
                lifetime: @event.TryGetValue(ContainerRegistrationEvent.LifetimeKey),
                interfaceTypeTemplateDependency: @event.TryGetValue(ContainerRegistrationEvent.InterfaceTypeTemplateIdKey) != null ? TemplateDependency.OnTemplate(@event.TryGetValue(ContainerRegistrationEvent.InterfaceTypeTemplateIdKey)) : null,
                concreteTypeTemplateDependency: @event.TryGetValue(ContainerRegistrationEvent.ConcreteTypeTemplateIdKey) != null ? TemplateDependency.OnTemplate(@event.TryGetValue(ContainerRegistrationEvent.ConcreteTypeTemplateIdKey)) : null));
        }
    }

    internal class ContainerRegistration
    {
        public ContainerRegistration(string interfaceType, string concreteType, string lifetime, ITemplateDependency interfaceTypeTemplateDependency, ITemplateDependency concreteTypeTemplateDependency)
        {
            InterfaceType = interfaceType;
            ConcreteType = concreteType;
            Lifetime = lifetime ?? Constants.ContainerRegistrationEvent.TransientLifetime;
            InterfaceTypeTemplateDependency = interfaceTypeTemplateDependency;
            ConcreteTypeTemplateDependency = concreteTypeTemplateDependency;
        }

        public string InterfaceType { get; private set; }
        public string ConcreteType { get; private set; }
        public string Lifetime { get; private set; }
        public ITemplateDependency InterfaceTypeTemplateDependency { get; private set; }
        public ITemplateDependency ConcreteTypeTemplateDependency { get; }
    }
}
