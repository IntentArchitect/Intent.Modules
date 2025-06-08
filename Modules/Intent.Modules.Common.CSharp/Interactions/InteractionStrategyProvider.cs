using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.IArchitect.Common.Publishing;
using Intent.Modules.Common.CSharp.Mapping.Resolvers;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using Intent.Utils;
using Serilog.Core;

namespace Intent.Modules.Common.CSharp.Interactions;
public interface IInteractionStrategy
{
    bool IsMatch(IElement interaction);
    void ImplementInteraction(CSharpClassMethod method, IElement interaction);
}

public class InteractionStrategyProvider
{
    public static InteractionStrategyProvider Instance = new();

    private readonly IList<IInteractionStrategy> _strategies = [];
    public void Register(IInteractionStrategy strategy)
    {
        _strategies.Add(strategy);
    }

    public bool HasInteractionStrategy(IElement interaction)
    {
        var matched = _strategies.Where(x => x.IsMatch(interaction)).ToList();

        if (matched.Count > 1)
        {
            Logging.Log.Warning($"Multiple interaction strategies matched for {interaction}: [{string.Join(", ", matched)}]");
        }

        return matched.Any();
    }

    public IInteractionStrategy? GetInteractionStrategy(IElement interaction)
    {
        var matched = _strategies.Where(x => x.IsMatch(interaction)).ToList();
        if (matched.Count == 0)
        {
            Logging.Log.Warning($"No interaction strategy matched for {interaction}");
            return null;
        }

        if (matched.Count > 1)
        {
            Logging.Log.Warning($"Multiple interaction strategies found for {interaction}: [{string.Join(", ", matched)}]");
        }

        return matched.First();
    }
}


public class CSharpClassMethodBuilder
{
    private readonly CSharpClassMethod _method;
    private readonly List<string> _phaseOrder;
    private readonly Dictionary<string, List<CSharpStatement>> _phaseStatements;

    public CSharpClassMethodBuilder(CSharpClassMethod method, IEnumerable<string> phaseOrder)
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
            if (_phaseStatements.TryGetValue(phase, out var statements))
            {
                _method.Statements.Clear();
                _method.AddStatements(statements);
            }
        }
    }
}

public class ExecutionPhases
{
    public const string Validation = "Validation";
    public const string Retrieval = "Retrieval";
    public const string BusinessLogic = "BusinessLogic";
    public const string Persistence = "Persistence";
    public const string IntegrationEvents = "IntegrationEvents";
    public const string Response = "Response";

    public static List<string> ExecutionPhaseOrder =
    [
        ExecutionPhases.Validation,
        ExecutionPhases.Retrieval,
        ExecutionPhases.BusinessLogic,
        ExecutionPhases.Persistence,
        ExecutionPhases.IntegrationEvents,
        ExecutionPhases.Response
    ];
}

public static class CSharpClassMethodInteractionExtensions
{
    private const string ExecutionPhasesKey = "execution-phases";
    private const string ExecutionPhaseKey = "execution-phase";


    public static void SetExecutionPhases(this CSharpClassMethod method, string[] phaseOrder)
    {
        if (method.HasMetadata(ExecutionPhasesKey))
        {
            method.Metadata.Remove(ExecutionPhasesKey);
        }
        method.AddMetadata(ExecutionPhasesKey, phaseOrder);
    }

    public static void AddStatement(this CSharpClassMethod method, string phase, CSharpStatement statement)
    {
        //if (!method.TryGetMetadata<string[]>(ExecutionPhasesKey, out var phases) || !phases.Contains(phase))
        //{
        //    throw new Exception("Phase " + phases + " has not been defined. Call SetExecutionPhases beforehand");
        //}

        statement.AddMetadata(ExecutionPhaseKey, phase);
        method.InsertStatement(FindInsertionIndex(method, phase), statement);
    }

    

    public static void AddStatements(this CSharpClassMethod method, string phase, IEnumerable<CSharpStatement> statements)
    {
        //if (!method.TryGetMetadata<string[]>(ExecutionPhasesKey, out var phases) || !phases.Contains(phase))
        //{
        //    throw new Exception("Phase " + phases + " has not been defined. Call SetExecutionPhases beforehand");
        //}

        foreach (var statement in statements)
        {
            statement.AddMetadata(ExecutionPhaseKey, phase);
            method.InsertStatement(FindInsertionIndex(method, phase), statement);
        }
    }

    //public static void ReorderStatments(this CSharpClassMethod method)
    //{
    //    var phases = method.GetMetadata<string[]>(ExecutionPhasesKey);
    //    var statements = method.Statements.ToList();
    //    method.Statements.Clear();
    //    method.AddStatements(statements.Where(x => !x.HasMetadata(ExecutionPhaseKey)));
    //    foreach (var phase in phases)
    //    {
    //        var phaseStatements = statements.Where(x => x.TryGetMetadata<string>(ExecutionPhaseKey, out var setPhase) && setPhase == phase);
    //        method.AddStatements(phaseStatements);
    //    }
    //}

    public static IEnumerable<CSharpStatement> GetStatementsInPhase(this CSharpClassMethod method, string phase)
    {
        var statementPhase = method.Statements.Where(s => (s.TryGetMetadata<string>(ExecutionPhaseKey, out var x) ? x : ExecutionPhases.BusinessLogic) == phase).ToList();
        return statementPhase;
    }

    private static int FindInsertionIndex(this CSharpClassMethod method, string phase)
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

    public static CSharpClassMappingManager GetMappingManager(this CSharpClassMethod method)
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

    public static void ImplementInteractions(this CSharpClassMethod method, IProcessingHandlerModel processingHandlerModel)
    {
        ImplementInteractions(method, processingHandlerModel.GetInteractions());
    }

    public static void ImplementInteractions(this CSharpClassMethod method, IEnumerable<IElement> interactions)
    {
        foreach (var interaction in interactions)
        {
            ImplementInteraction(method, interaction);
        }
    }

    public static void ImplementInteraction(this CSharpClassMethod method, IElement processionAction)
    {
        var strategy = InteractionStrategyProvider.Instance.GetInteractionStrategy(processionAction);
        if (strategy is not null)
        {
            strategy.ImplementInteraction(method, processionAction);
        }
    }

    public static Dictionary<string, EntityDetails> TrackedEntities(this CSharpClassMethod method)
    {
        if (!method.TryGetMetadata<Dictionary<string, EntityDetails>>("tracked-entities", out var trackedEntities))
        {
            trackedEntities = new Dictionary<string, EntityDetails>();
            if (method.HasMetadata("tracked-entities"))
            {
                method.Metadata.Remove("tracked-entities");
            }
            method.AddMetadata("tracked-entities", trackedEntities);
        }

        return trackedEntities;
    }
}

public record EntityDetails(IElement ElementModel, string VariableName, IDataAccessProvider DataAccessProvider, bool IsNew, string ProjectedType = null, bool IsCollection = false);

public interface IDataAccessProvider
{
    bool IsUsingProjections { get; }
    CSharpStatement SaveChangesAsync();
    CSharpStatement AddEntity(string entityName);
    CSharpStatement Update(string entityName);
    CSharpStatement Remove(string entityName);
    CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps);
    CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps);
    CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements);
    CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, bool orderByIsNUllable, out IList<CSharpStatement> prerequisiteStatements);
    CSharpStatement FindAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements);
}

public record PrimaryKeyFilterMapping(CSharpStatement ValueExpression, CSharpStatement Property, IElementToElementMappedEnd Mapping);

public static class CSharpClassExtensions
{
    public static string InjectService(this CSharpClass handlerClass, string interfaceName, string parameterName = null)
    {
        var fieldName = default(string);

        var ctor = handlerClass.Constructors.First();
        if (ctor.Parameters.All(x => x.Type != interfaceName))
        {
            ctor.AddParameter(interfaceName, (parameterName ?? interfaceName.RemovePrefix("I")).ToParameterName(),
                param => param.IntroduceReadonlyField(field => fieldName = field.Name));
        }
        else
        {
            fieldName = ctor.Parameters.First(x => x.Type == interfaceName).Name.ToPrivateMemberName();
        }
        return fieldName;
    }

}
