using Accelerators.Domain.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Accelerators.Domain.Entities.DTOFieldSync
{
    public class Block2Level3
    {
        public Block2Level3()
        {
            Name = null!;
            Renamed = null!;
            ShouldNotSee = null!;
            Money = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Added { get; set; }

        public string Renamed { get; set; }

        public TestEnum TestEnum { get; set; }

        public Guid ShouldNotSeeId { get; set; }

        public virtual ShouldNotSee ShouldNotSee { get; set; }

        public Money Money { get; set; }

        public ICollection<Money> Monies { get; set; } = [];
    }
}