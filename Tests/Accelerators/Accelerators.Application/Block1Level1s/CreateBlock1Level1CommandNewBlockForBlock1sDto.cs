using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s
{
    public class CreateBlock1Level1CommandNewBlockForBlock1sDto
    {
        public CreateBlock1Level1CommandNewBlockForBlock1sDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateBlock1Level1CommandNewBlockForBlock1sDto Create(string name)
        {
            return new CreateBlock1Level1CommandNewBlockForBlock1sDto
            {
                Name = name
            };
        }
    }
}