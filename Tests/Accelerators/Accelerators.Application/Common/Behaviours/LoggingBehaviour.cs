using Accelerators.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.LoggingBehaviour", Version = "1.0")]

namespace Accelerators.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehaviour<TRequest>> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly bool _logRequestPayload;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest>> logger,
            ICurrentUserService currentUserService,
            IConfiguration configuration)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _logRequestPayload = configuration.GetValue<bool?>("CqrsSettings:LogRequestPayload") ?? false;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var user = await _currentUserService.GetAsync();

            if (_logRequestPayload)
            {
                _logger.LogInformation("Accelerators Request: {Name} {@UserId} {@UserName} {@Request}", requestName, user?.Id, user?.Name, request);
            }
            else
            {
                _logger.LogInformation("Accelerators Request: {Name} {@UserId} {@UserName}", requestName, user?.Id, user?.Name);
            }
        }
    }
}