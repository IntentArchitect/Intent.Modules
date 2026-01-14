using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s
{
    public class UpdateBlock1Level1CommandBlock1Level3Dto
    {
        public UpdateBlock1Level1CommandBlock1Level3Dto()
        {
            Name = null!;
            BeforeRename = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BeforeRename { get; set; }
        public int TestEnum { get; set; }
        public bool ToBeDeleted { get; set; }

        public static UpdateBlock1Level1CommandBlock1Level3Dto Create(
            Guid id,
            string name,
            string beforeRename,
            int testEnum,
            bool toBeDeleted)
        {
            return new UpdateBlock1Level1CommandBlock1Level3Dto
            {
                Id = id,
                Name = name,
                BeforeRename = beforeRename,
                TestEnum = testEnum,
                ToBeDeleted = toBeDeleted
            };
        }
    }
}