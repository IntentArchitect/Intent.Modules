using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s
{
    public class UpdateBlock1Level1CommandMoneyDto
    {
        public UpdateBlock1Level1CommandMoneyDto()
        {
            Field = null!;
            Field1 = null!;
        }

        public string Field { get; set; }
        public string Field1 { get; set; }

        public static UpdateBlock1Level1CommandMoneyDto Create(string field, string field1)
        {
            return new UpdateBlock1Level1CommandMoneyDto
            {
                Field = field,
                Field1 = field1
            };
        }
    }
}