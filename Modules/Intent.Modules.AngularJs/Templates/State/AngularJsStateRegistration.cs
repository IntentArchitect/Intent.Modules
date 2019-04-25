﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.DTO;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.Templates


namespace Intent.Modules.AngularJs.Templates.State
{
    [Description(AngularJsStateTemplate.Identifier)]
    public class AngularJsStateRegistration : ModelTemplateRegistrationBase<ViewStateModel>
    {
        private readonly IMetadataManager _metaDataManager;

        public AngularJsStateRegistration(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => AngularJsStateTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, ViewStateModel model)
        {
            return new AngularJsStateTemplate(project, model);
        }

        public override IEnumerable<ViewStateModel> GetModels(IApplication application)
        {
            return _metaDataManager.GetMetaData<ViewStateModel>(new MetaDataIdentifier("ViewState")).ToList();
        }
    }
}

