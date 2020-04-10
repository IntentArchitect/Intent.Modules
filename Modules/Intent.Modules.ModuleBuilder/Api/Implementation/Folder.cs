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
    public class Folder : IHasStereotypes
    {
        public const string SpecializationType = "Folder";
        private readonly IElement _element;

        public Folder(IElement element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a folder from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            _element = element;

            ParentFolder = element.ParentElement?.SpecializationType == SpecializationType ? new Folder(element.ParentElement) : null;
            Stereotypes = element.Stereotypes;
        }

        public string Id => _element.Id;
        public string Name => _element.Name;
        public Folder ParentFolder { get; }
        public string ParentId => _element.ParentElement.Id;
        public IEnumerable<IStereotype> Stereotypes { get; }

        [IntentManaged(Mode.Fully)]
        public IList<FileTemplate> FileTemplates => _element.ChildElements
            .Where(x => x.SpecializationType == Api.FileTemplate.SpecializationType)
            .Select(x => new FileTemplate(x))
            .ToList<FileTemplate>();

        [IntentManaged(Mode.Fully)]
        public IList<Folder> Folders => _element.ChildElements
            .Where(x => x.SpecializationType == Api.Folder.SpecializationType)
            .Select(x => new Folder(x))
            .ToList<Folder>();

        [IntentManaged(Mode.Fully)]
        public IList<Decorator> TemplateDecorators => _element.ChildElements
            .Where(x => x.SpecializationType == Api.Decorator.SpecializationType)
            .Select(x => new Decorator(x))
            .ToList<Decorator>();

        [IntentManaged(Mode.Fully)]
        public IList<TypeDefinition> Types => _element.ChildElements
            .Where(x => x.SpecializationType == Api.TypeDefinition.SpecializationType)
            .Select(x => new TypeDefinition(x))
            .ToList<TypeDefinition>();

        public IElement UnderlyingElement => _element;

        protected bool Equals(Folder other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Folder)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IList<TemplateRegistration> TemplateRegistrations => _element.ChildElements
            .Where(x => x.SpecializationType == Api.TemplateRegistration.SpecializationType)
            .Select(x => new TemplateRegistration(x))
            .ToList<TemplateRegistration>();
    }
}