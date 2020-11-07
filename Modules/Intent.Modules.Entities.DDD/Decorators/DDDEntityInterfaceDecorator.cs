using System.Collections;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Templates;
using AssociationEndModel = Intent.Modelers.Domain.Api.AssociationEndModel;

namespace Intent.Modules.Entities.Decorators
{
    public class DDDEntityInterfaceDecorator : DomainEntityInterfaceDecoratorBase
    {
        public const string Id = "Intent.Entities.DDD.EntityInterfaceDecorator";

        public DDDEntityInterfaceDecorator(DomainEntityInterfaceTemplate template) : base(template)
        {
        }

        public override string ConvertAttributeType(AttributeModel attribute)
        {
            //var @namespace = attribute.Type.GetStereotypeProperty<string>("CommonType", "Namespace");
            //if (@namespace != null)
            //{
            //    return @namespace + "." + attribute.TypeName();
            //}
            var domainType = attribute.GetStereotypeProperty<string>("DomainType", "Type");
            if (domainType != null)
            {
                return domainType;
            }
            return base.ConvertAttributeType(attribute);
        }

        public override string AttributeAccessors(AttributeModel attribute)
        {
            return "get;";
        }

        public override bool CanWriteDefaultAssociation(AssociationEndModel association)
        {
            return false;
        }

        public override string PropertyBefore(AssociationEndModel associationEnd)
        {
            if (!associationEnd.IsNavigable)
            {
                return base.PropertyBefore(associationEnd);
            }
            var t = CSharpTypeSource.Create(Template.ExecutionContext, DomainEntityInterfaceTemplate.Identifier);
            return $@"
        {Template.NormalizeNamespace(t.GetType(associationEnd).Name)} {associationEnd.Name().ToPascalCase()} {{ get; }}
";
        }
    }
}
