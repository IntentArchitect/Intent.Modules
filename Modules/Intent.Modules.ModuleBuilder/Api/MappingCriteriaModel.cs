using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MappingCriteriaModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Mapping Criteria";
        protected readonly IElement _element;

        public MappingCriteriaModel(IElement element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'MappingCriteriaModel' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

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
        public bool Equals(MappingCriteriaModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappingCriteriaModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;
    }
}