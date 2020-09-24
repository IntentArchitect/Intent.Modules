using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Angular.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ComponentModel : IHasStereotypes, IMetadataModel
    {

        [IntentManaged(Mode.Ignore)]
        public ComponentModel(IElement element, string requiredType = SpecializationType)
        {
            _element = element;
        }

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;
        public string Comment => _element.Comment;

        public ModuleModel Module => new ModuleModel(_element.GetParentPath().Reverse().First(x => x.SpecializationType == ModuleModel.SpecializationType));

        [IntentManaged(Mode.Fully)]
        public bool Equals(ComponentModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComponentModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
        protected readonly IElement _element;
        public const string SpecializationType = "Component";

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IList<ComponentCommandModel> Commands => _element.ChildElements
            .Where(x => x.SpecializationType == ComponentCommandModel.SpecializationType)
            .Select(x => new ComponentCommandModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ComponentModelModel> Models => _element.ChildElements
            .Where(x => x.SpecializationType == ComponentModelModel.SpecializationType)
            .Select(x => new ComponentModelModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ModelDefinitionModel> ModelDefinitions => _element.ChildElements
            .Where(x => x.SpecializationType == ModelDefinitionModel.SpecializationType)
            .Select(x => new ModelDefinitionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public ComponentViewModel View => _element.ChildElements
            .Where(x => x.SpecializationType == ComponentViewModel.SpecializationType)
            .Select(x => new ComponentViewModel(x))
            .SingleOrDefault();
        public const string SpecializationTypeId = "b1c481e1-e91e-4c29-9817-00ab9cad4b6b";

    }
}