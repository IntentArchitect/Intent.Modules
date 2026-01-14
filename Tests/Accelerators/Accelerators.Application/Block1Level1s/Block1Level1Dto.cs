using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s
{
    public class Block1Level1Dto
    {
        public Block1Level1Dto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static Block1Level1Dto Create(Guid id, string name)
        {
            return new Block1Level1Dto
            {
                Id = id,
                Name = name
            };
        }
    }
}