#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.Interactions;

// ReSharper disable once InconsistentNaming
public class ICSharpClassMethodDeclarationBuilder
{
    private readonly ICSharpClassMethodDeclaration _method;
    private readonly List<string> _phaseOrder;
    private readonly Dictionary<string, List<CSharpStatement>> _phaseStatements;

    public ICSharpClassMethodDeclarationBuilder(ICSharpClassMethodDeclaration method, IEnumerable<string> phaseOrder)
    {
        _method = method;
        _phaseOrder = phaseOrder.ToList();
        _phaseStatements = _phaseOrder.ToDictionary(phase => phase, _ => new List<CSharpStatement>());
    }

    public void AddStatement(string phase, CSharpStatement statement)
    {
        EnsurePhaseExists(phase);
        _phaseStatements[phase].Add(statement);
        RebuildStatements();
    }

    public void AddStatements(string phase, IEnumerable<CSharpStatement> statements)
    {
        EnsurePhaseExists(phase);
        _phaseStatements[phase].AddRange(statements);
        RebuildStatements();
    }

    private void EnsurePhaseExists(string phase)
    {
        if (!_phaseStatements.ContainsKey(phase))
            throw new ArgumentException($"Phase '{phase}' is not defined in the builder's phase order.");
    }

    private void RebuildStatements()
    {
        _method.Statements.Clear();

        foreach (var phase in _phaseOrder)
        {
            if (!_phaseStatements.TryGetValue(phase, out var statements))
            {
                continue;
            }

            _method.Statements.Clear();
            _method.AddStatements(statements);
        }
    }
}