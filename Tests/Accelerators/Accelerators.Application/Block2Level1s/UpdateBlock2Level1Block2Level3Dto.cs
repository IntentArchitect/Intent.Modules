using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Accelerators.Application.Block2Level1s
{
    public class UpdateBlock2Level1Block2Level3Dto
    {
        public UpdateBlock2Level1Block2Level3Dto()
        {
            Name = null!;
            BeforeRename = null!;
            ToBeDeleted = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BeforeRename { get; set; }
        public string ToBeDeleted { get; set; }
        public int TestEnum { get; set; }

        public static UpdateBlock2Level1Block2Level3Dto Create(
            Guid id,
            string name,
            string beforeRename,
            string toBeDeleted,
            int testEnum)
        {
            return new UpdateBlock2Level1Block2Level3Dto
            {
                Id = id,
                Name = name,
                BeforeRename = beforeRename,
                ToBeDeleted = toBeDeleted,
                TestEnum = testEnum
            };
        }
    }
}