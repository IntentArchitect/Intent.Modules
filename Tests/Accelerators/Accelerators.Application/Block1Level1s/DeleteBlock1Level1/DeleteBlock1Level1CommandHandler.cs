using Accelerators.Domain.Common.Exceptions;
using Accelerators.Domain.Repositories.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Accelerators.Application.Block1Level1s.DeleteBlock1Level1
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteBlock1Level1CommandHandler : IRequestHandler<DeleteBlock1Level1Command>
    {
        private readonly IBlock1Level1Repository _block1Level1Repository;

        [IntentManaged(Mode.Merge)]
        public DeleteBlock1Level1CommandHandler(IBlock1Level1Repository block1Level1Repository)
        {
            _block1Level1Repository = block1Level1Repository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteBlock1Level1Command request, CancellationToken cancellationToken)
        {
            var block1Level1 = await _block1Level1Repository.FindByIdAsync(request.Id, cancellationToken);
            if (block1Level1 is null)
            {
                throw new NotFoundException($"Could not find Block1Level1 '{request.Id}'");
            }


            _block1Level1Repository.Remove(block1Level1);
        }
    }
}