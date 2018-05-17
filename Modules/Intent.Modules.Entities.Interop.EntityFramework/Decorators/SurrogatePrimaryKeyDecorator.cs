using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Configuration;
using Intent.Modules.Entities.Templates;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    public class SurrogatePrimaryKeyDecorator : AbstractDomainEntityDecorator, ISupportsConfiguration
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
    }
}
