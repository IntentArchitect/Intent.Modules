using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace Accelerators.Domain.Contracts
{
    public record DiagramContract
    {
        public DiagramContract(string valueOne, int valueTwo, string valueThree)
        {
            ValueOne = valueOne;
            ValueTwo = valueTwo;
            ValueThree = valueThree;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected DiagramContract()
        {
            ValueOne = null!;
            ValueThree = null!;
        }

        public string ValueOne { get; init; }
        public int ValueTwo { get; init; }
        public string ValueThree { get; init; }
    }
}