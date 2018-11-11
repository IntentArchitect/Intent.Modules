using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Autofac.Templates.AutofacConfig;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Autofac.Templates.AutofacConfig
{
    partial class AutofacConfigTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasDecorators<IAutofacRegistrationsDecorator>, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.Autofac.Config";

        private IEnumerable<IAutofacRegistrationsDecorator> _decorators;
        private readonly IList<ContainerRegistration> _registrations = new List<ContainerRegistration>();

        public AutofacConfigTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, null)
        {
            eventDispatcher.Subscribe(Constants.ContainerRegistrationEvent.EventId, Handle);
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
                fileName: "AutofacConfig",
                fileExtension: "cs",
                defaultLocationInProject: "Autofac",
                className: "AutofacConfig",
                @namespace: "${Project.ProjectName}.Autofac"
                );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new INugetPackageInfo[]
            {
                NugetPackages.Autofac,
                NugetPackages.AutofacExtensionsDependencyInjection,
                NugetPackages.MicrosoftExtensionsDependencyInjectionAbstractions,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public string Registrations()
        {
            var output = _registrations.Any() ? _registrations.Select(GetRegistrationString).Aggregate((x, y) => x + y) : string.Empty;

            return output + Environment.NewLine + GetDecorators().Aggregate(x => x.Registrations());
        }

        private string GetRegistrationString(ContainerRegistration x)
        {
            return x.InterfaceType != null 
                ? $"{Environment.NewLine}            builder.RegisterType<{NormalizeNamespace(x.ConcreteType)}>().As<{NormalizeNamespace(x.InterfaceType)}>(){GetLifetimeManager(x)};" 
                : $"{Environment.NewLine}            builder.RegisterType<{NormalizeNamespace(x.ConcreteType)}>(){GetLifetimeManager(x)};";
        }

        private string GetLifetimeManager(ContainerRegistration registration)
        {
            switch (registration.Lifetime)
            {
                case Constants.ContainerRegistrationEvent.SingletonLifetime:
                    return ".SingleInstance()";
                case Constants.ContainerRegistrationEvent.PerServiceCallLifetime:
                    return ".InstancePerRequest()";
                case Constants.ContainerRegistrationEvent.TransientLifetime:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new ITemplateDependancy[0];
            //return _registrations
            //    .Where(x => x.InterfaceType != null && x.InterfaceTypeTemplateDependency != null)
            //    .Select(x => x.InterfaceTypeTemplateDependency)
            //    .Union(_registrations
            //        .Where(x => x.ConcreteTypeTemplateDependency != null)
            //        .Select(x => x.ConcreteTypeTemplateDependency))
            //    .ToList();
        }

        public IEnumerable<IAutofacRegistrationsDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        private void Handle(ApplicationEvent @event)
        {
            _registrations.Add(new ContainerRegistration(
                interfaceType: @event.TryGetValue(ContainerRegistrationEvent.InterfaceTypeKey), 
                concreteType: @event.GetValue(ContainerRegistrationEvent.ConcreteTypeKey), 
                lifetime: @event.TryGetValue(ContainerRegistrationEvent.LifetimeKey),
                interfaceTypeTemplateDependency: @event.TryGetValue(ContainerRegistrationEvent.InterfaceTypeTemplateIdKey) != null ? TemplateDependancy.OnTemplate(@event.TryGetValue(ContainerRegistrationEvent.InterfaceTypeTemplateIdKey)) : null,
                concreteTypeTemplateDependency: @event.TryGetValue(ContainerRegistrationEvent.ConcreteTypeTemplateIdKey) != null ? TemplateDependancy.OnTemplate(@event.TryGetValue(ContainerRegistrationEvent.ConcreteTypeTemplateIdKey)) : null));
        }
    }

    internal class ContainerRegistration
    {
        public ContainerRegistration(string interfaceType, string concreteType, string lifetime, ITemplateDependancy interfaceTypeTemplateDependency, ITemplateDependancy concreteTypeTemplateDependency)
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
        public ITemplateDependancy InterfaceTypeTemplateDependency { get; private set; }
        public ITemplateDependancy ConcreteTypeTemplateDependency { get; }
    }
}
