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
    public class Block2Level1Repository : RepositoryBase<Block2Level1, Block2Level1, ApplicationDbContext>, IBlock2Level1Repository
    {
        public Block2Level1Repository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid blockId,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.BlockId == blockId, cancellationToken);
        }

        public async Task<Block2Level1?> FindByIdAsync(Guid blockId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.BlockId == blockId, cancellationToken);
        }

        public async Task<Block2Level1?> FindByIdAsync(
            Guid blockId,
            Func<IQueryable<Block2Level1>, IQueryable<Block2Level1>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.BlockId == blockId, queryOptions, cancellationToken);
        }

        public async Task<List<Block2Level1>> FindByIdsAsync(
            Guid[] blockIds,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = blockIds.ToList();
            return await FindAllAsync(x => idList.Contains(x.BlockId), cancellationToken);
        }
    }
}