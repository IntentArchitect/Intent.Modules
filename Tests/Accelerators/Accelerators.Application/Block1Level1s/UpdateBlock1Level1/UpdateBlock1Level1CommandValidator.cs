using Accelerators.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Accelerators.Application.Block1Level1s.UpdateBlock1Level1
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateBlock1Level1CommandValidator : AbstractValidator<UpdateBlock1Level1Command>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateBlock1Level1CommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Block1Level2s)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateBlock1Level1CommandBlock1Level2sDto>()!));

            RuleFor(v => v.BeforeRename)
                .NotNull();

            RuleFor(v => v.Param)
                .NotNull();
        }
    }
}