using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templates.NoT4Template.Templates.Xml
{
    [Description("Templates.NoT4Template.Xml Template")]
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
            var settings = new Dictionary<string, string>();
            settings.Add("Setting1", "One");
            settings.Add("Setting2", "Two");
            settings.Add("Setting3", "Three");
            return new Template(project, settings);
        }
    }
}
