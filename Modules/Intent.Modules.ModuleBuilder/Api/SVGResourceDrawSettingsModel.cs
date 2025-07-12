using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class SVGResourceDrawSettingsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "SVG Resource Draw Settings";
        public const string SpecializationTypeId = "b5566ace-3353-4009-9370-475b561451a0";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public SVGResourceDrawSettingsModel(IElement element, string requiredType = SpecializationTypeId)
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

        [IntentManaged(Mode.Ignore)]
        public SvgResourceDrawSettings ToPersistable()
        {
            return new SvgResourceDrawSettings
            {
                PositionSettings = new PositionSettings()
                {
                    X = this.GetPositionSettings().X(),
                    Y = this.GetPositionSettings().Y()
                },
                SizeSettings = new SizeSettings()
                {
                    Width = this.GetPositionSettings().Width(),
                    Height = this.GetPositionSettings().Height()
                },
                ConditionFunction = this.GetSVGResourceSettings().Condition(),
                ResourcePath = this.GetSVGResourceSettings().ResourcePath(),
            };
        }

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(SVGResourceDrawSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SVGResourceDrawSettingsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class SVGResourceDrawSettingsModelExtensions
    {

        public static bool IsSVGResourceDrawSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == SVGResourceDrawSettingsModel.SpecializationTypeId;
        }

        public static SVGResourceDrawSettingsModel AsSVGResourceDrawSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsSVGResourceDrawSettingsModel() ? new SVGResourceDrawSettingsModel((IElement)type) : null;
        }
    }
}