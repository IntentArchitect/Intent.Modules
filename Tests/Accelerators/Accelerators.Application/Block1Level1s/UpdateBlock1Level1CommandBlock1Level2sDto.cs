using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s
{
    public class UpdateBlock1Level1CommandBlock1Level2sDto
    {
        public UpdateBlock1Level1CommandBlock1Level2sDto()
        {
            Name = null!;
            BeforeRename = null!;
            Block1Level3 = null!;
        }

        public string Name { get; set; }
        public string BeforeRename { get; set; }
        public UpdateBlock1Level1CommandBlock1Level3Dto Block1Level3 { get; set; }
        public int TestEnum { get; set; }
        public bool ToBeDeleted { get; set; }

        public static UpdateBlock1Level1CommandBlock1Level2sDto Create(
            string name,
            string beforeRename,
            UpdateBlock1Level1CommandBlock1Level3Dto block1Level3,
            int testEnum,
            bool toBeDeleted)
        {
            return new UpdateBlock1Level1CommandBlock1Level2sDto
            {
                Name = name,
                BeforeRename = beforeRename,
                Block1Level3 = block1Level3,
                TestEnum = testEnum,
                ToBeDeleted = toBeDeleted
            };
        }
    }
}