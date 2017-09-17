using Intent.MetaModel.DTO;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Application.Contracts.Templates.DTO
{
    public interface IDTOAttributeDecorator : ITemplateDecorator
    {
        string ClasssAttributes(DTOModel dto);
        string PropertyAttributes(DTOModel dto, IDTOField field );
    }
}
