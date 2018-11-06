using Intent.MetaModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Decorators
{
    public class OrphanDetectionDomainEntityStateDecorator : DomainEntityStateDecoratorBase
    {
        public const string Identifier = "Intent.Entities.OrphanDetectionDomainEntityStateDecorator";

        public override IEnumerable<string> GetInterfaces(IClass @class)
        {
            if (!CanBeOrphaned(@class))
            {
                return base.GetInterfaces(@class);
            }
            return new List<string>() { "ICanBeOrphaned" };
        }

        public override IEnumerable<string> DeclareUsings()
        {
            return new List<string>() { "Intent.Framework.Domain" };
        }

        public override string BeforeProperties(IClass @class)
        {
            if (!CanBeOrphaned(@class))
            {
                return base.BeforeProperties(@class);
            }
            return $@"
        bool ICanBeOrphaned.IsOrphan()
        {{
            return {GetOrphanableAssociations(@class).First().Name().ToPascalCase()} == null;
        }}";
        }

        private List<IAssociationEnd> GetOrphanableAssociations(IClass model)
        {
            var result = new List<IAssociationEnd>();
            foreach (var a in model.AssociatedClasses)
            {
                if (a.Association.AssociationType == AssociationType.Composition && a.Association.SourceEnd.Multiplicity == Multiplicity.One && a == a.Association.SourceEnd)
                {
                    result.Add(a);
                }
            }
            return result;
        }

        private bool CanBeOrphaned(IClass @class)
        {
            return GetOrphanableAssociations(@class).Any();
        }
    }
}
