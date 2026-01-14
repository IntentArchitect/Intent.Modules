using Accelerators.Domain.Repositories.DTOFieldSync;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s.GetBlock1Level1s
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBlock1Level1sQueryHandler : IRequestHandler<GetBlock1Level1sQuery, List<Block1Level1Dto>>
    {
        private readonly IBlock1Level1Repository _block1Level1Repository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetBlock1Level1sQueryHandler(IBlock1Level1Repository block1Level1Repository, IMapper mapper)
        {
            _block1Level1Repository = block1Level1Repository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<Block1Level1Dto>> Handle(GetBlock1Level1sQuery request, CancellationToken cancellationToken)
        {
            var block1Level1s = await _block1Level1Repository.FindAllAsync(cancellationToken);
            return block1Level1s.MapToBlock1Level1DtoList(_mapper);
        }
    }
}