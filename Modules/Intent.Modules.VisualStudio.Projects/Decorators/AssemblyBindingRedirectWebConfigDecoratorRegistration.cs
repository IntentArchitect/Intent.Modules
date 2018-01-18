using Intent.SoftwareFactory.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.VisualStudio.Projects.Decorators
{
    [Description("Assembly Binding Redirect Web Config - Web config decorator")]
    public class AssemblyBindingRedirectWebConfigDecoratorRegistration : DecoratorRegistration<IWebConfigDecorator>
    {

        public override string DecoratorId
        {
            get
            {
                return AssemblyBindingRedirectWebConfigDecorator.Identifier;
            }
        }

        public override object CreateDecoratorInstance()
        {
            return new AssemblyBindingRedirectWebConfigDecorator();
        }
    }
}
