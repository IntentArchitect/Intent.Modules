using Intent.SoftwareFactory.Engine;
using Intent.Templates

using System.ComponentModel;
using Intent.Modules.Common.Registrations;

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
