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

namespace Intent.Modules.Common.CSharp.Interactions;
public interface IInteractionStrategy
{
    bool IsMatch(IAssociationEnd interaction);
    IEnumerable<CSharpStatement> GetStatements(CSharpClass handlerClass, IAssociationEnd interaction, CSharpClassMappingManager csharpMapping);
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
        return _strategies.FirstOrDefault(x => x.IsMatch(interaction));
    }
}

public class StandardInteractions
{
    private readonly ICSharpFileBuilderTemplate _template;
    private readonly CSharpClassMappingManager _csharpMapping;

    public StandardInteractions(ICSharpFileBuilderTemplate template, CSharpClassMappingManager csharpMapping)
    {
        _template = template;
        _csharpMapping = csharpMapping;
    }

    public IEnumerable<CSharpStatement> GetStatements(CSharpClass handlerClass, IProcessingHandlerModel processingHandlerModel)
    {
        var result = new List<CSharpStatement>();
        foreach (var association in processingHandlerModel.InternalElement.AssociatedElements)
        {
            result.AddRange(GetStatements(handlerClass, association));
        }

        return result;
    }

    public IEnumerable<CSharpStatement> GetStatements(CSharpClass handlerClass, IAssociationEnd callServiceOperation)
    {
        var strategy = InteractionStrategyProvider.Instance.GetInteractionStrategy(callServiceOperation);
        if (strategy is not null)
        {
            return strategy.GetStatements(handlerClass, callServiceOperation, _csharpMapping);
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
        }

        return csharpMapping;
    }
}
