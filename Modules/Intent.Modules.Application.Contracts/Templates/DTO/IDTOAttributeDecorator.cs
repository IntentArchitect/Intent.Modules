using Intent.MetaModel.DTO;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Packages.Application.Contracts.Templates.DTO
{
    public interface IDTOAttributeDecorator : ITemplateDecorator
    {
        string ClasssAttributes(DTOModel dto);
        string PropertyAttributes(DTOModel dto, IDTOField field );
    }
}
