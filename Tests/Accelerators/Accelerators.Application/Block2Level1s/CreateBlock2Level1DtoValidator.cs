using Accelerators.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Accelerators.Application.Block2Level1s
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateBlock2Level1DtoValidator : AbstractValidator<CreateBlock2Level1Dto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBlock2Level1DtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Block2Level2s)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateBlock2Level1Block2Level2sDto>()!));

            RuleFor(v => v.BeforeRename)
                .NotNull();
        }
    }
}