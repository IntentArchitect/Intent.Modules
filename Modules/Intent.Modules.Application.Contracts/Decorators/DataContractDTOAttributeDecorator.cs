using System;
using System.Collections.Generic;
using Intent.MetaModel.DTO;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Application.Contracts.Decorators
{
    public class DataContractDTOAttributeDecorator : IDTOAttributeDecorator, IDeclareUsings
    {
        public const string Id = "Intent.Application.Contracts.DataContractDecorator";

        public string ClasssAttributes(IDTOModel dto)
        {
            return "[DataContract]";
        }

        public string PropertyAttributes(IDTOModel dto, IDTOField field)
        {
            return "[DataMember]";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "System.Runtime.Serialization";
        }
    }
}
