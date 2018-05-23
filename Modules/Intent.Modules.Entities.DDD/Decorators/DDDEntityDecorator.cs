using System.Linq;
using System.Text;
using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Entities.DDD.Decorators
{
    public class DDDEntityDecorator : DomainEntityDecoratorBase
    {
        public const string Identifier = "Intent.Entities.DDD.EntityDecorator";
        
        public override string Constructors(IClass @class)
        {
            if (!@class.Attributes.Any() && !@class.AssociatedClasses.Any())
            {
                return base.Constructors(@class);
            }
            var sb = new StringBuilder();
            sb.AppendLine($"        [IntentInitialGen]");
            sb.AppendLine($"        public {@class.Name}(");
            foreach (var attribute in @class.Attributes)
            {
                sb.AppendLine($"            {Template.Types.Get(attribute.Type)} {attribute.Name.ToCamelCase()}{(attribute != @class.Attributes.Last() ? "," : "")}");
            }
            sb.AppendLine("        )");
            sb.AppendLine("        {");
            foreach (var attribute in @class.Attributes)
            {
                sb.AppendLine($"            {attribute.Name.ToPascalCase()} = {attribute.Name.ToCamelCase()};");
            }
            sb.AppendLine("        }");
            return sb.ToString();
        }
    }
}
