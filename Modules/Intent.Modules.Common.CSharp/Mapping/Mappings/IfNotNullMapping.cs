using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public class IfNotNullMapping : CSharpMappingBase
{
    private readonly ICSharpMapping _innerMapping;

    public IfNotNullMapping(MappingModel model, ICSharpTemplate template, ICSharpMapping? innerMapping = null) : base(model, template)
    {
        _innerMapping = innerMapping ?? new DefaultCSharpMapping(model, template);
        _innerMapping.Parent = this;
        foreach (var child in _innerMapping.Children)
        {
            child.Parent = this;
        }
    }

    public override IEnumerable<CSharpStatement> GetMappingStatements()
    {
        return
        [
            new CSharpIfStatement(GetSourcePathText(GetSourcePath(), true) + " is not null")
                .AddStatements(_innerMapping.GetMappingStatements(), s =>
                {
                    foreach (var statement in s)
                    {
                        statement.WithSemicolon();
                    }
                })
        ];
    }

    public override CSharpStatement GetSourceStatement(bool? targetIsNullable = null)
    {
        return new CSharpNullCoalescingExpression(base.GetSourceStatement(true), base.GetTargetStatement());
    }

    public override bool TryGetSourceReplacement(IMetadataModel type, out string replacement)
    {
        return (_innerMapping as CSharpMappingBase)?.SourceReplacements.TryGetValue(type.Id, out replacement) == true || base.TryGetSourceReplacement(type, out replacement);
    }

    public override bool TryGetTargetReplacement(IMetadataModel type, out string replacement)
    {
        return (_innerMapping as CSharpMappingBase)?.TargetReplacements.TryGetValue(type.Id, out replacement) == true || base.TryGetTargetReplacement(type, out replacement);
    }
}

public class NullCoalesceToExistingValueMapping : CSharpMappingBase
{
    public NullCoalesceToExistingValueMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
    }

    public override CSharpStatement GetSourceStatement(bool? targetIsNullable = null)
    {
        return new CSharpNullCoalescingExpression(base.GetSourceStatement(targetIsNullable), base.GetTargetStatement());
    }
}