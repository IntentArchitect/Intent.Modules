using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.TypeScript.Templates;

namespace Intent.Modules.Common.Typescript.Mapping.Resolvers
{
    public class TypeConvertingMappingResolver : IMappingTypeResolver
    {
        private readonly ITypescriptTemplate _template;

        public TypeConvertingMappingResolver(ITypescriptTemplate template)
        {
            _template = template;
        }

        public ITypescriptMapping ResolveMappings(MappingModel mappingModel)
        {
            if (mappingModel.Model.TypeReference?.Element?.IsTypeDefinitionModel() == true
                || mappingModel.Model.TypeReference?.Element?.IsEnumModel() == true)
            {
                return new TypeConvertingTypescriptMapping(mappingModel, _template);
            }

            return null;
        }
    }
}
