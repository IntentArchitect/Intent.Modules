using Intent.Modules.Common.Registrations;
using Intent.Modules.Typescript.Config.Templates.TypescriptConfig;
using Intent.Modules.Typescript.Config.Templates.TypescriptDefinitelyTypedReference;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Typescript.Config
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            RegisterTemplate(TypescriptConfigFileTemplate.Identifier, project => new TypescriptConfigFileTemplate(project));
            RegisterTemplate(TypescriptDefinitelyTypedReferencesTemplate.Identifier, project => new TypescriptDefinitelyTypedReferencesTemplate(project));
        }
    }
}
