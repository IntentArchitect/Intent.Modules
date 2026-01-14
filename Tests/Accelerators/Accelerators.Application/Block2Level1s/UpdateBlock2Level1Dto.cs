using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Accelerators.Application.Block2Level1s
{
    public class UpdateBlock2Level1Dto
    {
        public UpdateBlock2Level1Dto()
        {
            Name = null!;
            BeforeRename = null!;
            Block2Level2s = null!;
        }

        public string Name { get; set; }
        public bool ToBeDeleted { get; set; }
        public string BeforeRename { get; set; }
        public List<UpdateBlock2Level1Block2Level2sDto> Block2Level2s { get; set; }
        public int TestEnum { get; set; }
        public bool Added { get; set; }
        public Guid ShouldNotSeeId { get; set; }

        public static UpdateBlock2Level1Dto Create(
            string name,
            bool toBeDeleted,
            string beforeRename,
            List<UpdateBlock2Level1Block2Level2sDto> block2Level2s,
            int testEnum,
            bool added,
            Guid shouldNotSeeId)
        {
            return new UpdateBlock2Level1Dto
            {
                Name = name,
                ToBeDeleted = toBeDeleted,
                BeforeRename = beforeRename,
                Block2Level2s = block2Level2s,
                TestEnum = testEnum,
                Added = added,
                ShouldNotSeeId = shouldNotSeeId
            };
        }
    }
}