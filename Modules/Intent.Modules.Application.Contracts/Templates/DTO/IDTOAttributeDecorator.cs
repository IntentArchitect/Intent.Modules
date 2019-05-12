using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.Templates.DTO
{
    public interface IDTOAttributeDecorator : ITemplateDecorator
    {
        string ClassAttributes(IDTOModel dto);
        string PropertyAttributes(IDTOModel dto, IAttribute field );
    }
}
