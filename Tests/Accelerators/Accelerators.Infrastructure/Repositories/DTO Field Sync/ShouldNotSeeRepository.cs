using Accelerators.Domain.Entities.DTOFieldSync;
using Accelerators.Domain.Repositories;
using Accelerators.Domain.Repositories.DTOFieldSync;
using Accelerators.Infrastructure.Persistence;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Accelerators.Infrastructure.Repositories.DTOFieldSync
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ShouldNotSeeRepository : RepositoryBase<ShouldNotSee, ShouldNotSee, ApplicationDbContext>, IShouldNotSeeRepository
    {
        public ShouldNotSeeRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<ShouldNotSee?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<ShouldNotSee?> FindByIdAsync(
            Guid id,
            Func<IQueryable<ShouldNotSee>, IQueryable<ShouldNotSee>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<ShouldNotSee>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}