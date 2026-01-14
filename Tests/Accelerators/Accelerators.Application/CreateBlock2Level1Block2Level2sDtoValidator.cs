using Accelerators.Application.Block2Level1s;
using Accelerators.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Accelerators.Application
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateBlock2Level1Block2Level2sDtoValidator : AbstractValidator<CreateBlock2Level1Block2Level2sDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBlock2Level1Block2Level2sDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.BeforeRename)
                .NotNull();

            RuleFor(v => v.Block2Level3)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateBlock2Level1Block2Level3Dto>()!);
        }
    }
}