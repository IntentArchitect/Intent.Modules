using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MappingCriteriaModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Mapping Criteria";
        private readonly IElement _element;

        public MappingCriteriaModel(IElement element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'MappingCriteriaModel' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;


        protected bool Equals(MappingCriteriaModel other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappingCriteriaModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public ElementMappingCriteriaSettingPersistable ToPersistable()
        {
            return new ElementMappingCriteriaSettingPersistable()
            {
                SpecializationType = new ElementSettingsModel(_element.TypeReference.Element).Name,
                HasTypeReference = this.GetCriteriaSettings().HasTypeReference().IsYes() ? true :
                    this.GetCriteriaSettings().HasTypeReference().IsNo() ? false : (bool?)null,
                HasChildren = this.GetCriteriaSettings().HasChildren().IsYes() ? true :
                    this.GetCriteriaSettings().HasChildren().IsNo() ? false : (bool?)null,
                IsCollection = this.GetCriteriaSettings().IsCollection().IsYes() ? true :
                    this.GetCriteriaSettings().IsCollection().IsNo() ? false : (bool?)null,
            };
        }

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;
    }
}