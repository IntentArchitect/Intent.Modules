using Accelerators.Application.Block2Level1s;
using Accelerators.Application.Interfaces;
using Accelerators.Domain.Common;
using Accelerators.Domain.Common.Exceptions;
using Accelerators.Domain.DTOFieldSync;
using Accelerators.Domain.Entities.DTOFieldSync;
using Accelerators.Domain.Repositories.DTOFieldSync;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Accelerators.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class Block2Level1sService : IBlock2Level1sService
    {
        private readonly IBlock2Level1Repository _block2Level1Repository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public Block2Level1sService(IBlock2Level1Repository block2Level1Repository, IMapper mapper)
        {
            _block2Level1Repository = block2Level1Repository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateBlock2Level1(
            CreateBlock2Level1Dto dto,
            CancellationToken cancellationToken = default)
        {
            var block2Level1 = new Block2Level1
            {
                Name = dto.Name,
                Renamed = dto.BeforeRename,
                TestEnum = (TestEnum)dto.TestEnum,
                Block2Level2s = dto.Block2Level2s
                    .Select(bl => new Block2Level2
                    {
                        Name = bl.Name,
                        Renamed = bl.BeforeRename,
                        TestEnum = (TestEnum)bl.TestEnum,
                        Block2Level3 = new Block2Level3
                        {
                            Name = bl.Block2Level3.Name,
                            Renamed = bl.Block2Level3.BeforeRename,
                            TestEnum = (TestEnum)bl.Block2Level3.TestEnum
                        }
                    })
                    .ToList()
            };

            _block2Level1Repository.Add(block2Level1);
            await _block2Level1Repository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return block2Level1.BlockId;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateBlock2Level1(
            Guid id,
            UpdateBlock2Level1Dto dto,
            CancellationToken cancellationToken = default)
        {
            var block2Level1 = await _block2Level1Repository.FindByIdAsync(id, cancellationToken);
            if (block2Level1 is null)
            {
                throw new NotFoundException($"Could not find Block2Level1 '{id}'");
            }

            block2Level1.Name = dto.Name;
            block2Level1.Added = dto.Added;
            block2Level1.Renamed = dto.BeforeRename;
            block2Level1.TestEnum = (TestEnum)dto.TestEnum;
            block2Level1.ShouldNotSeeId = dto.ShouldNotSeeId;
            block2Level1.Block2Level2s = UpdateHelper.CreateOrUpdateCollection(block2Level1.Block2Level2s, dto.Block2Level2s, (e, d) => e.Id == d.Id, CreateOrUpdateBlock2Level2);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Block2Level1Dto> FindBlock2Level1ById(Guid id, CancellationToken cancellationToken = default)
        {
            var block2Level1 = await _block2Level1Repository.FindByIdAsync(id, cancellationToken);
            if (block2Level1 is null)
            {
                throw new NotFoundException($"Could not find Block2Level1 '{id}'");
            }
            return block2Level1.MapToBlock2Level1Dto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<Block2Level1Dto>> FindBlock2Level1s(CancellationToken cancellationToken = default)
        {
            var block2Level1s = await _block2Level1Repository.FindAllAsync(cancellationToken);
            return block2Level1s.MapToBlock2Level1DtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteBlock2Level1(Guid id, CancellationToken cancellationToken = default)
        {
            var block2Level1 = await _block2Level1Repository.FindByIdAsync(id, cancellationToken);
            if (block2Level1 is null)
            {
                throw new NotFoundException($"Could not find Block2Level1 '{id}'");
            }


            _block2Level1Repository.Remove(block2Level1);
        }

        [IntentManaged(Mode.Fully)]
        private static Block2Level2 CreateOrUpdateBlock2Level2(
            Block2Level2? entity,
            UpdateBlock2Level1Block2Level2sDto dto)
        {
            entity ??= new Block2Level2();
            entity.Id = dto.Id;
            entity.Name = dto.Name;
            entity.Added = dto.Added;
            entity.Renamed = dto.BeforeRename;
            entity.TestEnum = (TestEnum)dto.TestEnum;
            entity.ShouldNotSeeId = dto.ShouldNotSeeId;
            entity.Block2Level3.Id = dto.Block2Level3.Id;
            entity.Block2Level3.Name = dto.Block2Level3.Name;
            entity.Block2Level3.Added = dto.Added1;
            entity.Block2Level3.Renamed = dto.Block2Level3.BeforeRename;
            entity.Block2Level3.TestEnum = (TestEnum)dto.Block2Level3.TestEnum;
            entity.Block2Level3.ShouldNotSeeId = dto.ShouldNotSeeId1;
            return entity;
        }
    }
}