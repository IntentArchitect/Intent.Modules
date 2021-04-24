using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Api
{
    [IntentManaged(Mode.Merge)]
    public class ComponentModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Component";
        public const string SpecializationTypeId = "47064ef1-2295-4287-8301-c446910a1b1e";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ComponentModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;

        public IList<ModuleModel> Modules => _element.ChildElements
            .GetElementsOfType(ModuleModel.SpecializationTypeId)
            .Select(x => new ModuleModel(x))
            .ToList();

        [IntentManaged(Mode.Ignore)]
        public bool IncludeByDefault => this.GetComponentSettings().IncludeByDefault();

        [IntentManaged(Mode.Ignore)]
        public string Description => this.GetComponentSettings().Description();

        [IntentManaged(Mode.Ignore)]
        public IIconModel Icon => this.GetComponentSettings().Icon();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ComponentModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComponentModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public string Comment => _element.Comment;
    }
}