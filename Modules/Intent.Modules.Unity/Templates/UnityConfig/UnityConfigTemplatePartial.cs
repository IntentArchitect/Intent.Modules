using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Unity.Templates.UnityConfig
{
    partial class UnityConfigTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasDecorators<IUnityRegistrationsDecorator>
    {
        public const string Identifier = "Intent.WebApi.UnityConfig";

        private IEnumerable<IUnityRegistrationsDecorator> _decorators;
        private readonly IList<ContainerRegistration> _registrations = new List<ContainerRegistration>();

        public UnityConfigTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, null)
        {
            eventDispatcher.Subscribe(ApplicationEvents.Container_RegistrationRequired, Handle);
        }

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
                defaultLocationInProject: "",
                className: "UnityConfig",
                @namespace: "${Project.ProjectName}"
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
            var registrations = _registrations != null && _registrations.Any()
                ? _registrations.Select(x => $"{Environment.NewLine}            container.RegisterType<{x.InterfaceType}, {x.ConcreteType}>();").Aggregate((x, y) => x + y)
                : string.Empty;

            return registrations + Environment.NewLine + GetDecorators().Aggregate(x => x.Registrations());
        }

        public IEnumerable<IUnityRegistrationsDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        private void Handle(ApplicationEvent @event)
        {
            _registrations.Add(new ContainerRegistration(@event.GetValue("InterfaceType"), @event.GetValue("ConcreteType"), @event.TryGetValue("Lifetime")));
        }
    }

    internal class ContainerRegistration
    {
        public ContainerRegistration(string interfaceType, string concreteType, string lifetime)
        {
            InterfaceType = interfaceType;
            ConcreteType = concreteType;
            Lifetime = lifetime ?? "Transient";
        }

        public string InterfaceType { get; private set; }
        public string ConcreteType { get; private set; }
        public string Lifetime { get; private set; }
    }
}
