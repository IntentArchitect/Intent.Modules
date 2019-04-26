using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Plugins;

namespace Intent.Modules.Entities.Keys.Decorators
{
    public class BidirectionalOneToManyEntityStateDecorator : DomainEntityStateDecoratorBase, ISupportsConfiguration
    {
        public const string Identifier = "Intent.Entities.Keys.BidirectionalOneToManyEntityDecorator";

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
