using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public class DefaultCSharpMapping : CSharpMappingBase
{
    public DefaultCSharpMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
    }
}

public static class ElementToElementMappedEndExtensions
{
    public static bool IsOneToOne(this IElementToElementMappedEnd model)
    {
        return model.Sources.Count() == 1 && model.MappingExpression.Trim() == $"{{{model.Sources.Single().ExpressionIdentifier}}}";
    }
}