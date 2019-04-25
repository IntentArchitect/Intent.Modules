﻿using Intent.MetaModel.Domain;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Entities.Templates.DomainEntityInterface
{
    [Description(DomainEntityInterfaceTemplate.Identifier)]
    public class DomainEntityInterfaceTemplateRegistrations : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetadataManager _metaDataManager;

        public DomainEntityInterfaceTemplateRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => DomainEntityInterfaceTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new DomainEntityInterfaceTemplate(model, project);
        }

        public override IEnumerable<IClass> GetModels(Engine.IApplication application)
        {
            return _metaDataManager.GetDomainModels(application);
        }
    }
}
