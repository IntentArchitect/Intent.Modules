using Intent.Modules.RichDomain.EntityFramework.Templates.EFMapping;
using Intent.SoftwareFactory.MetaModels.UMLModel;

namespace Intent.Modules.RichDomain.Auditing.Decorators
{
    public class UserContextEFMappingTemplateDecorator : IEFMappingTemplateDecorator
    {
        public const string Identifier = "Intent.RichDomain.Auditing.EFMapping";
        public string[] PropertyMappings(Class @class)
        {
            if (!@class.IsAggregateRoot())
            {
                return new string[0];
            }

            return new[] { @"            this.Property(x => x.CreatedBy)
                .HasMaxLength(50);
" };
        }
    }
}