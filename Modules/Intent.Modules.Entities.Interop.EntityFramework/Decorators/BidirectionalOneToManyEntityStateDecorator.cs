using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Configuration;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    public class BidirectionalOneToManyEntityStateDecorator : DomainEntityStateDecoratorBase, ISupportsConfiguration
    {
        public const string Identifier = "Intent.Entities.Interop.EntityFramework.BidirectionalOneToManyEntityDecorator";

        public override string AssociationBefore(IAssociationEnd associationEnd)
        {
            if (!associationEnd.IsNavigable && associationEnd.Multiplicity == Multiplicity.One && associationEnd.OtherEnd().Multiplicity == Multiplicity.Many)
            {
                return $@"       public virtual { Template.Types.Get(associationEnd) } { associationEnd.Name() } {{ get; set; }}
";
            }
            return base.AssociationBefore(associationEnd);
        }
    }
}
