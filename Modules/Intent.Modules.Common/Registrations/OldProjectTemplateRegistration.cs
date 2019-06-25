using System;
using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Common.Registrations
{
    [Obsolete("Legacy system for registering templates")]
    public abstract class OldProjectTemplateRegistration
    {
        public Action<ITemplate> RegisterApplicationTemplate { get; set; }
        public Action<string, Type, object> RegisterDecoratorNonGeneric { get; set; }
        public Action<string, Func<IProject, ITemplate>> RegisterTemplate { get; set; }

        public void RegisterDecorator<TDecorator>(string id, TDecorator decorator)
        {
            RegisterDecoratorNonGeneric(id, typeof(TDecorator), decorator);
        }

        public abstract void RegisterStuff(IApplication application, IMetadataManager metadataManager);
    }
}
