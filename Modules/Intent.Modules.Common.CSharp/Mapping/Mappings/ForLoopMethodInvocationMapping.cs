using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public class ForLoopMethodInvocationMapping : MethodInvocationMapping
{
    public ForLoopMethodInvocationMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
        if (mapping == null)
        {
            throw new ArgumentException("An explicit mapping is required", nameof(mapping));
        }
    }

    public ForLoopMethodInvocationMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
    }

    public override IEnumerable<CSharpStatement> GetMappingStatements()
    {
        var variableName = GetSourcePath().Last().Name.Singularize().ToLocalVariableName();
        var forLoop = new CSharpStatementBlock($"foreach(var {variableName} in {GetSourcePathText()})");
        SetSourceReplacement(GetSourcePath().Last().Element, variableName);
        forLoop.AddStatements(base.GetMappingStatements());

        yield return forLoop;
    }
}