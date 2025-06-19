using Intent.Modules.Common.CSharp.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Common.CSharp.Mapping.Resolvers
{
    public class TypeConvertingMappingResolver : IMappingTypeResolver
    {
        private readonly ICSharpTemplate _template;

        public TypeConvertingMappingResolver(ICSharpTemplate template)
        {
            _template = template;
        }

        public ICSharpMapping ResolveMappings(MappingModel mappingModel)
        {
            if (mappingModel.Model.TypeReference?.Element?.IsTypeDefinitionModel() == true
                || mappingModel.Model.TypeReference?.Element?.IsEnumModel() == true)
            {
                return new TypeConvertingCSharpMapping(mappingModel, _template);
            }

            return null;
        }
    }
}
