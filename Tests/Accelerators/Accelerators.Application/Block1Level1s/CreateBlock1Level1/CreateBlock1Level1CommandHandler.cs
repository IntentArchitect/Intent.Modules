using Accelerators.Domain.DTOFieldSync;
using Accelerators.Domain.Entities.DTOFieldSync;
using Accelerators.Domain.Repositories.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Accelerators.Application.Block1Level1s.CreateBlock1Level1
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateBlock1Level1CommandHandler : IRequestHandler<CreateBlock1Level1Command, Guid>
    {
        private readonly IBlock1Level1Repository _block1Level1Repository;

        [IntentManaged(Mode.Merge)]
        public CreateBlock1Level1CommandHandler(IBlock1Level1Repository block1Level1Repository)
        {
            _block1Level1Repository = block1Level1Repository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateBlock1Level1Command request, CancellationToken cancellationToken)
        {
            var block1Level1 = new Block1Level1
            {
                Name = request.Name,
                Renamed = request.BeforeRename,
                TestEnum = (TestEnum)request.TestEnum,
                Block1Level2s = request.Block1Level2s
                    .Select(bl => new Block1Level2
                    {
                        Name = bl.Name,
                        Renamed = bl.BeforeRename,
                        TestEnum = (TestEnum)bl.TestEnum,
                        Block1Level3 = new Block1Level3
                        {
                            Name = bl.Block1Level3.Name,
                            Renamed = bl.Block1Level3.BeforeRename,
                            TestEnum = (TestEnum)bl.Block1Level3.TestEnum
                        }
                    })
                    .ToList()
            };

            _block1Level1Repository.Add(block1Level1);
            await _block1Level1Repository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return block1Level1.Id;
        }
    }
}