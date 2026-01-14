using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s
{
    public class CreateBlock1Level1CommandBlock1Level3Dto
    {
        public CreateBlock1Level1CommandBlock1Level3Dto()
        {
            Name = null!;
            BeforeRename = null!;
        }

        public string Name { get; set; }
        public string BeforeRename { get; set; }
        public int TestEnum { get; set; }
        public bool ToBeDeleted { get; set; }

        public static CreateBlock1Level1CommandBlock1Level3Dto Create(
            string name,
            string beforeRename,
            int testEnum,
            bool toBeDeleted)
        {
            return new CreateBlock1Level1CommandBlock1Level3Dto
            {
                Name = name,
                BeforeRename = beforeRename,
                TestEnum = testEnum,
                ToBeDeleted = toBeDeleted
            };
        }
    }
}