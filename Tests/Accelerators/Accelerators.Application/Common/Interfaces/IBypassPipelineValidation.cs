using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.BypassPipelineValidationInterface", Version = "1.0")]

namespace Accelerators.Application.Common.Interfaces
{
    /// <summary>
    /// Defines a marker interface that, when implemented by a request, instructs the 
    /// <c>ValidationBehaviour&lt;TRequest, TResponse&gt;</c> to skip the execution 
    /// of all registered validators.
    /// </summary>
    /// <remarks>
    /// Use this interface for specialized requests where standard pipeline validation 
    /// is redundant or must be deferred to a later stage of processing.
    /// </remarks>
    public interface IBypassPipelineValidation
    {
    }
}