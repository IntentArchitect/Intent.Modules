using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace Accelerators.Domain.DTOFieldSync
{
    public class Money : ValueObject
    {
        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        [IntentMerge]
        protected Money()
        {
            Currency = null!;
        }

        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}