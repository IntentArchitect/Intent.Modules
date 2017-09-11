using Intent.Packages.Typescript.Config.Templates.TypescriptConfig;
using Intent.Packages.Typescript.Config.Templates.TypescriptDefinitelyTypedReference;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Typescript.Config
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterTemplate(TypescriptConfigFileTemplate.Identifier, project => new TypescriptConfigFileTemplate(project));
            RegisterTemplate(TypescriptDefinitelyTypedReferencesTemplate.Identifier, project => new TypescriptDefinitelyTypedReferencesTemplate(project));
        }
    }
}
