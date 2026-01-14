using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Accelerators.Application.Block1Level1s.GetBlock1Level1s
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetBlock1Level1sQueryValidator : AbstractValidator<GetBlock1Level1sQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetBlock1Level1sQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}