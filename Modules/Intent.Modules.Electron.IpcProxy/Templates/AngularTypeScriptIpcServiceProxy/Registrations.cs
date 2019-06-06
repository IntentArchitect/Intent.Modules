using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.Service;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Electron.IpcProxy.Templates.AngularTypeScriptIpcServiceProxy
{
    [Description(AngularTypeScriptIpcServiceProxyTemplate.Identifier)]
    public class Registrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => AngularTypeScriptIpcServiceProxyTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new AngularTypeScriptIpcServiceProxyTemplate(model, project);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication applicationManager)
        {
            return _metaDataManager
                .GetServiceModels(applicationManager)
                .Where(x => x.Stereotypes.Any(y => y.Name == "IpcService"))
                .ToList();
        }
    }
}
