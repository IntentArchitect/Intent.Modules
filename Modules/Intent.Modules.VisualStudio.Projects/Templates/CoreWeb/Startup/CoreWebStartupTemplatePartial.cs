using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.Startup
{
    partial class CoreWebStartupTemplate : IntentRoslynProjectItemTemplateBase<object>, IHasTemplateDependencies, IDeclareUsings
    {
        public const string Identifier = "Intent.VisualStudio.Projects.CoreWeb.Startup";
        private readonly IList<ContainerRegistration> _registrations = new List<ContainerRegistration>();
        private readonly IList<DbContextContainerRegistration> _dbContextRegistrations = new List<DbContextContainerRegistration>();
        private readonly IList<Initializations> _initializations = new List<Initializations>();

        public CoreWebStartupTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, null)
        {
            eventDispatcher.Subscribe(ContainerRegistrationEvent.EventId, HandleServiceRegistraiton);
            eventDispatcher.Subscribe(ContainerRegistrationForDbContextEvent.EventId, HandleDbContextRegistration);
            eventDispatcher.Subscribe(InitializationRequiredEvent.EventId, HandleInitialization);

        }

        private void HandleServiceRegistraiton(ApplicationEvent @event)
        {
            _registrations.Add(new ContainerRegistration(
                interfaceType: @event.TryGetValue("InterfaceType"),
                concreteType: @event.GetValue("ConcreteType"),
                lifetime: @event.TryGetValue("Lifetime"),
                interfaceTypeTemplateDependency: @event.TryGetValue("InterfaceTypeTemplateId") != null ? TemplateDependancy.OnTemplate(@event.TryGetValue("InterfaceTypeTemplateId")) : null,
                concreteTypeTemplateDependency: @event.TryGetValue("ConcreteTypeTemplateId") != null ? TemplateDependancy.OnTemplate(@event.TryGetValue("ConcreteTypeTemplateId")) : null));
        }

        private void HandleDbContextRegistration(ApplicationEvent @event)
        {
            _dbContextRegistrations.Add(new DbContextContainerRegistration(
                @event.TryGetValue(ContainerRegistrationForDbContextEvent.UsingsKey),
                @event.GetValue(ContainerRegistrationForDbContextEvent.ConcreteTypeKey),
                @event.TryGetValue(ContainerRegistrationForDbContextEvent.ConcreteTypeTemplateIdKey) != null ? TemplateDependancy.OnTemplate(@event.TryGetValue(ContainerRegistrationForDbContextEvent.ConcreteTypeTemplateIdKey)) : null,
                @event.TryGetValue(ContainerRegistrationForDbContextEvent.OptionsKey)));
        }

        private void HandleInitialization(ApplicationEvent @event)
        {
            _initializations.Add(new Initializations(
                usings: @event.GetValue(InitializationRequiredEvent.UsingsKey),
                code: @event.GetValue(InitializationRequiredEvent.CallKey),
                method: @event.TryGetValue(InitializationRequiredEvent.MethodKey)));
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
            var methods = _initializations
                .Where(x => !string.IsNullOrWhiteSpace(x.Method))
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

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return _registrations
                .Where(x => x.InterfaceType != null && x.InterfaceTypeTemplateDependency != null)
                .Select(x => x.InterfaceTypeTemplateDependency)
                .Union(_registrations
                    .Where(x => x.ConcreteTypeTemplateDependency != null)
                    .Select(x => x.ConcreteTypeTemplateDependency))
                .Union(_dbContextRegistrations
                    .Where(x => x.ConcreteTypeTemplateDependency != null)
                    .Select(x => x.ConcreteTypeTemplateDependency))
                .ToList();
        }

        public IEnumerable<string> DeclareUsings()
        {
            return _dbContextRegistrations.Select(x => x.Usings)
                .Select(x => x.Split(';'))
                .SelectMany(x => x)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());
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

            public string InterfaceType { get; }
            public string ConcreteType { get; }
            public string Lifetime { get; }
            public ITemplateDependancy InterfaceTypeTemplateDependency { get; }
            public ITemplateDependancy ConcreteTypeTemplateDependency { get; }
        }

        internal class DbContextContainerRegistration
        {
            public DbContextContainerRegistration(string usings, string concreteType, ITemplateDependancy concreteTypeTemplateDependency, string options)
            {
                Usings = usings;
                ConcreteType = concreteType;
                ConcreteTypeTemplateDependency = concreteTypeTemplateDependency;
                Options = options;
            }

            public string Usings { get; }
            public string ConcreteType { get; }
            public ITemplateDependancy ConcreteTypeTemplateDependency { get; }
            public string Options { get; }
        }

        internal class Initializations
        {
            public string Usings { get; }
            public string Code { get; }
            public string Method { get; }

            public Initializations(string usings, string code, string method)
            {
                Usings = usings;
                Code = code;
                Method = method;
            }
        }
    }
}
