using Intent.MetaModel.Domain;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templates.NoT4Template.Templates.StringBuilder
{
    [Description("Templates.NoT4Template.StringBuilder Template")]
    public class Registration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId
        {
            get
            {
                return Template.Identifier;
            }
        }

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new Template(project);
        }
    }
}
