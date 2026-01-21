using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Metadata.ApiGateway.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ApiGatewayRouteModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
    {
        public const string SpecializationType = "Api Gateway Route";
        public const string SpecializationTypeId = "b09d7684-5dde-4d4b-9cb5-0707bfd8578f";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public ApiGatewayRouteModel(IElement element, string requiredType = SpecializationTypeId)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Folder = _element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId ? new FolderModel(_element.ParentElement) : null;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public FolderModel Folder { get; }

        public IElement InternalElement => _element;

        [IntentIgnore]
        public UpstreamRouteInfo GetUpstreamRouteInfo()
        {
            var upstreamRoute = this.GetStereotype("Http Settings")?.GetProperty("Route")?.Value;
            var upstreamVerb = this.GetStereotype("Http Settings")?.GetProperty("Verb")?.Value;
            var upstreamVerbEnum = !string.IsNullOrWhiteSpace(upstreamVerb) ? (HttpVerb?)Enum.Parse(typeof(HttpVerb), upstreamVerb, true) : null;

            var downstreamOperation = this.DownstreamEndpoints().FirstOrDefault()?.Element as IElement;
            if (string.IsNullOrWhiteSpace(upstreamRoute))
            {
                var downstreamRoute = downstreamOperation?.GetStereotype("Http Settings")?.GetProperty("Route")?.Value;
                var downstreamServiceRoute = downstreamOperation?.ParentElement?.GetStereotype("Http Service Settings")?.GetProperty("Route")?.Value;
                var separator = string.IsNullOrWhiteSpace(downstreamServiceRoute) || downstreamServiceRoute.EndsWith("/")
                    ? string.Empty
                    : !string.IsNullOrWhiteSpace(downstreamRoute)
                        ? "/"
                        : string.Empty;
                upstreamRoute = $"{downstreamServiceRoute}{separator}{downstreamRoute}";
            }

            if (!upstreamVerbEnum.HasValue)
            {
                var downstreamVerb = downstreamOperation?.GetStereotype("Http Settings")?.GetProperty("Verb")?.Value;
                var downstreamVerbEnum = !string.IsNullOrWhiteSpace(downstreamVerb) ? (HttpVerb?)Enum.Parse(typeof(HttpVerb), downstreamVerb, true) : null;
                upstreamVerbEnum = downstreamVerbEnum;
            }

            return new UpstreamRouteInfo(
                Route: upstreamRoute,
                Verb: upstreamVerbEnum);
        }

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ApiGatewayRouteModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ApiGatewayRouteModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentIgnore]
    public record UpstreamRouteInfo(string? Route, HttpVerb? Verb);

    [IntentIgnore]
    public enum HttpVerb
    {
        Get,
        Post,
        Put,
        Patch,
        Delete,
    }

    [IntentManaged(Mode.Fully)]
    public static class ApiGatewayRouteModelExtensions
    {

        public static bool IsApiGatewayRouteModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ApiGatewayRouteModel.SpecializationTypeId;
        }

        public static ApiGatewayRouteModel AsApiGatewayRouteModel(this ICanBeReferencedType type)
        {
            return type.IsApiGatewayRouteModel() ? new ApiGatewayRouteModel((IElement)type) : null;
        }
    }
}