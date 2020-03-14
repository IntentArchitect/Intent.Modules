using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class Folder : IFolder
    {
        public const string SpecializationType = "Folder";
        private readonly IElement _element;

        public Folder(IElement element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Cannot create a folder from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            _element = element;

            ParentFolder = element.ParentElement != null ? new Folder(element.ParentElement) : null;
            Stereotypes = element.Stereotypes;
        }

        public string Id => _element.Id;
        public string Name => _element.Name;
        public IFolder ParentFolder { get; }
        public IEnumerable<IStereotype> Stereotypes { get; }

        public IList<ICSharpTemplate> CSharpTemplates { get; }

        public IList<IFileTemplate> FileTemplates { get; }

        public IList<IFolder> Folders { get; }

        public IList<IModelerReference> ModelerReferences { get; }

        public IList<IStereotypeDefinition> StereotypeDefinitions { get; }

        public IList<IDecorator> TemplateDecorators { get; }

        public IList<ITypeDefinition> Types { get; }

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
    }
}