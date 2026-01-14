using Accelerators.Domain.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Accelerators.Domain.Entities.DTOFieldSync
{
    public class Block1Level1
    {
        public Block1Level1()
        {
            Name = null!;
            Renamed = null!;
            Money = null!;
            ShouldNotSee = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Added { get; set; }

        public string Renamed { get; set; }

        public TestEnum TestEnum { get; set; }

        public Guid ShouldNotSeeId { get; set; }

        public Money Money { get; set; }

        public virtual ICollection<Block1Level2> Block1Level2s { get; set; } = [];

        public virtual ShouldNotSee ShouldNotSee { get; set; }

        public virtual ICollection<NewBlockForBlock1> NewBlockForBlock1s { get; set; } = [];
    }
}