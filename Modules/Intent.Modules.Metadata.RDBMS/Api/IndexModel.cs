using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{
    [IntentManaged(Mode.Merge)]
    public class IndexModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Index";
        public const string SpecializationTypeId = "436e3afe-b4ef-481c-b803-0d1e7d263561";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public IndexModel(IElement element, string requiredType = SpecializationType)
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

        public bool IsMapped => _element.IsMapped;

        public IElementMapping Mapping => _element.MappedElement;

        public IElement InternalElement => _element;

        public IList<IndexColumnModel> Columns => _element.ChildElements
            .GetElementsOfType(IndexColumnModel.SpecializationTypeId)
            .Select(x => new IndexColumnModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(IndexModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IndexModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class IndexModelExtensions
    {

        public static bool IsIndexModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == IndexModel.SpecializationTypeId;
        }

        public static IndexModel AsIndexModel(this ICanBeReferencedType type)
        {
            return type.IsIndexModel() ? new IndexModel((IElement)type) : null;
        }

        public static bool HasSelectColumnsMapping(this IndexModel type)
        {
            return type.Mapping?.MappingSettingsId == "30f4278f-1d74-4e7e-bfdb-39c8e120f24c";
        }

        public static IElementMapping GetSelectColumnsMapping(this IndexModel type)
        {
            return type.HasSelectColumnsMapping() ? type.Mapping : null;
        }
    }
}