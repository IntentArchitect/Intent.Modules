using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Entities.Decorators
{
    public class DDDEntityDecorator : AbstractDomainEntityDecorator, ISupportsConfiguration
    {
        public const string Id = "Intent.Entities.DDD.EntityDecorator";

        private const string AggregateRootBaseClassSetting = "Aggregate Root Base Class";
        private const string EntityBaseClassSetting = "Entity Base Class";
        private const string ValueObjectBaseClassSetting = "Value Object Base Class";
        private const string DefaultBaseClassSetting  = "Default Base Class";

        private string _aggregateRootBaseClass = null;
        private string _entityBaseClass = null;
        private string _valueObjectBaseClass = null;

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            if (settings.ContainsKey(AggregateRootBaseClassSetting) && !string.IsNullOrWhiteSpace(settings[AggregateRootBaseClassSetting]))
            {
                _aggregateRootBaseClass = settings[AggregateRootBaseClassSetting];
            }
            if (settings.ContainsKey(EntityBaseClassSetting) && !string.IsNullOrWhiteSpace(settings[EntityBaseClassSetting]))
            {
                _entityBaseClass = settings[EntityBaseClassSetting];
            }
            if (settings.ContainsKey(ValueObjectBaseClassSetting) && !string.IsNullOrWhiteSpace(settings[ValueObjectBaseClassSetting]))
            {
                _valueObjectBaseClass = settings[ValueObjectBaseClassSetting];
            }
        }

        public override string ConvertAttributeType(IAttribute attribute)
        {
            var @namespace = attribute.Type.Stereotypes.GetProperty<string>("CommonType", "Namespace");
            if (@namespace != null)
            {
                return @namespace + "." + attribute.TypeName();
            }
            var domainType = attribute.Stereotypes.GetProperty<string>("DomainType", "Type");
            if (domainType != null)
            {
                return domainType;
            }
            return base.ConvertAttributeType(attribute);
        }

        public override string GetBaseClass(IClass @class)
        {
            var baseClass = @class.Stereotypes.GetProperty<string>("Aggregate Root", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            baseClass = @class.Stereotypes.GetProperty<string>("Entity", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            baseClass = @class.Stereotypes.GetProperty<string>("Value Object", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            if (_aggregateRootBaseClass != null && @class.Stereotypes.FirstOrDefault(s => s.Name == "Aggregate Root") != null)
            {
                return _aggregateRootBaseClass;
            }
            if (_entityBaseClass != null && @class.Stereotypes.FirstOrDefault(s => s.Name == "Entity") != null)
            {
                return _entityBaseClass;
            }
            if (_valueObjectBaseClass != null && @class.Stereotypes.FirstOrDefault(s => s.Name == "Value Object") != null)
            {
                return _valueObjectBaseClass;
            }
            return base.GetBaseClass(@class);
        }
    }
}
