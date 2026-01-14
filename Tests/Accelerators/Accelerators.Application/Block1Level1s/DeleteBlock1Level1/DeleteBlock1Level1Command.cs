using Accelerators.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s.DeleteBlock1Level1
{
    public class DeleteBlock1Level1Command : IRequest, ICommand
    {
        public DeleteBlock1Level1Command(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}