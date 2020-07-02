using System.Linq;
using System.Text;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates;
using Intent.Templates;

namespace Intent.Modules.Entities.DDD.Decorators
{
    public class DDDEntityDecorator : DomainEntityDecoratorBase
    {
        public const string Identifier = "Intent.Entities.DDD.EntityDecorator";

        public DDDEntityDecorator(DomainEntityTemplate template) : base(template)
        {
        }

        public override string Constructors(ClassModel @class)
        {
            var associatedClass = @class.AssociatedClasses.Where(x => x.IsNavigable).ToList();
            if (!@class.Attributes.Any() && !@class.AssociatedClasses.Any())
            {
                return base.Constructors(@class);
            }

            var sb = new StringBuilder();
            sb.AppendLine($"        [IntentInitialGen]");
            sb.Append($"        public {@class.Name}(");
            sb.Append($"{string.Join(", ", @class.Attributes.Select(x => Template.GetTypeName(x.Type)+ " " + x.Name.ToCamelCase()))}");
            sb.Append($"{(@class.Attributes.Any() && associatedClass.Any() ? ", " : "")}");
            sb.Append($"{string.Join(", ", associatedClass.Select(x => Template.GetTypeName(x)+ " " + x.Name().ToCamelCase()))}");
            //foreach (var attribute in @class.Attributes)
            //{
            //    sb.Append($"{Template.Types.Get(attribute.Type)} {attribute.Name.ToCamelCase()}{(associatedClass.Any() || attribute != @class.Attributes.Last() ? ", " : "")}");
            //}
            //foreach (var associationEnd in associatedClass)
            //{
            //    sb.Append($"{Template.NormalizeNamespace(Template.Types.Get(associationEnd))} {associationEnd.Name().ToCamelCase()}{(associationEnd != associatedClass.Last() ? ", " : "")}");
            //}
            sb.Append(")");
            sb.AppendLine("        {");
            foreach (var attribute in @class.Attributes)
            {
                sb.AppendLine($"            {attribute.Name.ToPascalCase()} = {attribute.Name.ToCamelCase()};");
            }
            foreach (var associationEnd in associatedClass)
            {
                var cast = associationEnd.IsCollection ? $"(ICollection<{associationEnd.Class.Name}>)" : $"({associationEnd.Class.Name})";
                sb.AppendLine($"            {associationEnd.Name().ToPascalCase()} = {cast}{associationEnd.Name().ToCamelCase()};");
            }
            sb.AppendLine("        }");
            return sb.ToString();
        }
    }
}
