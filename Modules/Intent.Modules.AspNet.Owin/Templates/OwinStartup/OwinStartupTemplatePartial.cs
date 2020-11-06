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
using Intent.Templates;

namespace Intent.Modules.AspNet.Owin.Templates.OwinStartup
{
    public static class NugetPackage
    {
        public static NugetPackageInfo MicrosoftOwin = new NugetPackageInfo("Microsoft.Owin", "4.0.0")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("Microsoft.Owin", "4.0.0.0", "31bf3856ad364e35"));
        public static NugetPackageInfo MicrosoftOwinHostSystemWeb = new NugetPackageInfo("Microsoft.Owin.Host.SystemWeb", "4.0.0");

    }

    partial class OwinStartupTemplate : CSharpTemplateBase, IHasDecorators<IOwinStartupDecorator>, IHasAssemblyDependencies, Intent.Modules.Common.IDeclareUsings
    {
        public const string Identifier = "Intent.Owin.OwinStartup";
        private readonly IList<IOwinStartupDecorator> _decorators = new List<IOwinStartupDecorator>();
        private readonly IList<Initializations> _initializations = new List<Initializations>();

        public OwinStartupTemplate(IProject project)
            : base(Identifier, project)
        {
            AddNugetDependency(NugetPackage.MicrosoftOwin);
            AddNugetDependency(NugetPackage.MicrosoftOwinHostSystemWeb);

            Project.Application.EventDispatcher.Subscribe(InitializationRequiredEvent.EventId, Handle);
        }

        private void Handle(ApplicationEvent @event)
        {
            _initializations.Add(new Initializations(
                usings: @event.GetValue(InitializationRequiredEvent.UsingsKey),
                code: @event.GetValue(InitializationRequiredEvent.CallKey),
                method: @event.TryGetValue(InitializationRequiredEvent.MethodKey),
                priority: @event.TryGetValue(InitializationRequiredEvent.PriorityKey),
                templateDependency: @event.TryGetValue(InitializationRequiredEvent.TemplateDependencyIdKey)));
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"Startup", 
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return base.GetTemplateDependencies()
                .Concat(_initializations.Where(x => x.TemplateDependency != null).Select(x => TemplateDependency.OnTemplate(x.TemplateDependency)));
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return base.GetNugetDependencies()
                    .Concat(_initializations.Where(x => x.TemplateDependency != null).SelectMany(x => GetTemplate<IHasNugetDependencies>(TemplateDependency.OnTemplate(x.TemplateDependency))?.GetNugetDependencies()));
        }

        public string Configuration()
        {
            var configurations = GetDecorators()
                .SelectMany(x => x.Configuration(), (x, configuration) => new { Code = configuration, x.Priority })
                .Union(_initializations
                    .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                    .Select(x => new { x.Code, x.Priority }))
                .OrderByDescending(x => x.Priority)
                .Select(x => x.Code)
                .ToArray();

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

        public string Methods()
        {
            var methods = GetDecorators()
                .SelectMany(x => x.Methods(), (x, method) => new { Code = method, x.Priority })
                .Union(_initializations
                    .Where(x => !string.IsNullOrWhiteSpace(x.Method))
                    .Select(x => new { Code = x.Method, x.Priority }))
                .OrderByDescending(x => x.Priority)
                .Select(x => x.Code)
                .ToArray();

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

        public void AddDecorator(IOwinStartupDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<IOwinStartupDecorator> GetDecorators()
        {
            return _decorators;
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return GetDecorators().Where(x => x is IHasAssemblyDependencies)
                .Cast<IHasAssemblyDependencies>()
                .SelectMany(x => x.GetAssemblyDependencies())
                .ToList();
        }

        public IEnumerable<string> DeclareUsings()
        {
            return _initializations.Select(x => x.Usings)
                .Select(x => x.Split(';'))
                .SelectMany(x => x)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());
        }
    }

    internal class Initializations
    {
        public string Usings { get; }
        public string Code { get; }
        public string Method { get; }
        public int Priority { get; }
        public string TemplateDependency { get; }


        public Initializations(string usings, string code, string method, string priority, string templateDependency)
        {
            Usings = usings;
            Code = code;
            Method = method;
            TemplateDependency = templateDependency;
            Priority = int.TryParse(priority, out var parsedPriority)
                ? parsedPriority
                : 0;
        }
    }
}
