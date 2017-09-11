using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.DTO;
using Intent.Packages.Application.Contracts.Templates.DTO;

namespace Intent.Packages.Application.Contracts.Decorators
{
    public class DataContractDTOAttributeDecorator : IDTOAttributeDecorator
    {
        public const string Id = "Intent.Application.Contracts.DataContractDecorator";

        public string ClasssAttributes(DTOModel dto)
        {
            return "[DataContract]";
        }

        public string PropertyAttributes(DTOModel dto, IDTOField field)
        {
            return "[DataMember]";
        }
    }
}
