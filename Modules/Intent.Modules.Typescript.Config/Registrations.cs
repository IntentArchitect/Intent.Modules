using Intent.Modules.Common.Registrations;
using Intent.Modules.Typescript.Config.Templates.TypescriptConfig;
using Intent.Modules.Typescript.Config.Templates.TypescriptDefinitelyTypedReference;
using Intent.Engine;
using Intent.Registrations;

namespace Intent.Modules.Typescript.Config
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetadataManager metadataManager)
        {
            RegisterTemplate(TypescriptConfigFileTemplate.Identifier, project => new TypescriptConfigFileTemplate(project));
            RegisterTemplate(TypescriptDefinitelyTypedReferencesTemplate.Identifier, project => new TypescriptDefinitelyTypedReferencesTemplate(project));
        }
    }
}
