using Accelerators.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Accelerators.Application.Block1Level1s.CreateBlock1Level1
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateBlock1Level1CommandValidator : AbstractValidator<CreateBlock1Level1Command>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBlock1Level1CommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Block1Level2s)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateBlock1Level1CommandBlock1Level2sDto>()!));

            RuleFor(v => v.BeforeRename)
                .NotNull();
        }
    }
}