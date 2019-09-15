using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class Modeler : IModeler, IEquatable<Modeler>
    {
        private readonly IElement _element;
        public const string SpecializationType = "Modeler";

        public Modeler(IElement element)
        {
            if (element.SpecializationType != SpecializationType)
            {
                throw new InvalidOperationException($"Cannot load {nameof(Modeler)} from element of type {element.SpecializationType}");
            }

            _element = element;
            Folder = Api.Folder.SpecializationType.Equals(_element.ParentElement?.SpecializationType, StringComparison.OrdinalIgnoreCase) ? new Folder(_element.ParentElement) : null;
            ModelTypes = _element.Literals.Select(x => new ModelerModelType(x)).ToList();
        }

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public IFolder Folder { get; }
        public string Id => _element.Id;
        public string Name => _element.Name;
        public IEnumerable<IModelerModelType> ModelTypes { get; }
        public string ModuleDependency => _element.GetStereotypeProperty<string>("Modeler", "Module Dependency");
        public string ModuleVersion => _element.GetStereotypeProperty<string>("Modeler", "Module Version");

        public bool Equals(Modeler other)
        {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Modeler) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}