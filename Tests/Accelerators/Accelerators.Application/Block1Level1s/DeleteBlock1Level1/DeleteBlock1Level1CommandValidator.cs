using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Accelerators.Application.Block1Level1s.DeleteBlock1Level1
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteBlock1Level1CommandValidator : AbstractValidator<DeleteBlock1Level1Command>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteBlock1Level1CommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}