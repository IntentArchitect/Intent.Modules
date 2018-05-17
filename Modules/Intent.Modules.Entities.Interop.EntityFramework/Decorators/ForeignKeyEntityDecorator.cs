using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Configuration;
using Intent.Modules.Entities.Templates;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    public class ForeignKeyEntityDecorator : AbstractDomainEntityDecorator, ISupportsConfiguration
    {
        public const string Identifier = "Intent.Entities.Interop.EntityFramework.ForeignKeyEntityDecorator";

        public override string AssociationBefore(IAssociationEnd associationEnd)
        {
            if (associationEnd.Multiplicity == Multiplicity.One)
            {
                return $@"       public virtual Guid{ (associationEnd.IsNullable ? "?" : "") } { associationEnd.Name() }Id {{ get; set; }}
";
            }
            return base.AssociationBefore(associationEnd);
        }
    }
}
