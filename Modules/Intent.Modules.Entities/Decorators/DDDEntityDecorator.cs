using System;
using System.Linq;
using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.Packages.Entities.Templates;
using Intent.Packages.Entities.Templates.DomainEntity;
using Intent.SoftwareFactory.Configuration;

namespace Intent.Packages.Entities.Decorators
{
    public class DDDEntityDecorator : AbstractDomainEntityDecorator, ISupportsConfiguration
    {
        public const string Id = "Intent.DDD.Entity.Decorator";

        private const string AggregateRootBaseClassSetting = "Aggregate Root Base Class";
        private const string EntityBaseClassSetting = "Entity Base Class";
        private const string ValueObjectBaseClassSetting = "Value Object Base Class";

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
            var baseClass = @class.Stereotypes.GetProperty<string>("AggregateRoot", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            baseClass = @class.Stereotypes.GetProperty<string>("Entity", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            baseClass = @class.Stereotypes.GetProperty<string>("ValueObject", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            if (_aggregateRootBaseClass != null && @class.Stereotypes.FirstOrDefault(s => s.Name == "AggregateRoot") != null)
            {
                return _aggregateRootBaseClass;
            }
            if (_entityBaseClass != null && @class.Stereotypes.FirstOrDefault(s => s.Name == "Entity") != null)
            {
                return _entityBaseClass;
            }
            if (_valueObjectBaseClass != null && @class.Stereotypes.FirstOrDefault(s => s.Name == "ValueObject") != null)
            {
                return _valueObjectBaseClass;
            }
            return base.GetBaseClass(@class);
        }
    }
}
