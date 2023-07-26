using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Types.ServiceProxies.Api
{
    public class ServiceProxyDTOModel : DTOModel, IMetadataModel, IHasStereotypes, IHasName
    {
        public new const string SpecializationType = "Service Proxy DTO";
        public new const string SpecializationTypeId = "ba46b928-b0f4-4672-a520-d6ae1cfe077a";

        public ServiceProxyDTOModel(IElement element, ServiceProxyModel serviceProxy) : base(element, SpecializationType)
        {
            ServiceProxy = serviceProxy;
        }

        public ServiceProxyModel ServiceProxy { get; }

        public bool Equals(ServiceProxyDTOModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ServiceProxyDTOModel)obj);
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