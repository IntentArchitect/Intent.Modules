using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Modelers.Services.ProxyInteractions.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ServiceReferencesValidationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Modelers.Services.ProxyInteractions.ServiceReferencesValidationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;


        public const string CallServiceOperationSpecTypeId = "3e69085c-fa2f-44bd-93eb-41075fd472f8";
        public const string ServiceProxySpecTypeId = "07d8d1a9-6b9f-4676-b7d3-8db06299e35c";
        
        protected override void OnAfterMetadataLoad(IApplication application)
        {
            var proxyServiceCalls = application.MetadataManager
                .Services(application.Id)
                .Associations
                .Where(IsProxyServiceCall)
                .ToArray();

            foreach (var proxyServiceCall in proxyServiceCalls)
            {
                foreach (var mapping in proxyServiceCall.TargetEnd.Mappings)
                {
                    foreach (var mappedEnd in mapping.MappedEnds)
                    {
                        if (mappedEnd.TargetElement is not null)
                        {
                            continue;
                        }

                        var operation = proxyServiceCall.TargetEnd.TypeReference.Element.Name;
                        var targetPath = string.Join(".", mappedEnd.TargetPath.Select(s => s.Name));
                        var packageName = mapping.HostElement.Package.Name;
                        var message = $"The proxy operation '{operation}' is bound to '{targetPath}', which seems to connect to an element from another package. Add the required package reference to '{packageName}'.";
                        throw new ElementException(proxyServiceCall.TargetEnd, message);
                    }
                }
            }

            return;
            static bool IsProxyServiceCall(IAssociation association)
            {
                return association.SpecializationTypeId == CallServiceOperationSpecTypeId &&
                       ((IElement)association.TargetEnd?.TypeReference?.Element)?.ParentElement?.SpecializationTypeId == ServiceProxySpecTypeId;
            }
        }
    }
}