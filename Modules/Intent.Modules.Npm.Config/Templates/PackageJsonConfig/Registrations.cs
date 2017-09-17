using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.ComponentModel;

namespace Intent.Modules.Npm.Config.Templates.PackageJsonConfig
{
    [Description("Intent npm package.json")]
    public class Registrations : NoModelTemplateRegistrationBase
    {
        public Registrations()
        {
        }

        public override string TemplateId => PackageJsonTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new PackageJsonTemplate(project, project.Application.EventDispatcher);
        }
    }
}
