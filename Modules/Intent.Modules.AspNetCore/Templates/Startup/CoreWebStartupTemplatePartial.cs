using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.VisualStudio.Projects;
using Intent.Templates;

namespace Intent.Modules.AspNetCore.Templates.Startup
{
    partial class CoreWebStartupTemplate : CSharpTemplateBase<object>, IHasTemplateDependencies, IDeclareUsings, IHasDecorators<CoreWebStartupDecorator>, IHasNugetDependencies
    {
        public const string Identifier = "Intent.AspNetCore.Startup";
        private readonly IList<CoreWebStartupDecorator> _decorators = new List<CoreWebStartupDecorator>();
        private readonly IList<ContainerRegistration> _registrations = new List<ContainerRegistration>();
        private readonly IList<DbContextContainerRegistration> _dbContextRegistrations = new List<DbContextContainerRegistration>();
        private readonly IList<Initializations> _serviceConfigurations = new List<Initializations>();
        private readonly IList<Initializations> _initializations = new List<Initializations>();

        public CoreWebStartupTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, null)
        {
            eventDispatcher.Subscribe(ContainerRegistrationEvent.EventId, HandleServiceRegistration);
            eventDispatcher.Subscribe(ContainerRegistrationForDbContextEvent.EventId, HandleDbContextRegistration);
            eventDispatcher.Subscribe(ServiceConfigurationRequiredEvent.EventId, HandleServiceConfiguration);
            eventDispatcher.Subscribe(InitializationRequiredEvent.EventId, HandleInitialization);
        }

        private void HandleServiceRegistration(ApplicationEvent @event)
        {
            _registrations.Add(new ContainerRegistration(
                interfaceType: @event.TryGetValue("InterfaceType"),
                concreteType: @event.GetValue("ConcreteType"),
                lifetime: @event.TryGetValue("Lifetime"),
                interfaceTypeTemplateDependency: @event.TryGetValue("InterfaceTypeTemplateId") != null ? TemplateDependency.OnTemplate(@event.TryGetValue("InterfaceTypeTemplateId")) : null,
                concreteTypeTemplateDependency: @event.TryGetValue("ConcreteTypeTemplateId") != null ? TemplateDependency.OnTemplate(@event.TryGetValue("ConcreteTypeTemplateId")) : null));
        }

        private void HandleDbContextRegistration(ApplicationEvent @event)
        {
            var registration = new DbContextContainerRegistration(
                @event.TryGetValue(ContainerRegistrationForDbContextEvent.UsingsKey),
                @event.GetValue(ContainerRegistrationForDbContextEvent.ConcreteTypeKey),
                @event.TryGetValue(ContainerRegistrationForDbContextEvent.ConcreteTypeTemplateIdKey) != null ? TemplateDependency.OnTemplate(@event.TryGetValue(ContainerRegistrationForDbContextEvent.ConcreteTypeTemplateIdKey)) : null,
                @event.TryGetValue(ContainerRegistrationForDbContextEvent.OptionsKey),
                @event.TryGetValue(ContainerRegistrationForDbContextEvent.NugetDependency),
                @event.TryGetValue(ContainerRegistrationForDbContextEvent.NugetDependencyVersion));
            _dbContextRegistrations.Add(registration);
            if (registration.NugetPackage != null)
            {
                AddNugetDependency(registration.NugetPackage);
            }
        }

        private void HandleServiceConfiguration(ApplicationEvent @event)
        {
            _serviceConfigurations.Add(new Initializations(
                usings: @event.GetValue(ServiceConfigurationRequiredEvent.UsingsKey),
                code: @event.GetValue(ServiceConfigurationRequiredEvent.CallKey),
                method: @event.TryGetValue(ServiceConfigurationRequiredEvent.MethodKey),
                priority: int.TryParse(@event.TryGetValue(ServiceConfigurationRequiredEvent.PriorityKey), out var priority) ? priority : 0,
                templateDependency: null));
        }

        private void HandleInitialization(ApplicationEvent @event)
        {
            _initializations.Add(new Initializations(
                usings: @event.GetValue(InitializationRequiredEvent.UsingsKey),
                code: @event.GetValue(InitializationRequiredEvent.CallKey),
                method: @event.TryGetValue(InitializationRequiredEvent.MethodKey),
                priority: int.TryParse(@event.TryGetValue(InitializationRequiredEvent.PriorityKey), out var priority) ? priority : 0,
                templateDependency: @event.TryGetValue(InitializationRequiredEvent.TemplateDependencyIdKey) != null ? TemplateDependency.OnTemplate(@event.TryGetValue(InitializationRequiredEvent.TemplateDependencyIdKey)) : null));
        }

        public string ServiceConfigurations()
        {
            var configurations = _serviceConfigurations.Select(x => x.Code).ToList();

            if (!configurations.Any())
            {
                return string.Empty;
            }

            const string tabbing = "            ";
            return Environment.NewLine +
                   configurations
                       .Select(x => x.Trim())
                       .Select(x => x.StartsWith("#") ? x : $"{tabbing}{x}")
                       .Aggregate((x, y) => $"{x}{Environment.NewLine}" +
                                            $"{y}");
        }

        public void AddDecorator(CoreWebStartupDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<CoreWebStartupDecorator> GetDecorators()
        {
            return _decorators;
        }

        public string Configurations()
        {
            var configurations = _initializations.Select(x => x.Code).ToList();

            if (!configurations.Any())
            {
                return string.Empty;
            }

            const string tabbing = "            ";
            return Environment.NewLine +
                   configurations
                       .Select(x => x.Trim())
                       .Select(x => x.StartsWith("#") ? x : $"{tabbing}{x}")
                       .Aggregate((x, y) => $"{x}{Environment.NewLine}" +
                                            $"{y}");
        }

        public string Registrations()
        {
            string registrations = string.Empty;
            if (_dbContextRegistrations.Any())
            {
                registrations += $"{Environment.NewLine}            ConfigureDbContext(services);";
            }

            registrations += _registrations.Any()
                ? _registrations.Select(DefineServiceRegistration).Aggregate((x, y) => x + y)
                : string.Empty;

            return registrations;// + Environment.NewLine + GetDecorators().Aggregate(x => x.Registrations());
        }

        public string Methods()
        {
            var methods = _initializations.Concat(_serviceConfigurations)
                .Where(x => !string.IsNullOrWhiteSpace(x.Method))
                .OrderBy(x => x.Priority)
                .Select(x => x.Method)
                .ToList();

            if (_dbContextRegistrations.Any())
            {
                var dbContextRegistration = string.Empty;
                dbContextRegistration += $"{Environment.NewLine}        private void ConfigureDbContext(IServiceCollection services)";
                dbContextRegistration += $"{Environment.NewLine}        {{";
                dbContextRegistration += _dbContextRegistrations.Select(DefineDbContextRegistration).Aggregate((x, y) => x + y);
                dbContextRegistration += $"{Environment.NewLine}        }}";
                methods.Add(dbContextRegistration);
            }

            if (!methods.Any())
            {
                return string.Empty;
            }

            const string tabbing = "        ";
            return Environment.NewLine +
                   Environment.NewLine +
                   methods
                       .Select(x => x.Trim())
                       .Select(x => $"{tabbing}{x}")
                       .Aggregate((x, y) => $"{x}{Environment.NewLine}" +
                                            $"{Environment.NewLine}" +
                                            $"{y}");
        }

        private string DefineServiceRegistration(ContainerRegistration x)
        {
            return x.InterfaceType != null
                ? $"{Environment.NewLine}            services.{RegistrationType(x)}<{NormalizeNamespace(x.InterfaceType)}, {NormalizeNamespace(x.ConcreteType)}>();"
                : $"{Environment.NewLine}            services.{RegistrationType(x)}<{NormalizeNamespace(x.ConcreteType)}>();";
        }

        private string DefineDbContextRegistration(DbContextContainerRegistration x)
        {
            return $"{Environment.NewLine}            services.AddDbContext<{NormalizeNamespace(x.ConcreteType)}>({(x.Options != null ? $"x => x{x.Options}" : string.Empty)});";
        }

        private string RegistrationType(ContainerRegistration registration)
        {
            switch (registration.Lifetime)
            {
                case Constants.ContainerRegistrationEvent.SingletonLifetime:
                    return "AddSingleton";
                case Constants.ContainerRegistrationEvent.PerServiceCallLifetime:
                    return "AddScoped";
                case Constants.ContainerRegistrationEvent.TransientLifetime:
                    return "AddTransient";
                default:
                    return "AddTransient";
            }
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"Startup",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return base.GetTemplateDependencies().Concat(_registrations
                .Where(x => x.InterfaceType != null && x.InterfaceTypeTemplateDependency != null)
                .Select(x => x.InterfaceTypeTemplateDependency)
                .Union(_registrations
                    .Where(x => x.ConcreteTypeTemplateDependency != null)
                    .Select(x => x.ConcreteTypeTemplateDependency))
                .Union(_initializations
                    .Where(x => x.TemplateDependancy != null)
                    .Select(x => x.TemplateDependancy))
                .Union(_dbContextRegistrations
                    .Where(x => x.ConcreteTypeTemplateDependency != null)
                    .Select(x => x.ConcreteTypeTemplateDependency))
                .ToList());
        }

        public IEnumerable<string> DeclareUsings()
        {
            return _dbContextRegistrations.Select(x => x.Usings)
                .Concat(_initializations.Select(x => x.Usings))
                .Concat(_serviceConfigurations.Select(x => x.Usings))
                .Select(x => x.Split(';'))
                .SelectMany(x => x)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());
        }

        private bool IsNetCore2App() // Dirty way to get this info. Should not have this dependency
        {
            return Project.TargetFramework().StartsWith("netcoreapp2");
        }

        internal class ContainerRegistration
        {
            public ContainerRegistration(string interfaceType, string concreteType, string lifetime, ITemplateDependency interfaceTypeTemplateDependency, ITemplateDependency concreteTypeTemplateDependency)
            {
                InterfaceType = interfaceType;
                ConcreteType = concreteType;
                Lifetime = lifetime ?? "Transient";
                InterfaceTypeTemplateDependency = interfaceTypeTemplateDependency;
                ConcreteTypeTemplateDependency = concreteTypeTemplateDependency;
            }

            public string InterfaceType { get; }
            public string ConcreteType { get; }
            public string Lifetime { get; }
            public ITemplateDependency InterfaceTypeTemplateDependency { get; }
            public ITemplateDependency ConcreteTypeTemplateDependency { get; }
        }

        internal class DbContextContainerRegistration
        {
            public DbContextContainerRegistration(string usings, string concreteType,
                ITemplateDependency concreteTypeTemplateDependency, string options, string nugetDependency,
                string nugetDependencyVersion)
            {
                Usings = usings;
                ConcreteType = concreteType;
                ConcreteTypeTemplateDependency = concreteTypeTemplateDependency;
                Options = options;
                if (!string.IsNullOrWhiteSpace(nugetDependency) && !string.IsNullOrWhiteSpace(nugetDependencyVersion))
                {
                    NugetPackage = new NugetPackageInfo(nugetDependency, nugetDependencyVersion);
                }
            }

            public string Usings { get; }
            public string ConcreteType { get; }
            public ITemplateDependency ConcreteTypeTemplateDependency { get; }
            public string Options { get; }
            public NugetPackageInfo NugetPackage { get; }
        }

        internal class Initializations
        {
            public string Usings { get; }
            public string Code { get; }
            public string Method { get; }
            public int Priority { get; }
            public ITemplateDependency TemplateDependancy { get; }

            public Initializations(string usings, string code, string method, int priority, ITemplateDependency templateDependency)
            {
                Usings = usings;
                Code = code;
                Method = method;
                Priority = priority;
                TemplateDependancy = templateDependency;
            }
        }
    }
}
