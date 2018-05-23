using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Configuration;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    public class ForeignKeyEntityStateDecorator : DomainEntityStateDecoratorBase, ISupportsConfiguration
    {
        public const string Identifier = "Intent.Entities.Interop.EntityFramework.ForeignKeyEntityDecorator";

        public override string AssociationBefore(IAssociationEnd associationEnd)
        {
            if (associationEnd.Multiplicity == Multiplicity.One && associationEnd.OtherEnd().Multiplicity == Multiplicity.Many)
            {
                return $@"       public virtual Guid{ (associationEnd.IsNullable ? "?" : "") } { associationEnd.Name() }Id {{ get; set; }}
";
            }
            return base.AssociationBefore(associationEnd);
        }
    }
}
