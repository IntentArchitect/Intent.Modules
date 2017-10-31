
using Intent.SoftwareFactory.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Templates.Registrations
{
    public abstract class NoModelTemplateRegistrationBase : IProjectTemplateRegistration
    {
        public abstract string TemplateId { get; }

        public abstract ITemplate CreateTemplateInstance(IProject project);

        public void DoRegistration(Action<string, Func<IProject, ITemplate>> register, IApplication applicationManager)
        {
            register(TemplateId, project => CreateTemplateInstance(project));
        }
    }
}
