using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Types.ServiceProxies.Api
{
    public class ServiceProxyEnumModel : EnumModel, IMetadataModel, IHasStereotypes, IHasName
    {
        public new const string SpecializationType = "Service Proxy Enum";
        public new const string SpecializationTypeId = "54ddf4bc-73f5-4c8f-9f29-cf0175b236d4";

        public ServiceProxyEnumModel(IElement element, ServiceProxyModel serviceProxy) : base(element)
        {
            ServiceProxy = serviceProxy;
        }

        public ServiceProxyModel ServiceProxy { get; }

        public bool Equals(ServiceProxyEnumModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ServiceProxyEnumModel)obj);
        }

        public override int GetHashCode()
        {
            return _element != null ? _element.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return _element.ToString();
        }
    }
}