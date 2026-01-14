using Accelerators.Application.Block2Level1s;
using Accelerators.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Accelerators.Application
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateBlock2Level1Block2Level2sDtoValidator : AbstractValidator<UpdateBlock2Level1Block2Level2sDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateBlock2Level1Block2Level2sDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.BeforeRename)
                .NotNull();

            RuleFor(v => v.ToBeDeleted)
                .NotNull();

            RuleFor(v => v.Block2Level3)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateBlock2Level1Block2Level3Dto>()!);
        }
    }
}