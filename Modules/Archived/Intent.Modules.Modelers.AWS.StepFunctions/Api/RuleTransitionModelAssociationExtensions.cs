using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.StepFunctions.Api
{
    public static class RuleTransitionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<RuleTransitionTargetEndModel> ChoiceTransitionTarget(this ChoiceModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RuleTransitionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => RuleTransitionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<RuleTransitionSourceEndModel> ChoiceTransitionSource(this ChoiceModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RuleTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RuleTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<RuleTransitionSourceEndModel> ChoiceTransitionSource(this ParallelModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RuleTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RuleTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<RuleTransitionSourceEndModel> ChoiceTransitionSource(this WaitModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RuleTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RuleTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<RuleTransitionSourceEndModel> ChoiceTransitionSource(this SuccessModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RuleTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RuleTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<RuleTransitionSourceEndModel> ChoiceTransitionSource(this FailModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RuleTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RuleTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<RuleTransitionSourceEndModel> ChoiceTransitionSource(this StartModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RuleTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RuleTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<RuleTransitionSourceEndModel> ChoiceTransitionSource(this EndModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RuleTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RuleTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<RuleTransitionSourceEndModel> ChoiceTransitionSource(this PassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RuleTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RuleTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<RuleTransitionSourceEndModel> ChoiceTransitionSource(this LambdaInvokeModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RuleTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RuleTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<RuleTransitionSourceEndModel> ChoiceTransitionSource(this SQSSendMessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RuleTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RuleTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<RuleTransitionEndModel> RuleTransitionEnds(this ChoiceModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsRuleTransitionEndModel())
                .Select(RuleTransitionEndModel.Create)
                .ToList();
        }

    }
}