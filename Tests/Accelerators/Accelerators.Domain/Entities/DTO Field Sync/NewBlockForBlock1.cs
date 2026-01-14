using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Accelerators.Domain.Entities.DTOFieldSync
{
    public class NewBlockForBlock1
    {
        public NewBlockForBlock1()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid Block1Level1Id { get; set; }
    }
}