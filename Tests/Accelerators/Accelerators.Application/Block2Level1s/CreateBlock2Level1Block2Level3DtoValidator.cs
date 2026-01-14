using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Accelerators.Application.Block2Level1s
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateBlock2Level1Block2Level3DtoValidator : AbstractValidator<CreateBlock2Level1Block2Level3Dto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBlock2Level1Block2Level3DtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.BeforeRename)
                .NotNull();
        }
    }
}