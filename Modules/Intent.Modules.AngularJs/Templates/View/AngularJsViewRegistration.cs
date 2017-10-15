using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.AngularJs.Templates.State;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.AngularJs.Templates.View
{
    [Description("Intent - AngularJs View Registration")]
    public class AngularJsViewRegistration : ModelTemplateRegistrationBase<ViewStateModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public AngularJsViewRegistration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => AngularJsViewTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, ViewStateModel model)
        {
            return new AngularJsViewTemplate(project, model);
        }

        public override IEnumerable<ViewStateModel> GetModels(IApplication application)
        {
            return _metaDataManager.GetMetaData<ViewStateModel>(new MetaDataType("ViewState")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}

