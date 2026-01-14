using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.UnhandledExceptionBehaviour", Version = "1.0")]

namespace Accelerators.Application.Common.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> _logger;
        private readonly bool _logRequestPayload;

        public UnhandledExceptionBehaviour(ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                return await next(cancellationToken);
            }
            catch (ValidationException)
            {
                // Do not log Fluent Validation Exceptions
                throw;
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;

                if (_logRequestPayload)
                {
                    _logger.LogError(ex, "Accelerators Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);
                }
                else
                {
                    _logger.LogError(ex, "Accelerators Request: Unhandled Exception for Request {Name}", requestName);
                }
                throw;
            }
        }
    }
}