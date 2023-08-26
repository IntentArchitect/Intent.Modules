using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.DomainInteractions.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DiagramModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Diagram";
        public const string SpecializationTypeId = "acd76ada-3ea6-4011-89f1-4320605dc78a";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public DiagramModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(DiagramModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiagramModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DiagramModelExtensions
    {

        public static bool IsDiagramModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DiagramModel.SpecializationTypeId;
        }

        public static DiagramModel AsDiagramModel(this ICanBeReferencedType type)
        {
            return type.IsDiagramModel() ? new DiagramModel((IElement)type) : null;
        }
    }
}