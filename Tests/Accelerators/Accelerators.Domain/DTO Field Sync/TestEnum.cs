using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace Accelerators.Domain.DTOFieldSync
{
    public enum TestEnum
    {
        Item1,
        Item2
    }
}