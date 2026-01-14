using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Accelerators.Application.Block2Level1s
{
    public class Block2Level1Dto
    {
        public Block2Level1Dto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static Block2Level1Dto Create(Guid id, string name)
        {
            return new Block2Level1Dto
            {
                Id = id,
                Name = name
            };
        }
    }
}