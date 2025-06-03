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
using Intent.Modules.Common.Templates;
using Intent.Templates;
using Intent.Utils;
using Serilog.Core;

namespace Intent.Modules.Common.CSharp.Interactions;
public interface IInteractionStrategy
{
    bool IsMatch(IAssociationEnd interaction);
    void AddInteractionStatements(CSharpClassMethod method, IAssociationEnd interaction);
}

public class InteractionStrategyProvider
{
    public static InteractionStrategyProvider Instance = new();

    private readonly IList<IInteractionStrategy> _strategies = [];
    public void Register(IInteractionStrategy strategy)
    {
        _strategies.Add(strategy);
    }

    public IInteractionStrategy? GetInteractionStrategy(IAssociationEnd interaction)
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

    public static void AddInteractionStatements(this CSharpClassMethod method, IProcessingHandlerModel processingHandlerModel)
    {
        var result = new List<CSharpStatement>();
        foreach (var association in processingHandlerModel.InternalElement.AssociatedElements.Where(x => x.Traits.Any(t => t.Id == "d00a2ab0-9a23-4192-b8bb-166798fc7dba")))
        {
            result.AddRange(CreateInteractionStatements(method, association));
        }

        return result;
    }

    public static void CreateInteractionStatements(this CSharpClassMethod method, IAssociationEnd callServiceOperation)
    {
        var strategy = InteractionStrategyProvider.Instance.GetInteractionStrategy(callServiceOperation);
        if (strategy is not null)
        {
            return strategy.AddInteractionStatements(method, callServiceOperation);
        }

        return [];
    }
}

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

    public static CSharpClassMappingManager GetMappingManager(this CSharpClassMethod method)
    {
        if (!method.TryGetMetadata<CSharpClassMappingManager>("mapping-manager", out var csharpMapping))
        {
            csharpMapping = new CSharpClassMappingManager(method.File.Template);
            method.AddMetadata("mapping-manager", csharpMapping);
        }

        return csharpMapping;
    }
}
