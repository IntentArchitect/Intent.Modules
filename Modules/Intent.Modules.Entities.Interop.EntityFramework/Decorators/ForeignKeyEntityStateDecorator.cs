using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Configuration;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Interop.EntityFramework.Decorators
{
    public class ForeignKeyEntityStateDecorator : DomainEntityStateDecoratorBase, ISupportsConfiguration
    {
        private string _foreignKeyType = "Guid";
        public const string Identifier = "Intent.Entities.Interop.EntityFramework.ForeignKeyEntityDecorator";
        public const string ForeignKeyType = "Foreign Key Type";

        public override string AssociationBefore(IAssociationEnd associationEnd)
        {
            if (associationEnd.Multiplicity == Multiplicity.One && associationEnd.OtherEnd().Multiplicity == Multiplicity.Many)
            {
                return $@"       public virtual {_foreignKeyType}{ (associationEnd.IsNullable ? "?" : "") } { associationEnd.Name() }Id {{ get; set; }}
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
