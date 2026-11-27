using Accelerators.Application.Common.Validation;
using Accelerators.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Accelerators.Api.Controllers
{
    [ApiController]
    public class Block2Level1sController : ControllerBase
    {
        private readonly IBlock2Level1sService _appService;

        public Block2Level1sController(IBlock2Level1sService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        /// <summary>
        /// Regression anchor: a GET with no return type. It must model without a validation error, and the endpoint answers 204 No Content.
        /// </summary>
        /// <response code="200">Returns the specified .</response>
        [HttpGet("ping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Ping(CancellationToken cancellationToken = default)
        {
            await _appService.Ping(cancellationToken);
            return NoContent();
        }
    }
}