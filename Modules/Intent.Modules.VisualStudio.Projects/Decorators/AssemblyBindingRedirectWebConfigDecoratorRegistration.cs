﻿using Intent.SoftwareFactory.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Registrations;
using Intent.Engine;

namespace Intent.Modules.VisualStudio.Projects.Decorators
{
    [Description("Assembly Binding Redirect Web Config - Web config decorator")]
    public class AssemblyBindingRedirectWebConfigDecoratorRegistration : DecoratorRegistration<IWebConfigDecorator>
    {

        public override string DecoratorId
        {
            get
            {
                return AssemblyBindingRedirectWebConfigDecorator.IDENTIFIER;
            }
        }

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new AssemblyBindingRedirectWebConfigDecorator();
        }
    }
}
