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

public static class CSharpClassMethodInteractionExtensions
{
    public static CSharpClassMappingManager GetMappingManager(this CSharpClassMethod method)
    {
        if (!method.TryGetMetadata<CSharpClassMappingManager>("mapping-manager", out var csharpMapping))
        {
            csharpMapping = new CSharpClassMappingManager(method.File.Template);
            csharpMapping.AddMappingResolver(new TypeConvertingMappingResolver(method.File.Template));
            method.AddMetadata("mapping-manager", csharpMapping);
        }

        return csharpMapping;
    }

    public static void ImplementInteractions(this CSharpClassMethod method, IProcessingHandlerModel processingHandlerModel)
    {
        var interactions = new List<IElement>();
        interactions.AddRange(processingHandlerModel.InternalElement.ChildElements);
        foreach (var interaction in processingHandlerModel.InternalElement.AssociatedElements.Where(x => x.IsTargetEnd()).OrderBy(x => x.Order))
        {
            // Math.Min because the order number of associations was being stored incorrectly - it was counting type references. This has been fixed in 4.5
            interactions.Insert(Math.Min(interaction.Order, interactions.Count), interaction);
        }
        // TODO: NB: Checking this trait means that nothing will work unless the user presses save in the designer.
        foreach (var interaction in interactions.Where(x => x.Traits.Any(t => t.Id == "d00a2ab0-9a23-4192-b8bb-166798fc7dba")))
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
