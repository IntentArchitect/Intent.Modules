using Accelerators.Application.Block2Level1s;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Accelerators.Application
{
    public class UpdateBlock2Level1Block2Level2sDto
    {
        public UpdateBlock2Level1Block2Level2sDto()
        {
            Name = null!;
            BeforeRename = null!;
            ToBeDeleted = null!;
            Block2Level3 = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BeforeRename { get; set; }
        public string ToBeDeleted { get; set; }
        public UpdateBlock2Level1Block2Level3Dto Block2Level3 { get; set; }
        public int TestEnum { get; set; }
        public bool Added { get; set; }
        public Guid ShouldNotSeeId { get; set; }
        public bool Added1 { get; set; }
        public Guid ShouldNotSeeId1 { get; set; }

        public static UpdateBlock2Level1Block2Level2sDto Create(
            Guid id,
            string name,
            string beforeRename,
            string toBeDeleted,
            UpdateBlock2Level1Block2Level3Dto block2Level3,
            int testEnum,
            bool added,
            Guid shouldNotSeeId,
            bool added1,
            Guid shouldNotSeeId1)
        {
            return new UpdateBlock2Level1Block2Level2sDto
            {
                Id = id,
                Name = name,
                BeforeRename = beforeRename,
                ToBeDeleted = toBeDeleted,
                Block2Level3 = block2Level3,
                TestEnum = testEnum,
                Added = added,
                ShouldNotSeeId = shouldNotSeeId,
                Added1 = added1,
                ShouldNotSeeId1 = shouldNotSeeId1
            };
        }
    }
}