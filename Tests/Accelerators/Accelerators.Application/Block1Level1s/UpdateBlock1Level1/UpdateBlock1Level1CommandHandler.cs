using Accelerators.Domain.Common;
using Accelerators.Domain.Common.Exceptions;
using Accelerators.Domain.DTOFieldSync;
using Accelerators.Domain.Entities.DTOFieldSync;
using Accelerators.Domain.Repositories.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Accelerators.Application.Block1Level1s.UpdateBlock1Level1
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateBlock1Level1CommandHandler : IRequestHandler<UpdateBlock1Level1Command>
    {
        private readonly IBlock1Level1Repository _block1Level1Repository;

        [IntentManaged(Mode.Merge)]
        public UpdateBlock1Level1CommandHandler(IBlock1Level1Repository block1Level1Repository)
        {
            _block1Level1Repository = block1Level1Repository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateBlock1Level1Command request, CancellationToken cancellationToken)
        {
            var block1Level1 = await _block1Level1Repository.FindByIdAsync(request.Id, cancellationToken);
            if (block1Level1 is null)
            {
                throw new NotFoundException($"Could not find Block1Level1 '{request.Id}'");
            }

            block1Level1.Name = request.Name;
            block1Level1.Renamed = request.BeforeRename;
            block1Level1.TestEnum = (TestEnum)request.TestEnum;
            block1Level1.Block1Level2s = UpdateHelper.CreateOrUpdateCollection(block1Level1.Block1Level2s, request.Block1Level2s, (e, d) => false, CreateOrUpdateBlock1Level2);
        }

        [IntentManaged(Mode.Fully)]
        private static Block1Level2 CreateOrUpdateBlock1Level2(
            Block1Level2? entity,
            UpdateBlock1Level1CommandBlock1Level2sDto dto)
        {
            entity ??= new Block1Level2();
            entity.Name = dto.Name;
            entity.Renamed = dto.BeforeRename;
            entity.TestEnum = (TestEnum)dto.TestEnum;
            entity.Block1Level3.Id = dto.Block1Level3.Id;
            entity.Block1Level3.Name = dto.Block1Level3.Name;
            entity.Block1Level3.Renamed = dto.Block1Level3.BeforeRename;
            entity.Block1Level3.TestEnum = (TestEnum)dto.Block1Level3.TestEnum;
            return entity;
        }
    }
}