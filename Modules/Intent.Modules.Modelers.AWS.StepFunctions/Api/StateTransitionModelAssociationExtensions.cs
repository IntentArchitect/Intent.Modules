using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.StepFunctions.Api
{
    public static class StateTransitionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionTargetEndModel> StateTransitionTarget(this WaitModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionTargetEndModel> StateTransitionTarget(this ParallelModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionTargetEndModel> StateTransitionTarget(this StartModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionTargetEndModel> StateTransitionTarget(this PassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionTargetEndModel> StateTransitionTarget(this LambdaInvokeModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionTargetEndModel> StateTransitionTarget(this SQSSendMessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionTargetEndModel> StateTransitionTarget(this ChoiceModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionSourceEndModel> StateTransitionSource(this ChoiceModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionSourceEndModel> StateTransitionSource(this ParallelModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionSourceEndModel> StateTransitionSource(this WaitModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionSourceEndModel> StateTransitionSource(this SuccessModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionSourceEndModel> StateTransitionSource(this FailModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionSourceEndModel> StateTransitionSource(this StartModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionSourceEndModel> StateTransitionSource(this EndModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionSourceEndModel> StateTransitionSource(this PassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionSourceEndModel> StateTransitionSource(this LambdaInvokeModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionSourceEndModel> StateTransitionSource(this SQSSendMessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StateTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => StateTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionEndModel> StateTransitionEnds(this ChoiceModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsStateTransitionEndModel())
                .Select(StateTransitionEndModel.Create)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionEndModel> StateTransitionEnds(this ParallelModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsStateTransitionEndModel())
                .Select(StateTransitionEndModel.Create)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionEndModel> StateTransitionEnds(this WaitModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsStateTransitionEndModel())
                .Select(StateTransitionEndModel.Create)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionEndModel> StateTransitionEnds(this StartModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsStateTransitionEndModel())
                .Select(StateTransitionEndModel.Create)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionEndModel> StateTransitionEnds(this PassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsStateTransitionEndModel())
                .Select(StateTransitionEndModel.Create)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionEndModel> StateTransitionEnds(this LambdaInvokeModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsStateTransitionEndModel())
                .Select(StateTransitionEndModel.Create)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StateTransitionEndModel> StateTransitionEnds(this SQSSendMessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsStateTransitionEndModel())
                .Select(StateTransitionEndModel.Create)
                .ToList();
        }

    }
}