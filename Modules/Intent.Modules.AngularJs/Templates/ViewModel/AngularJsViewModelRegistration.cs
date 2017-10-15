using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.AngularJs.Templates.State;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.AngularJs.Templates.ViewModel
{
    [Description("Intent - AngularJs ViewModel Registration")]
    public class AngularJsViewModelRegistration : ModelTemplateRegistrationBase<ViewStateModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public AngularJsViewModelRegistration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => AngularJsViewModelTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, ViewStateModel model)
        {
            return new AngularJsViewModelTemplate(project, model);
        }

        public override IEnumerable<ViewStateModel> GetModels(IApplication application)
        {
            return _metaDataManager.GetMetaData<ViewStateModel>(new MetaDataType("ViewState")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}

