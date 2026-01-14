using Accelerators.Domain.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Accelerators.Domain.Entities.DTOFieldSync
{
    public class Block1Level2
    {
        public Block1Level2()
        {
            Name = null!;
            Renamed = null!;
            Money = null!;
            Block1Level3 = null!;
            ShouldNotSee = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid Block1Level1Id { get; set; }

        public bool Added { get; set; }

        public string Renamed { get; set; }

        public TestEnum TestEnum { get; set; }

        public Guid ShouldNotSeeId { get; set; }

        public Money Money { get; set; }

        public virtual Block1Level3 Block1Level3 { get; set; }

        public virtual ShouldNotSee ShouldNotSee { get; set; }
    }
}