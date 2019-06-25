using System.Collections.Generic;
using Intent.MetaModel;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Configuration;

namespace Intent.Modules.Entities.Keys.Decorators
{
    public class ForeignKeyEntityStateDecorator : DomainEntityStateDecoratorBase, ISupportsConfiguration
    {
        private string _foreignKeyType = "Guid";
        public const string Identifier = "Intent.Entities.Keys.ForeignKeyEntityDecorator";
        public const string ForeignKeyType = "Foreign Key Type";

        public override string AssociationBefore(IAssociationEnd associationEnd)
        {
            if (
                (associationEnd.Multiplicity == Multiplicity.One 
                || associationEnd.Multiplicity == Multiplicity.ZeroToOne 
                || (associationEnd.Multiplicity == Multiplicity.ZeroToOne && associationEnd.Association.TargetEnd == associationEnd)) 
                &&
                associationEnd.OtherEnd().Multiplicity == Multiplicity.Many)
            {
                if (associationEnd.OtherEnd().HasStereotype("Foreign Key"))
                {
                    return base.AssociationBefore(associationEnd);
                }
                return $@"       public {_foreignKeyType}{ (associationEnd.IsNullable ? "?" : "") } { associationEnd.Name().ToPascalCase() }Id {{ get; set; }}
";
            }
            return base.AssociationBefore(associationEnd);
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            if (settings.ContainsKey(ForeignKeyType) && !string.IsNullOrWhiteSpace(settings[ForeignKeyType]))
            {
                _foreignKeyType = settings[ForeignKeyType];
            }
        }
    }
}
