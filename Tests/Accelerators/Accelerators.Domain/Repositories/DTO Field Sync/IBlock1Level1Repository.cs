using Accelerators.Domain.Entities.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Accelerators.Domain.Repositories.DTOFieldSync
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IBlock1Level1Repository : IEFRepository<Block1Level1, Block1Level1>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Block1Level1?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Block1Level1?> FindByIdAsync(Guid id, Func<IQueryable<Block1Level1>, IQueryable<Block1Level1>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Block1Level1>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}