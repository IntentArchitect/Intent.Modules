using Accelerators.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s.CreateBlock1Level1
{
    public class CreateBlock1Level1Command : IRequest<Guid>, ICommand
    {
        public CreateBlock1Level1Command(string name,
            List<CreateBlock1Level1CommandBlock1Level2sDto> block1Level2s,
            string beforeRename,
            bool toBeDeleted,
            int testEnum)
        {
            Name = name;
            Block1Level2s = block1Level2s;
            BeforeRename = beforeRename;
            ToBeDeleted = toBeDeleted;
            TestEnum = testEnum;
        }

        public string Name { get; set; }
        public List<CreateBlock1Level1CommandBlock1Level2sDto> Block1Level2s { get; set; }
        public string BeforeRename { get; set; }
        public bool ToBeDeleted { get; set; }
        public int TestEnum { get; set; }
    }
}