using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ElementMappingModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Element Mapping";
        protected readonly IElement _element;

        public ElementMappingModel(IElement element)
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
        public MappingCriteriaModel Criteria => _element.ChildElements
            .Where(x => x.SpecializationType == MappingCriteriaModel.SpecializationType)
            .Select(x => new MappingCriteriaModel(x))
            .SingleOrDefault();
        [IntentManaged(Mode.Fully)]
        public MappingOutputModel Output => _element.ChildElements
            .Where(x => x.SpecializationType == MappingOutputModel.SpecializationType)
            .Select(x => new MappingOutputModel(x))
            .SingleOrDefault();
        [IntentManaged(Mode.Fully)]
        public IList<ElementMappingModel> ChildMappings => _element.ChildElements
            .Where(x => x.SpecializationType == ElementMappingModel.SpecializationType)
            .Select(x => new ElementMappingModel(x))
            .ToList();

        public ElementMappingSettingPersistable ToPersistable()
        {
            return new ElementMappingSettingPersistable()
            {
                Criteria = Criteria.ToPersistable(),
                MapTo = Output.ToPersistable(),
                ChildMappingSettings = ChildMappings.Select(x => x.ToPersistable()).ToList()
            };
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(ElementMappingModel other)
        {
            return Equals(_element, other._element);
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
    }
}