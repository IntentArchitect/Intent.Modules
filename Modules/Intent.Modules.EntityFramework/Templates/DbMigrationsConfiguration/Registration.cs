using Intent.SoftwareFactory.Templates.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.EntityFramework.Templates.DbMigrationsConfiguration;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.EF.Templates.DbMigrationsConfiguration
{
    [Description("Intent EF  Migration Configuration")]
    public class Registrations : NoModelTemplateRegistrationBase
    {

        public override string TemplateId
        {
            get
            {
                return DbMigrationsConfigurationTemplate.Identifier;
            }
        }

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new DbMigrationsConfigurationTemplate(project);
        }
    }
}
