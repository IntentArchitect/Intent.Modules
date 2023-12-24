using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge)]
    public partial class FactoryExtensionModel : IMetadataModel, IHasStereotypes, IHasName, IHasFolder, IElementWrapper
    {
        public const string SpecializationType = "Factory Extension";
        public const string SpecializationTypeId = "7d008e84-bb28-4b10-ba28-7439202fca76";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public FactoryExtensionModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public FolderModel Folder { get; }

        public IElement InternalElement => _element;

        [IntentManaged(Mode.Ignore)]
        public IntentModuleModel GetModule()
        {
            return new IntentModuleModel(_element.Package);
        }

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(FactoryExtensionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FactoryExtensionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public string Comment => _element.Comment;
    }

    [IntentManaged(Mode.Fully)]
    public static class FactoryExtensionModelExtensions
    {

        public static bool IsFactoryExtensionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == FactoryExtensionModel.SpecializationTypeId;
        }

        public static FactoryExtensionModel AsFactoryExtensionModel(this ICanBeReferencedType type)
        {
            return type.IsFactoryExtensionModel() ? new FactoryExtensionModel((IElement)type) : null;
        }
    }
}