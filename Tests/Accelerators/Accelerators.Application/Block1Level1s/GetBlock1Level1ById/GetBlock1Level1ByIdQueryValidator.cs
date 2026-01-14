using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Accelerators.Application.Block1Level1s.GetBlock1Level1ById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetBlock1Level1ByIdQueryValidator : AbstractValidator<GetBlock1Level1ByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetBlock1Level1ByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}