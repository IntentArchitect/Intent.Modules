using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.Angular.Layout.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FormModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Form";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public FormModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            if (InternalElement.IsMapped)
            {
                DataModelPath = string.Join(".", InternalElement.MappedElement.Path.Select(x => x.Name.ToCamelCase()));
            }
        }

        public ModuleModel Module => new ModuleModel(_element.GetParentPath().Reverse().First(x => x.SpecializationType == ModuleModel.SpecializationType));
        public string DataModelPath { get; } = "";

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public bool IsMapped => _element.IsMapped;

        [IntentManaged(Mode.Fully)]
        public IElementMapping Mapping => _element.MappedElement;

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public IList<FormFieldModel> FormFields => _element.ChildElements
            .Where(x => x.SpecializationType == FormFieldModel.SpecializationType)
            .Select(x => new FormFieldModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(FormModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FormModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public bool IsValid()
        {
            return true;
        }
        public const string SpecializationTypeId = "8aee9b69-d02d-4ca8-b28a-6585508bd033";
    }
}