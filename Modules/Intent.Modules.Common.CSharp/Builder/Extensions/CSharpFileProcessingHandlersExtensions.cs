using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public static class CSharpFileProcessingHandlersExtensions
{
    /// <summary>
    /// Registers this <paramref name="method"/> and <paramref name="model"/> as a processing handler.
    /// This method can then be rediscovered from the <see cref="CSharpFile"/> through the <see cref="GetProcessingHandlers"/> extension method.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="model"></param>
    public static void RegisterAsProcessingHandlerForModel(this CSharpClassMethod method, IProcessingHandlerModel model)
    {
        if (!method.File.TryGetMetadata("processing-handlers", out ICollection<ICSharpProcessingHandler> handlers))
        {
            handlers = new List<ICSharpProcessingHandler>();
            method.File.AddMetadata("processing-handlers", handlers);
        }
        handlers.Add(new CSharpProcessingHandler(model, method));
    }

    /// <summary>
    /// Returns any registered <see cref="ICSharpProcessingHandler"/>s that were registered with the
    /// <see cref="RegisterAsProcessingHandlerForModel"/> method. This can be used to generically identify
    /// processing handlers so that interaction code can be injected without creating explicit dependencies
    /// on the concrete types that represent these processing handlers.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static ICollection<ICSharpProcessingHandler> GetProcessingHandlers(this CSharpFile file)
    {
        return file.TryGetMetadata("processing-handlers", out ICollection<ICSharpProcessingHandler> handlers)
            ? handlers
            : new List<ICSharpProcessingHandler>();
    }
}

public interface ICSharpProcessingHandler
{
    IProcessingHandlerModel Model { get; }
    CSharpClassMethod Method { get; }
}

internal class CSharpProcessingHandler : ICSharpProcessingHandler
{
    public CSharpProcessingHandler(IProcessingHandlerModel model, CSharpClassMethod method)
    {
        Model = model;
        Method = method;
    }

    public IProcessingHandlerModel Model { get; }
    public CSharpClassMethod Method { get; }
}