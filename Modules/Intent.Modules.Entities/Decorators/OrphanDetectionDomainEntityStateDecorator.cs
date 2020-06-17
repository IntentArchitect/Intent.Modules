using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Decorators
{
    public class OrphanDetectionDomainEntityStateDecorator : DomainEntityStateDecoratorBase
    {
        public const string Identifier = "Intent.Entities.OrphanDetectionDomainEntityStateDecorator";

        public OrphanDetectionDomainEntityStateDecorator(DomainEntityStateTemplate template) : base(template)
        {
        }

        public override IEnumerable<string> GetInterfaces(ClassModel @class)
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

        public override string BeforeProperties(ClassModel @class)
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

        private List<AssociationEndModel> GetOrphanableAssociations(ClassModel model)
        {
            var result = new List<AssociationEndModel>();
            foreach (var a in model.AssociatedFromClasses())
            {
                if (a.Association.AssociationType == AssociationType.Composition && !a.Association.SourceEnd.IsNullable && a.Association.TargetEnd.IsCollection)
                {
                    result.Add(a);
                }
            }
            return result;
        }

        private bool CanBeOrphaned(ClassModel @class)
        {
            return GetOrphanableAssociations(@class).Any();
        }
    }
}
