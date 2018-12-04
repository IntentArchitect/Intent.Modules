using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.Engine;

namespace Intent.SoftwareFactory.Registrations
{
    public abstract class OldProjectTemplateRegistration : IOldProjectTemplateRegistration
    {
        public Action<ITemplate> RegisterApplicationTeamplate { get; set; }
        public Action<string, Type, object> RegisterDecoratorNonGeneric { get; set; }
        public Action<string, Func<IProject, ITemplate>> RegisterTemplate { get; set; }

        public void RegisterDecorator<TDecorator>(string id, TDecorator decorator)
        {
            RegisterDecoratorNonGeneric(id, typeof(TDecorator), decorator);
        }

        public abstract void RegisterStuff(IApplication application, IMetaDataManager metaDataManager);
    }
}
