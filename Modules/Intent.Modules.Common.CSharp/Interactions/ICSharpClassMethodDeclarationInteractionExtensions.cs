#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;

namespace Intent.Modules.Common.CSharp.Interactions;

// ReSharper disable once InconsistentNaming
public static class ICSharpClassMethodDeclarationInteractionExtensions
{
    private const string ExecutionPhasesKey = "execution-phases";
    private const string ExecutionPhaseKey = "execution-phase";

    public static void SetExecutionPhases(this ICSharpClassMethodDeclaration method, string[] phaseOrder)
    {
        if (method.HasMetadata(ExecutionPhasesKey))
        {
            method.RemoveMetadata(ExecutionPhasesKey);
        }
        method.AddMetadata(ExecutionPhasesKey, phaseOrder);
    }

    public static void AddStatement(this ICSharpClassMethodDeclaration method, string phase, CSharpStatement statement)
    {
        statement.AddMetadata(ExecutionPhaseKey, phase);
        method.InsertStatement(FindInsertionIndex(method, phase), statement);
    }

    public static void AddStatements(this ICSharpClassMethodDeclaration method, string phase, IEnumerable<CSharpStatement> statements)
    {
        foreach (var statement in statements)
        {
            statement.AddMetadata(ExecutionPhaseKey, phase);
            method.InsertStatement(FindInsertionIndex(method, phase), statement);
        }
    }

    public static IEnumerable<ICSharpStatement> GetStatementsInPhase(this ICSharpClassMethodDeclaration method, string phase)
    {
        var statementPhase = method.Statements.Where(s => (s.TryGetMetadata<string>(ExecutionPhaseKey, out var x) ? x : ExecutionPhases.BusinessLogic) == phase).ToList();
        return statementPhase;
    }

    private static int FindInsertionIndex(this ICSharpClassMethodDeclaration method, string phase)
    {
        var phaseIndex = ExecutionPhases.ExecutionPhaseOrder.IndexOf(phase);

        for (var i = 0; i < method.Statements.Count; i++)
        {
            var statementPhase = method.Statements[i].TryGetMetadata<string>(ExecutionPhaseKey, out var x) ? x : ExecutionPhases.BusinessLogic;
            var currentPhaseIndex = ExecutionPhases.ExecutionPhaseOrder.IndexOf(statementPhase);
            if (currentPhaseIndex > phaseIndex)
            {
                return i;
            }
        }

        return method.Statements.Count; // append to end
    }

    public static CSharpClassMappingManager GetMappingManager(this ICSharpClassMethodDeclaration method)
    {
        if (!method.TryGetMetadata<CSharpClassMappingManager>("mapping-manager", out var csharpMapping))
        {
            csharpMapping = new CSharpClassMappingManager(method.File.Template);
            method.AddMetadata("mapping-manager", csharpMapping);
        }

        return csharpMapping;
    }

    public static IEnumerable<IElement> GetInteractions(this IProcessingHandlerModel processingHandlerModel)
    {
        var interactions = new List<IElement>();
        interactions.AddRange(processingHandlerModel.InternalElement.ChildElements);
        foreach (var interaction in processingHandlerModel.InternalElement.AssociatedElements.Where(x => x.IsTargetEnd()).OrderBy(x => x.Order))
        {
            // Math.Min because the order number of associations was being stored incorrectly - it was counting type references. This has been fixed in 4.5
            interactions.Insert(Math.Min(interaction.Order, interactions.Count), interaction);
        }

        return interactions.Where(InteractionStrategyProvider.Instance.HasInteractionStrategy).ToList();
    }

    public static void ImplementInteractions(this ICSharpClassMethodDeclaration method, IProcessingHandlerModel processingHandlerModel)
    {
        ImplementInteractions(method, processingHandlerModel.GetInteractions());
    }

    public static void ImplementInteractions(this ICSharpClassMethodDeclaration method, IEnumerable<IElement> interactions)
    {
        foreach (var interaction in interactions)
        {
            ImplementInteraction(method, interaction);
        }
    }

    public static void ImplementInteraction(this ICSharpClassMethodDeclaration method, IElement processionAction)
    {
        var strategy = InteractionStrategyProvider.Instance.GetInteractionStrategy(processionAction);
        strategy?.ImplementInteraction(method, processionAction);
    }

    public static Dictionary<string, EntityDetails> TrackedEntities(this ICSharpClassMethodDeclaration method)
    {
        if (!method.TryGetMetadata<Dictionary<string, EntityDetails>>("tracked-entities", out var trackedEntities))
        {
            trackedEntities = new Dictionary<string, EntityDetails>();
            method.AddMetadata("tracked-entities", trackedEntities);
        }

        return trackedEntities;
    }
}