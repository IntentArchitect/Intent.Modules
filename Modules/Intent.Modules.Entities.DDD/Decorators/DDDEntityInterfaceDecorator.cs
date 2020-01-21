using System.Collections;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Templates;
using IAssociationEnd = Intent.Modelers.Domain.Api.IAssociationEnd;

namespace Intent.Modules.Entities.Decorators
{
    public class DDDEntityInterfaceDecorator : DomainEntityInterfaceDecoratorBase
    {
        public const string Id = "Intent.Entities.DDD.EntityInterfaceDecorator";

        public DDDEntityInterfaceDecorator(DomainEntityInterfaceTemplate template) : base(template)
        {
        }

        public override string ConvertAttributeType(IAttribute attribute)
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

        public override string AttributeAccessors(IAttribute attribute)
        {
            return "get;";
        }

        public override bool CanWriteDefaultAssociation(IAssociationEnd association)
        {
            return false;
        }

        public override string PropertyBefore(IAssociationEnd associationEnd)
        {
            if (!associationEnd.IsNavigable)
            {
                return base.PropertyBefore(associationEnd);
            }
            var t = CSharpTypeSource.InProject(Template.Project, DomainEntityInterfaceTemplate.Identifier);
            return $@"
        {Template.NormalizeNamespace(t.GetClassType(associationEnd))} {associationEnd.Name().ToPascalCase()} {{ get; }}
";
        }

        public override bool CanWriteDefaultOperation(IOperation operation)
        {
            return !operation.HasStereotype("Command Operation");
        }
    }
}
