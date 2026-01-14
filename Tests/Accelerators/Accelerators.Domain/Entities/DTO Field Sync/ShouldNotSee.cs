using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Accelerators.Domain.Entities.DTOFieldSync
{
    public class ShouldNotSee
    {
        public ShouldNotSee()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}