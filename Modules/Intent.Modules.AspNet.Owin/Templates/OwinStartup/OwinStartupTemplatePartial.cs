using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.AspNet.Owin.Templates.OwinStartup
{
    public static class NugetPackage
    {
        public static NugetPackageInfo MicrosoftOwin = new NugetPackageInfo("Microsoft.Owin", "4.0.0", "net45")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("Microsoft.Owin", "4.0.0.0", "31bf3856ad364e35"));
        public static NugetPackageInfo MicrosoftOwinHostSystemWeb = new NugetPackageInfo("Microsoft.Owin.Host.SystemWeb", "4.0.0", "net45");

    }

    partial class OwinStartupTemplate : IntentRoslynProjectItemTemplateBase, ITemplate, IHasNugetDependencies, IHasDecorators<IOwinStartupDecorator>, IHasAssemblyDependencies
    {
        public const string Identifier = "Intent.Owin.OwinStartup";
        private IEnumerable<IOwinStartupDecorator> _decorators;

        public OwinStartupTemplate(IProject project)
            : base(Identifier, project)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackage.MicrosoftOwin,
                NugetPackage.MicrosoftOwinHostSystemWeb,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "Startup",
                fileExtension: "cs",
                defaultLocationInProject: "",
                className: "Startup",
                @namespace: "${Project.Name}"
                );
        }

        public string Configuration()
        {
            var configurations = GetDecorators().SelectMany(x => x.Configuration()).ToArray();
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
            var methods = GetDecorators().SelectMany(x => x.Methods()).ToArray();
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

        public IEnumerable<IOwinStartupDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return GetDecorators().Where(x => x is IHasAssemblyDependencies)
                .Cast<IHasAssemblyDependencies>()
                .SelectMany(x => x.GetAssemblyDependencies())
                .ToList();
        }
    }

}
