using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class CSharpTemplate : TemplateRegistration, ICSharpTemplate
    {
        public const string SpecializationType = "C# Template";
        private readonly IElement _element;

        public CSharpTemplate(IElement element) : base(element)
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

        protected bool Equals(CSharpTemplate other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CSharpTemplate)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public string GetExposedDecoratorContractType()
        {
            return string.Empty;
        }
    }
}