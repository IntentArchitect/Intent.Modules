using System.Collections.Generic;
using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Mapping.EntityToDto.Templates.DTOMappingProfile;
using Intent.Templates

namespace Intent.Modules.Mapping.EntityToDto.Owin.Decorators
{
    public class AutomapperOwinStartupDecorator : IOwinStartupDecorator, IHasTemplateDependencies, IHasNugetDependencies
    {
        public const string IDENTIFIER = "Intent.Mapping.EntityToDto.Owin.ConfigurationDecorator";

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "Intent.Framework.AutoMapper",
                "Intent.Framework.Core.Mapping",
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

        public IEnumerable<ITemplateDependency> GetTemplateDependencies() => new[]
        {
            TemplateDependancy.OnTemplate(DTOMappingTemplate.Identifier),
        };

        public IEnumerable<INugetPackageInfo> GetNugetDependencies() => new[]
        {
            NugetPackages.IntentFrameworkAutoMapper,
        };
    }
}
