using Accelerators.Domain.Entities.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Accelerators.Domain.Repositories.DTOFieldSync
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IShouldNotSeeRepository : IEFRepository<ShouldNotSee, ShouldNotSee>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ShouldNotSee?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ShouldNotSee?> FindByIdAsync(Guid id, Func<IQueryable<ShouldNotSee>, IQueryable<ShouldNotSee>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ShouldNotSee>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}