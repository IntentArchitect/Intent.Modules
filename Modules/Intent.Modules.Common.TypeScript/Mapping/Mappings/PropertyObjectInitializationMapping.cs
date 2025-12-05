#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Typescript.Mapping;
using Intent.Modules.Common.TypeScript.Builder;
using Intent.Modules.Common.TypeScript.Templates;

namespace Intent.Modules.Common.Angular.Mapping
{
    /// <inheritdoc />
    public class PropertyObjectInitializationMapping : ObjectInitializationMapping
    {
        private readonly MappingModel _mappingModel;
        private readonly ITypescriptTemplate _template;

        /// <inheritdoc />
        public PropertyObjectInitializationMapping(MappingModel model, ITypescriptTemplate template) : base(model, template)
        {
            _mappingModel = model;
            _template = template;
        }

        public override IEnumerable<TypescriptStatement> GetMappingStatements()
        {
            // TODO
            yield return new TypescriptStatement($"{GetTargetStatement()}: {GetSourceStatement()}");
        }
    }
}