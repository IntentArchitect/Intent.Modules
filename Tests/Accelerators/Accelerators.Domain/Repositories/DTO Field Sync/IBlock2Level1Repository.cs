using Accelerators.Domain.Entities.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Accelerators.Domain.Repositories.DTOFieldSync
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IBlock2Level1Repository : IEFRepository<Block2Level1, Block2Level1>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid blockId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Block2Level1?> FindByIdAsync(Guid blockId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Block2Level1?> FindByIdAsync(Guid blockId, Func<IQueryable<Block2Level1>, IQueryable<Block2Level1>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Block2Level1>> FindByIdsAsync(Guid[] blockIds, CancellationToken cancellationToken = default);
    }
}