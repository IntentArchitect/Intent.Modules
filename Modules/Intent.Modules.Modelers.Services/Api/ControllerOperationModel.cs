using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ControllerOperationModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Controller Operation";
        private readonly IElement _element;

        public ControllerOperationModel(IElement element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'ControllerOperationModel' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;

        [IntentManaged(Mode.Fully)]
        public IList<ControllerParameterModel> Parameters => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ControllerParameterModel.SpecializationType)
            .Select(x => new ControllerParameterModel(x))
            .ToList<ControllerParameterModel>();

        protected bool Equals(ControllerOperationModel other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ControllerOperationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}