using Accelerators.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Accelerators.Application.Block1Level1s
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateBlock1Level1CommandBlock1Level2sDtoValidator : AbstractValidator<UpdateBlock1Level1CommandBlock1Level2sDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateBlock1Level1CommandBlock1Level2sDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.BeforeRename)
                .NotNull();

            RuleFor(v => v.Block1Level3)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateBlock1Level1CommandBlock1Level3Dto>()!);
        }
    }
}