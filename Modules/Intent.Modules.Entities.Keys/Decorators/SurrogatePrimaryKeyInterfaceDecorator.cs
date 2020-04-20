using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Entities.Templates.DomainEntityInterface;

namespace Intent.Modules.Entities.Keys.Decorators
{
    public class SurrogatePrimaryKeyInterfaceDecorator : DomainEntityInterfaceDecoratorBase
    {
        private string _surrogateKeyType = "Guid";

        public const string Identifier = "Intent.Entities.Keys.SurrogatePrimaryKeyInterfaceDecorator";

        public string SurrogateKeyType => "Surrogate Key Type";

        public SurrogatePrimaryKeyInterfaceDecorator(DomainEntityInterfaceTemplate template) : base(template)
        {
        }

        public override string BeforeProperties(ClassModel @class)
        {
            if (@class.ParentClass != null || @class.Attributes.Any(x => x.HasStereotype("Primary Key")))
            {
                return base.BeforeProperties(@class);
            }

            return $@"
        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        {_surrogateKeyType} Id {{ get; }}";
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            if (settings.ContainsKey(SurrogateKeyType) && !string.IsNullOrWhiteSpace(settings[SurrogateKeyType]))
            {
                _surrogateKeyType = settings[SurrogateKeyType];
            }
        }
    }
}
