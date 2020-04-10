using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public class FileTemplate : TemplateRegistration, IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "File Template";
        private readonly IElement _element;

        public FileTemplate(IElement element) : base(element)
        {
            if (element.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element type {element.SpecializationType}", nameof(element));
            }
            _element = element;
            //Folder = element.ParentElement != null ? new Folder(element.ParentElement) : null;
        }

        //public string Id => _element.Id;

        //public string Name => _element.Name;

        //public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        //public IFolder Folder { get; }

        protected bool Equals(FileTemplate other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileTemplate)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}