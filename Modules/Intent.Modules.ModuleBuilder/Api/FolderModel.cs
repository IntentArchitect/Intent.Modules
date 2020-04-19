using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using System.Linq;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FolderModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Folder";
        protected readonly IElement _element;

        public FolderModel(IElement element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a folder from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            _element = element;

            ParentFolder = element.ParentElement?.SpecializationType == SpecializationType ? new FolderModel(element.ParentElement) : null;
            Stereotypes = element.Stereotypes;
        }

        public string Id => _element.Id;
        public string Name => _element.Name;
        public FolderModel ParentFolder { get; }
        public string ParentId => _element.ParentElement.Id;
        public IEnumerable<IStereotype> Stereotypes { get; }

        [IntentManaged(Mode.Fully)]
        public IList<FileTemplateModel> FileTemplates => _element.ChildElements
            .Where(x => x.SpecializationType == Api.FileTemplateModel.SpecializationType)
            .Select(x => new FileTemplateModel(x))
            .ToList<FileTemplateModel>();

        [IntentManaged(Mode.Fully)]
        public IList<FolderModel> Folders => _element.ChildElements
            .Where(x => x.SpecializationType == Api.FolderModel.SpecializationType)
            .Select(x => new FolderModel(x))
            .ToList<FolderModel>();

        [IntentManaged(Mode.Fully)]
        public IList<DecoratorModel> TemplateDecorators => _element.ChildElements
            .Where(x => x.SpecializationType == Api.DecoratorModel.SpecializationType)
            .Select(x => new DecoratorModel(x))
            .ToList<DecoratorModel>();

        public IElement UnderlyingElement => _element;

        [IntentManaged(Mode.Fully)]
        public bool Equals(FolderModel other)
        {
            return Equals(_element, other._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FolderModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IList<TemplateRegistrationModel> TemplateRegistrations => _element.ChildElements
            .Where(x => x.SpecializationType == Api.TemplateRegistrationModel.SpecializationType)
            .Select(x => new TemplateRegistrationModel(x))
            .ToList<TemplateRegistrationModel>();
    }
}