using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeScript.Templates;

namespace Intent.Modules.Common.Typescript.Mapping;

public class DefaultTypescriptMapping : TypescriptMappingBase
{
    public DefaultTypescriptMapping(MappingModel model, ITypescriptTemplate template) : base(model, template)
    {
    }
}

public static class ElementToElementMappedEndExtensions
{
    public static bool IsOneToOne(this IElementToElementMappedEnd model)
    {
        return model?.Sources.Count() == 1 && model.MappingExpression.Trim() == $"{{{model.Sources.Single().ExpressionIdentifier}}}";
    }
}