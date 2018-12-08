using System;
using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Templates;
using IdentityGeneratorTemplate = Intent.Modules.Entities.Keys.Templates.IdentityGenerator.IdentityGeneratorTemplate;

namespace Intent.Modules.Entities.Keys.Decorators
{
    public class SurrogatePrimaryKeyEntityStateDecorator : DomainEntityStateDecoratorBase, IHasTemplateDependencies
    {
        private string _surrogateKeyType = "Guid";
        private bool _usingIdentityGenerator = false;
        public const string Identifier = "Intent.Entities.Keys.SurrogatePrimaryKeyEntityStateDecorator";
        public const string SurrogateKeyType = "Surrogate Key Type";

        public override string BeforeProperties(IClass @class)
        {
            if (@class.ParentClass != null || @class.Attributes.Any(x => x.HasStereotype("Primary Key")))
            {
                return base.BeforeProperties(@class);
            }

            if (_surrogateKeyType.Trim().Equals("Guid", StringComparison.InvariantCultureIgnoreCase))
            {
                _usingIdentityGenerator = true;
                return @"
        private Guid? _id = null;

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual Guid Id
        {
            get { return _id ?? (_id = IdentityGenerator.NewSequentialId()).Value; }
            set { _id = value; }
        }";
            }

            return $@"
        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual {_surrogateKeyType} Id {{ get; set; }}";
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            if (!_usingIdentityGenerator)
            {
                return new List<ITemplateDependancy>();
            }
            return new[] { TemplateDependancy.OnTemplate(IdentityGeneratorTemplate.Identifier) };
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
