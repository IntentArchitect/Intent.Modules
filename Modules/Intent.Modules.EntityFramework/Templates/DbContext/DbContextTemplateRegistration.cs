using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Modelers.Domain;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Templates.DbContext
{
    [Description(DbContextTemplate.Identifier)]
    public class DbContextTemplateRegistration : ListModelTemplateRegistrationBase<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DbContextTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DbContextTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IList<ClassModel> models)
        {
            return new DbContextTemplate(models, project, project.Application.EventDispatcher);
        }

        public override IList<ClassModel> GetModels(Engine.IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels().ToList();
        }
    }
}
