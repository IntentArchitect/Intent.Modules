using Intent.Modules.Bower.Templates.BowerConfig;
using Intent.Modules.Bower.Templates.BowerRCFile;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Bower
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            RegisterTemplate(BowerConfigTemplate.Identifier, project => new BowerConfigTemplate(project));
            RegisterTemplate(BowerRCFileTemplate.Identifier, project => new BowerRCFileTemplate(project));
        }
    }
}
