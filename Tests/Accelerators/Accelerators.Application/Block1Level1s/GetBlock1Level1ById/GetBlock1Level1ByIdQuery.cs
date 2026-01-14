using Accelerators.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s.GetBlock1Level1ById
{
    public class GetBlock1Level1ByIdQuery : IRequest<Block1Level1Dto>, IQuery
    {
        public GetBlock1Level1ByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}