using Intent.MetaModel.Domain;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.EntityFramework.Templates.DbContext;

namespace Intent.Modules.EF.Templates.DbContext
{
    [Description("Intent EF  DB Context")]
    public class Registrations : ListModelTemplateRegistrationBase<IClass>
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
                return DbContextTemplate.Identifier;
            }
        }

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
