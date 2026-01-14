using Accelerators.Application.Block2Level1s;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Accelerators.Application
{
    public class CreateBlock2Level1Block2Level2sDto
    {
        public CreateBlock2Level1Block2Level2sDto()
        {
            Name = null!;
            BeforeRename = null!;
            Block2Level3 = null!;
        }

        public string Name { get; set; }
        public string BeforeRename { get; set; }
        public CreateBlock2Level1Block2Level3Dto Block2Level3 { get; set; }
        public int TestEnum { get; set; }
        public bool ToBeDeleted { get; set; }

        public static CreateBlock2Level1Block2Level2sDto Create(
            string name,
            string beforeRename,
            CreateBlock2Level1Block2Level3Dto block2Level3,
            int testEnum,
            bool toBeDeleted)
        {
            return new CreateBlock2Level1Block2Level2sDto
            {
                Name = name,
                BeforeRename = beforeRename,
                Block2Level3 = block2Level3,
                TestEnum = testEnum,
                ToBeDeleted = toBeDeleted
            };
        }
    }
}