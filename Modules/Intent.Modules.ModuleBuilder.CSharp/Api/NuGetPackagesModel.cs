using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.CSharp.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class NuGetPackagesModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "NuGet Packages";
        public const string SpecializationTypeId = "4c75c84d-c745-4f0f-b1f4-82eeca01dfc7";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public NuGetPackagesModel(IElement element, string requiredType = SpecializationTypeId)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
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

        public IList<NuGetPackageModel> NuGetPackages => _element.ChildElements
            .GetElementsOfType(NuGetPackageModel.SpecializationTypeId)
            .Select(x => new NuGetPackageModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(NuGetPackagesModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NuGetPackagesModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class NuGetPackagesModelExtensions
    {

        public static bool IsNuGetPackagesModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == NuGetPackagesModel.SpecializationTypeId;
        }

        public static NuGetPackagesModel AsNuGetPackagesModel(this ICanBeReferencedType type)
        {
            return type.IsNuGetPackagesModel() ? new NuGetPackagesModel((IElement)type) : null;
        }
    }
}