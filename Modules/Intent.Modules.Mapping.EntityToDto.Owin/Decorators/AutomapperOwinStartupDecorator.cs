using System.Collections.Generic;
using Intent.Packages.Mapping.EntityToDto.Templates.DTOMappingProfile;
using Intent.Packages.Owin.Templates.OwinStartup;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Mapping.EntityToDto.Owin.Decorators
{
    public class AutomapperOwinStartupDecorator : IOwinStartupDecorator, IHasTemplateDependencies, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Mapping.EntityToDto.Owin.ConfigurationDecorator";

        public IEnumerable<string> DeclareUsings()
        {


            return new[]
            {
                "using Intent.Framework.AutoMapper;",
                "using Intent.Framework.Core.Mapping;",
            };
        }

        public IEnumerable<string> Configuration()
        {
            return new[]
            {
                "TypeMapperFactory.SetCurrent(AutomapperTypeMapperFactory.LoadFromAssemblyBasedOnType(typeof(CommonProfile), typeof(DtoMappingProfile)));",
            };
        }

        public IEnumerable<string> Methods() => new string[0];

        public int Priority { get; set; } = 200;

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies() => new[]
        {
            TemplateDependancy.OnTemplate(DTOMappingTemplate.Identifier),
        };

        public IEnumerable<INugetPackageInfo> GetNugetDependencies() => new[]
        {
            NugetPackages.IntentFrameworkAutoMapper,
        };
    }
}
