using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.Startup
{
    partial class CoreWebStartupTemplate : IntentRoslynProjectItemTemplateBase<object>
    {
        public const string Identifier = "Intent.VisualStudio.Projects.CoreWeb.Startup";
        private readonly IList<ContainerRegistration> _registrations = new List<ContainerRegistration>();

        public CoreWebStartupTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, null)
        {
            eventDispatcher.Subscribe(ApplicationEvents.Container_RegistrationRequired, Handle);
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

        public string Registrations()
        {
            var registrations = _registrations != null && _registrations.Any()
                ? _registrations.Select(DefineRegistration).Aggregate((x, y) => x + y)
                : string.Empty;

            return registrations;// + Environment.NewLine + GetDecorators().Aggregate(x => x.Registrations());
        }

        private static string DefineRegistration(ContainerRegistration x)
        {
            return x.InterfaceType != null 
                ? $"{Environment.NewLine}            services.AddTransient<{x.InterfaceType}, {x.ConcreteType}>();" 
                : $"{Environment.NewLine}            services.AddTransient<{x.ConcreteType}>();";
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"Startup",
                fileExtension: "cs",
                defaultLocationInProject: "",
                className: $"Startup",
                @namespace: "${Project.Name}"
                );
        }

        internal class ContainerRegistration
        {
            public ContainerRegistration(string interfaceType, string concreteType, string lifetime, ITemplateDependancy interfaceTypeTemplateDependency, ITemplateDependancy concreteTypeTemplateDependency)
            {
                InterfaceType = interfaceType;
                ConcreteType = concreteType;
                Lifetime = lifetime ?? "Transient";
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
}
