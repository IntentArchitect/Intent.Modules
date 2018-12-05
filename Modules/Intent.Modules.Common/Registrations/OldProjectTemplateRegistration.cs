using System;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Common.Registrations
{
    [Obsolete("Legacy system for registering templates")]
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
