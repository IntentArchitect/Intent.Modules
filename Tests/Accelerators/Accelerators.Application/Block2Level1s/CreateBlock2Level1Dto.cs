using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Accelerators.Application.Block2Level1s
{
    public class CreateBlock2Level1Dto
    {
        public CreateBlock2Level1Dto()
        {
            Name = null!;
            Block2Level2s = null!;
            BeforeRename = null!;
        }

        public string Name { get; set; }
        public List<CreateBlock2Level1Block2Level2sDto> Block2Level2s { get; set; }
        public string BeforeRename { get; set; }
        public bool ToBeDeleted { get; set; }
        public int TestEnum { get; set; }

        public static CreateBlock2Level1Dto Create(
            string name,
            List<CreateBlock2Level1Block2Level2sDto> block2Level2s,
            string beforeRename,
            bool toBeDeleted,
            int testEnum)
        {
            return new CreateBlock2Level1Dto
            {
                Name = name,
                Block2Level2s = block2Level2s,
                BeforeRename = beforeRename,
                ToBeDeleted = toBeDeleted,
                TestEnum = testEnum
            };
        }
    }
}