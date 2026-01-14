using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Accelerators.Application.Block1Level1s
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateBlock1Level1CommandBlock1Level3DtoValidator : AbstractValidator<UpdateBlock1Level1CommandBlock1Level3Dto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateBlock1Level1CommandBlock1Level3DtoValidator()
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