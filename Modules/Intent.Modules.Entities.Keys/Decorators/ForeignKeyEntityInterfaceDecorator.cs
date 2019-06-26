using System.Collections.Generic;
using Intent.MetaModel;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.SoftwareFactory.Configuration;

namespace Intent.Modules.Entities.Keys.Decorators
{
    public class ForeignKeyEntityInterfaceDecorator : DomainEntityInterfaceDecoratorBase, ISupportsConfiguration
    {
        private string _foreignKeyType = "Guid";
        public const string Identifier = "Intent.Entities.Keys.ForeignKeyEntityInterfaceDecorator";
        public const string ForeignKeyType = "Foreign Key Type";

        public override string PropertyBefore(IAssociationEnd associationEnd)
        {
            if (RequiresForeignKey(associationEnd))
            {
                if (associationEnd.OtherEnd().HasStereotype("Foreign Key"))
                {
                    return base.PropertyBefore(associationEnd);
                }
                return $@"       {_foreignKeyType}{ (associationEnd.IsNullable ? "?" : "") } { associationEnd.Name().ToPascalCase() }Id {{ get; }}
";
            }
            return base.PropertyBefore(associationEnd);
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            if (settings.ContainsKey(ForeignKeyType) && !string.IsNullOrWhiteSpace(settings[ForeignKeyType]))
            {
                _foreignKeyType = settings[ForeignKeyType];
            }
        }

        private static bool RequiresForeignKey(IAssociationEnd associationEnd)
        {
            return IsManyToVariantsOfOne(associationEnd) || IsSelfReferencingZeroToOne(associationEnd);
        }

        private static bool IsManyToVariantsOfOne(IAssociationEnd associationEnd)
        {
            return (associationEnd.Multiplicity == Multiplicity.One || associationEnd.Multiplicity == Multiplicity.ZeroToOne)
                    && associationEnd.OtherEnd().Multiplicity == Multiplicity.Many;
        }

        private static bool IsSelfReferencingZeroToOne(IAssociationEnd associationEnd)
        {
            return associationEnd.Multiplicity == Multiplicity.ZeroToOne && associationEnd.Association.TargetEnd.Class == associationEnd.Association.SourceEnd.Class;
        }
    }
}
