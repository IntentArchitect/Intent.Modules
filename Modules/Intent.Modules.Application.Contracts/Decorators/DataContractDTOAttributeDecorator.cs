using Intent.MetaModel.DTO;
using Intent.Modules.Application.Contracts.Templates.DTO;

namespace Intent.Modules.Application.Contracts.Decorators
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
