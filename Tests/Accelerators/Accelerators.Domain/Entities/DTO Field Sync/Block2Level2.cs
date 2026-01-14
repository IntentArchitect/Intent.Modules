using Accelerators.Domain.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Accelerators.Domain.Entities.DTOFieldSync
{
    public class Block2Level2
    {
        public Block2Level2()
        {
            Name = null!;
            Renamed = null!;
            Money = null!;
            Block2Level3 = null!;
            ShouldNotSee = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid Block2Level1Id { get; set; }

        public bool Added { get; set; }

        public string Renamed { get; set; }

        public TestEnum TestEnum { get; set; }

        public Guid ShouldNotSeeId { get; set; }

        public Money Money { get; set; }

        public virtual Block2Level3 Block2Level3 { get; set; }

        public virtual ShouldNotSee ShouldNotSee { get; set; }
    }
}