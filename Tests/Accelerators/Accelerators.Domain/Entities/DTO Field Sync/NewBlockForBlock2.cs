using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Accelerators.Domain.Entities.DTOFieldSync
{
    public class NewBlockForBlock2
    {
        public NewBlockForBlock2()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid Block2Level1Id { get; set; }
    }
}