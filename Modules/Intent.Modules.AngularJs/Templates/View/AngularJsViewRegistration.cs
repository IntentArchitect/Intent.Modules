﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.AngularJs.Templates.State;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates


namespace Intent.Modules.AngularJs.Templates.View
{
    [Description("Intent - AngularJs View Registration")]
    public class AngularJsViewRegistration : ModelTemplateRegistrationBase<ViewStateModel>
    {
        private readonly IMetadataManager _metaDataManager;

        public AngularJsViewRegistration(IMetadataManager metaDataManager)
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
            return _metaDataManager.GetMetaData<ViewStateModel>(new MetaDataIdentifier("ViewState")).ToList();
        }
    }
}

