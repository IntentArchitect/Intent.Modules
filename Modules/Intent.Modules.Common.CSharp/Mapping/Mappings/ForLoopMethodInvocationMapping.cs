using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public class ForLoopMethodInvocationMapping : MethodInvocationMapping
{
    public ForLoopMethodInvocationMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
    }

    public ForLoopMethodInvocationMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
    }

    public override IEnumerable<CSharpStatement> GetMappingStatements()
    {
        var variableName = GetFromPath().Last().Name.Singularize().ToLocalVariableName();
        var forLoop = new CSharpStatementBlock($"foreach(var {variableName} in {GetFromPathText()})");
        SetFromReplacement(GetFromPath().Last().Element, variableName);
        forLoop.AddStatements(base.GetMappingStatements());

        yield return forLoop;
    }
}