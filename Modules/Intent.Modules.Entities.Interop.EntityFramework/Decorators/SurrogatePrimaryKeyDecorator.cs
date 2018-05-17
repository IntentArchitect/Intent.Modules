using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Interop.EntityFramework.Templates.IdentityGenerator;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Configuration;
using Intent.Modules.Entities.Templates;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    public class SurrogatePrimaryKeyDecorator : AbstractDomainEntityDecorator, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.Entities.Interop.EntityFramework.SurrogatePrimaryKeyEntityDecorator";

        public override string BeforeProperties(IClass @class)
        {
            return @"
        private Guid? _id = null;

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual Guid Id
        {
            get { return _id ?? (_id = IdentityGenerator.NewSequentialId()).Value; }
            set { _id = value; }
        }";
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[] { TemplateDependancy.OnTemplate(IdentityGeneratorTemplate.Identifier) };
        }
    }
}
