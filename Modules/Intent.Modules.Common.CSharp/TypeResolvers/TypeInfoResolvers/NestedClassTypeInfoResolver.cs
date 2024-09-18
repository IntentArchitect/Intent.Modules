using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.TypeResolvers
{
    public class NestedClassTypeInfoResolver : ITypeInfoResolver
    {
        private readonly ITemplate _template;
        private readonly IMetadataModel _model;
        private readonly string _nestedTypeName;

        public NestedClassTypeInfoResolver(ITemplate template, IMetadataModel model, string nestedTypeName)
        {
            _template = template;
            _model = model;
            _nestedTypeName = nestedTypeName;
        }

        public ITemplate Template => _template;

        public IMetadataModel Model => _model;

        public IResolvedTypeInfo? Resolve()
        {
            if (_template is not IClassProvider classProvider)
            {
                return null;
            }
            if (((IntentTemplateBase)_template).HasTypeResolver())
            {
                var parentClass = ((IntentTemplateBase)_template).Types.Get(classProvider) as CSharpResolvedTypeInfo;

                return CSharpResolvedTypeInfo.Create(
                    name: $"{parentClass.Name}.{_nestedTypeName}",
                    @namespace: parentClass.Namespace,
                    isPrimitive: false,
                    isNullable: false,
                    isCollection: false,
                    typeReference: null,
                    nullableFormatter: null,
                    template: classProvider,
                    genericTypeParameters: null);
            }

            return CSharpResolvedTypeInfo.Create(
                name: $"{classProvider.ClassName}.{_nestedTypeName}",
                @namespace : classProvider.Namespace,
                    isPrimitive: false,
                    isNullable: false,
                    isCollection: false,
                    typeReference: null,
                    nullableFormatter: null,
                    template: classProvider,
                    genericTypeParameters: null);
        }
    }

}
