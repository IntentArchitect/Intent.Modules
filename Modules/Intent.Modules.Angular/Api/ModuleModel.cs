using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Angular.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ModuleModel : IHasStereotypes, IMetadataModel
    {
        protected readonly IElement _element;
        public const string SpecializationType = "Module";

        public ModuleModel(IElement element)
        {
            _element = element;
        }

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;
        public IElementApplication Application => _element.Application;

        [IntentManaged(Mode.Fully)]
        public IList<ServiceProxyModel> ServiceProxies => _element.ChildElements
            .Where(x => x.SpecializationType == ServiceProxyModel.SpecializationType)
            .Select(x => new ServiceProxyModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ModelDefinitionModel> ModelDefinitions => _element.ChildElements
            .Where(x => x.SpecializationType == ModelDefinitionModel.SpecializationType)
            .Select(x => new ModelDefinitionModel(x))
            .ToList();

        public string Comment => _element.Comment;

        [IntentManaged(Mode.Fully)]
        public IList<ComponentModel> Components => _element.ChildElements
            .Where(x => x.SpecializationType == ComponentModel.SpecializationType)
            .Select(x => new ComponentModel(x))
            .ToList();


        public string GetModuleName()
        {
            if (Name.EndsWith("Module", StringComparison.InvariantCultureIgnoreCase))
            {
                return Name.Substring(0, Name.Length - "Module".Length);
            }
            return Name;
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(ModuleModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModuleModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }


        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IList<FormGroupDefinitionModel> FormGroups => _element.ChildElements
            .Where(x => x.SpecializationType == FormGroupDefinitionModel.SpecializationType)
            .Select(x => new FormGroupDefinitionModel(x))
            .ToList();
        public const string SpecializationTypeId = "cac14331-198a-4f9a-bbb9-171eb0bd4efe";

        [IntentManaged(Mode.Ignore)]
        public ModuleModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }
    }
}