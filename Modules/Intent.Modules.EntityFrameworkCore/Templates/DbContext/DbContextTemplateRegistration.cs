using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.Templates

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContext
{
    [Description(DbContextTemplate.Identifier)]
    public class DbContextTemplateRegistration : ListModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;

        public DbContextTemplateRegistration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => DbContextTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IList<IClass> models)
        {
            return new DbContextTemplate(models, project, project.Application.EventDispatcher);
        }

        public override IList<IClass> GetModels(SoftwareFactory.Engine.IApplication application)
        {
            return _metaDataManager.GetDomainModels(application).ToList();
        }
    }
}
