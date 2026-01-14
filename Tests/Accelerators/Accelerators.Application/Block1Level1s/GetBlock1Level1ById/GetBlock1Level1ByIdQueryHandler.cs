using Accelerators.Domain.Common.Exceptions;
using Accelerators.Domain.Repositories.DTOFieldSync;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s.GetBlock1Level1ById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBlock1Level1ByIdQueryHandler : IRequestHandler<GetBlock1Level1ByIdQuery, Block1Level1Dto>
    {
        private readonly IBlock1Level1Repository _block1Level1Repository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetBlock1Level1ByIdQueryHandler(IBlock1Level1Repository block1Level1Repository, IMapper mapper)
        {
            _block1Level1Repository = block1Level1Repository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Block1Level1Dto> Handle(GetBlock1Level1ByIdQuery request, CancellationToken cancellationToken)
        {
            var block1Level1 = await _block1Level1Repository.FindByIdAsync(request.Id, cancellationToken);
            if (block1Level1 is null)
            {
                throw new NotFoundException($"Could not find Block1Level1 '{request.Id}'");
            }
            return block1Level1.MapToBlock1Level1Dto(_mapper);
        }
    }
}