using Accelerators.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s.GetBlock1Level1s
{
    public class GetBlock1Level1sQuery : IRequest<List<Block1Level1Dto>>, IQuery
    {
        public GetBlock1Level1sQuery()
        {
        }
    }
}