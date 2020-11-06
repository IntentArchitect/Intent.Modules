using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Autofac.Templates.AutofacConfig;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.Autofac.Templates.AutofacConfig
{
    partial class AutofacConfigTemplate : CSharpTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasDecorators<IAutofacRegistrationsDecorator>, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.Autofac.Config";

        private readonly IList<IAutofacRegistrationsDecorator> _decorators = new List<IAutofacRegistrationsDecorator>();
        private readonly IList<ContainerRegistration> _registrations = new List<ContainerRegistration>();

        public AutofacConfigTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, null)
        {
            eventDispatcher.Subscribe(Constants.ContainerRegistrationEvent.EventId, Handle);
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"AutofacConfig",
                @namespace: $"{OutputTarget.GetNamespace()}");
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

        // Overriden to empty so that namespaces are not normalized completely (to avoid class name ambiguities - e.g. two SCHs with the same name)
        public override string DependencyUsings => "";

        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return base.GetTemplateDependencies().Concat(_registrations
                .Where(x => x.InterfaceType != null && x.InterfaceTypeTemplateDependency != null)
                .Select(x => x.InterfaceTypeTemplateDependency)
                .Union(_registrations
                    .Where(x => x.ConcreteTypeTemplateDependency != null)
                    .Select(x => x.ConcreteTypeTemplateDependency))
                .ToList());
        }

        public void AddDecorator(IAutofacRegistrationsDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<IAutofacRegistrationsDecorator> GetDecorators()
        {
            return _decorators;
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
