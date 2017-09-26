﻿using Intent.MetaModel.Domain;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.EF.Templates.EFMapping
{
    [Description("Intent EF Mapping Template")]
    public class Registrations : ModelTemplateRegistrationBase<Class>
    {
        private IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId
        {
            get
            {
                return EFMappingTemplate.Identifier;
            }
        }

        public override ITemplate CreateTemplateInstance(IProject project, Class model)
        {
            return new EFMappingTemplate(model, project);
        }

        public override IEnumerable<Class> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            return _metaDataManager.GetMetaData<Class>(new MetaDataType("DomainEntity")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}
