using Accelerators.Application.Block2Level1s;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Accelerators.Application.Interfaces
{
    public interface IBlock2Level1sService
    {
        Task<Guid> CreateBlock2Level1(CreateBlock2Level1Dto dto, CancellationToken cancellationToken = default);
        Task UpdateBlock2Level1(Guid id, UpdateBlock2Level1Dto dto, CancellationToken cancellationToken = default);
        Task<Block2Level1Dto> FindBlock2Level1ById(Guid id, CancellationToken cancellationToken = default);
        Task<List<Block2Level1Dto>> FindBlock2Level1s(CancellationToken cancellationToken = default);
        Task DeleteBlock2Level1(Guid id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Regression anchor: a GET with no return type. It must model without a validation error, and the endpoint answers 204 No Content.
        /// </summary>
        Task Ping(CancellationToken cancellationToken = default);
    }
}