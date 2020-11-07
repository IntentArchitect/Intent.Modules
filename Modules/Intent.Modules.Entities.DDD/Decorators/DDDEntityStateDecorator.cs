using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp;
using Intent.Plugins;
using Intent.Templates;

namespace Intent.Modules.Entities.DDD.Decorators
{
    public class DDDEntityStateDecorator : DomainEntityStateDecoratorBase, ISupportsConfiguration
    {
        public const string Identifier = "Intent.Entities.DDD.EntityStateDecorator";

        private const string AggregateRootBaseClassSetting = "Aggregate Root Base Class";

        private const string EntityBaseClassSetting = "Entity Base Class";

        private const string ValueObjectBaseClassSetting = "Value Object Base Class";

        private const string DefaultBaseClassSetting = "Default Base Class";

        private string _aggregateRootBaseClass = null;

        private string _entityBaseClass = null;

        private string _valueObjectBaseClass = null;

        public DDDEntityStateDecorator(DomainEntityStateTemplate template) : base(template)
        {
        }

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

        public override string GetBaseClass(ClassModel @class)
        {
            var baseClass = @class.GetStereotypeProperty<string>("Aggregate Root", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            baseClass = @class.GetStereotypeProperty<string>("Entity", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            baseClass = @class.GetStereotypeProperty<string>("Value Object", "BaseType");
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
        
        public override string AssociationAfter(AssociationEndModel associationEnd)
        {
            if (!associationEnd.IsNavigable)
            {
                return base.AssociationBefore(associationEnd);
            }

            var t = CSharpTypeSource.Create(Template.ExecutionContext, DomainEntityInterfaceTemplate.Identifier, "IEnumerable<{0}>");
            return $@"
        {Template.NormalizeNamespace(t.GetType(associationEnd).Name)} I{associationEnd.OtherEnd().Class.Name}.{associationEnd.Name().ToPascalCase()} => {associationEnd.Name().ToPascalCase()};
";
        }
    }
}
