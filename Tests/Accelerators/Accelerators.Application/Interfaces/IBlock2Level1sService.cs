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
    }
}