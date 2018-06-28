using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Unity.Templates.PerServiceCallLifetimeManager;

namespace Intent.Modules.Unity.Templates.UnityConfig
{
    partial class UnityConfigTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasDecorators<IUnityRegistrationsDecorator>, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.Unity.Config";

        private IEnumerable<IUnityRegistrationsDecorator> _decorators;
        private readonly IList<ContainerRegistration> _registrations = new List<ContainerRegistration>();

        public UnityConfigTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, null)
        {
            eventDispatcher.Subscribe(Constants.ContainerRegistration.EventId, Handle);
        }

        public IEnumerable<IProject> ApplicationProjects => Project.Application.Projects;

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "UnityConfig",
                fileExtension: "cs",
                defaultLocationInProject: "Unity",
                className: "UnityConfig",
                @namespace: "${Project.ProjectName}.Unity"
                );
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
                .Where(x => x.InterfaceType != null || !x.Lifetime.Equals(Constants.ContainerRegistration.TransientLifetime, StringComparison.InvariantCultureIgnoreCase))
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
                case Constants.ContainerRegistration.SingletonLifetime:
                    return "new ContainerControlledLifetimeManager()";
                case Constants.ContainerRegistration.PerServiceCallLifetime:
                    return $"new {Project.Application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnTemplate(PerServiceCallLifetimeManagerTemplate.Identifier)).ClassName}()";
                case Constants.ContainerRegistration.TransientLifetime:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }

        public IEnumerable<IUnityRegistrationsDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        private void Handle(ApplicationEvent @event)
        {
            _registrations.Add(new ContainerRegistration(
                interfaceType: @event.TryGetValue("InterfaceType"), 
                concreteType: @event.GetValue("ConcreteType"), 
                lifetime: @event.TryGetValue("Lifetime"),
                interfaceTypeTemplateDependency: @event.TryGetValue("InterfaceTypeTemplateId") != null ? TemplateDependancy.OnTemplate(@event.TryGetValue("InterfaceTypeTemplateId")) : null,
                concreteTypeTemplateDependency: @event.TryGetValue("ConcreteTypeTemplateId") != null ? TemplateDependancy.OnTemplate(@event.TryGetValue("ConcreteTypeTemplateId")) : null));
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return _registrations
                .Where(x => x.InterfaceType != null && x.InterfaceTypeTemplateDependency != null)
                .Select(x => x.InterfaceTypeTemplateDependency)
                .Union(_registrations
                    .Where(x => x.ConcreteTypeTemplateDependency != null)
                    .Select(x => x.ConcreteTypeTemplateDependency))
                .ToList();
        }
    }

    internal class ContainerRegistration
    {
        public ContainerRegistration(string interfaceType, string concreteType, string lifetime, ITemplateDependancy interfaceTypeTemplateDependency, ITemplateDependancy concreteTypeTemplateDependency)
        {
            InterfaceType = interfaceType;
            ConcreteType = concreteType;
            Lifetime = lifetime ?? Constants.ContainerRegistration.TransientLifetime;
            InterfaceTypeTemplateDependency = interfaceTypeTemplateDependency;
            ConcreteTypeTemplateDependency = concreteTypeTemplateDependency;
        }

        public string InterfaceType { get; private set; }
        public string ConcreteType { get; private set; }
        public string Lifetime { get; private set; }
        public ITemplateDependancy InterfaceTypeTemplateDependency { get; private set; }
        public ITemplateDependancy ConcreteTypeTemplateDependency { get; }
    }
}
