using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class PackageSettings : IPackageSettings
    {
        private readonly IElement _element;
        public const string SpecializationType = "Package Settings";

        public PackageSettings(IElement element)
        {
            if (element?.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            _element = element;

            MenuOptions = element.ChildElements.Any(x => x.SpecializationType == Api.ContextMenu.SpecializationType)
                ? new ContextMenu(element.ChildElements.Single(x => x.SpecializationType == Api.ContextMenu.SpecializationType))
                : null;
        }

        public static IPackageSettings Create(IElement element)
        {
            return element != null ? new PackageSettings(element) : null;
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name => _element.Name;
        public IContextMenu MenuOptions { get; }

        protected bool Equals(PackageSettings other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PackageSettings)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }


    }
}