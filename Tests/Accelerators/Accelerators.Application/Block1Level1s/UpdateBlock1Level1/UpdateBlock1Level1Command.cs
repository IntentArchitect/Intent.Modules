using Accelerators.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s.UpdateBlock1Level1
{
    public class UpdateBlock1Level1Command : IRequest, ICommand
    {
        public UpdateBlock1Level1Command(Guid id,
            string name,
            List<UpdateBlock1Level1CommandBlock1Level2sDto> block1Level2s,
            string beforeRename,
            bool toBeDeleted,
            int testEnum,
            string param)
        {
            Id = id;
            Name = name;
            Block1Level2s = block1Level2s;
            BeforeRename = beforeRename;
            ToBeDeleted = toBeDeleted;
            TestEnum = testEnum;
            Param = param;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<UpdateBlock1Level1CommandBlock1Level2sDto> Block1Level2s { get; set; }
        public string BeforeRename { get; set; }
        public bool ToBeDeleted { get; set; }
        public int TestEnum { get; set; }
        public string Param { get; set; }
    }
}