using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ElementMappingModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Element Mapping";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ElementMappingModel(IElement element, string requiredType = SpecializationType)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'ElementMappingModel' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        [IntentManaged(Mode.Fully)]
        public IList<ElementMappingModel> ChildMappings => _element.ChildElements
            .GetElementsOfType(ElementMappingModel.SpecializationTypeId)
            .Select(x => new ElementMappingModel(x))
            .ToList();

        public ElementMappingSettingPersistable ToPersistable()
        {
            return new ElementMappingSettingPersistable()
            {
                Id = Id,
                Criteria = new ElementMappingCriteriaSettingPersistable()
                {
                    SpecializationType = this.GetCriteriaSettings().FromType().Name,
                    HasTypeReference = this.GetCriteriaSettings().HasTypeReference().IsYes() ? true :
                        this.GetCriteriaSettings().HasTypeReference().IsNo() ? false : (bool?)null,
                    HasChildren = this.GetCriteriaSettings().HasChildren().IsYes() ? true :
                        this.GetCriteriaSettings().HasChildren().IsNo() ? false : (bool?)null,
                    IsCollection = this.GetCriteriaSettings().IsCollection().IsYes() ? true :
                        this.GetCriteriaSettings().IsCollection().IsNo() ? false : (bool?)null,
                },
                MapTo = new ElementMappingMapToSettingPersistable()
                {
                    SpecializationType = this.GetOutputSettings().ToType()?.Name,
                    ChildMappingMode = this.GetOutputSettings().ChildMappingMode().IsTraverse() ? ChildMappingMode.Traverse : ChildMappingMode.MapToChild,
                    UseMappingSettings = this.GetOutputSettings().UseMappingSettings()?.Id
                },
                Behaviour = new ElementMappingBehaviourPersistable()
                {
                    AutoSelectChildren = this.GetBehaviourSettings().AutoSelectChildren()
                },
                ChildMappingSettings = ChildMappings.Select(x => x.ToPersistable()).ToList()
            };
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(ElementMappingModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementMappingModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;
        public const string SpecializationTypeId = "4a16943b-702c-4fb0-bfcc-2afd98b8814c";

        public string Comment => _element.Comment;
    }
}