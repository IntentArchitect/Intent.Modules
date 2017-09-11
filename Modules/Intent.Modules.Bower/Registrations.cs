using Intent.Packages.Bower.Templates.BowerConfig;
using Intent.Packages.Bower.Templates.BowerRCFile;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Bower
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterTemplate(BowerConfigTemplate.Identifier, project => new BowerConfigTemplate(project));
            RegisterTemplate(BowerRCFileTemplate.Identifier, project => new BowerRCFileTemplate(project));
        }
    }
}
