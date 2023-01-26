using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.StepFunctions.Api
{
    public static class ChoiceTransitionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<ChoiceTransitionTargetEndModel> ChoiceTransitionTarget(this ChoiceModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ChoiceTransitionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => ChoiceTransitionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ChoiceTransitionSourceEndModel> ChoiceTransitionSource(this ChoiceModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ChoiceTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ChoiceTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ChoiceTransitionSourceEndModel> ChoiceTransitionSource(this ParallelModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ChoiceTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ChoiceTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ChoiceTransitionSourceEndModel> ChoiceTransitionSource(this WaitModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ChoiceTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ChoiceTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ChoiceTransitionSourceEndModel> ChoiceTransitionSource(this SuccessModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ChoiceTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ChoiceTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ChoiceTransitionSourceEndModel> ChoiceTransitionSource(this FailModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ChoiceTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ChoiceTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ChoiceTransitionSourceEndModel> ChoiceTransitionSource(this StartModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ChoiceTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ChoiceTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ChoiceTransitionSourceEndModel> ChoiceTransitionSource(this EndModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ChoiceTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ChoiceTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ChoiceTransitionSourceEndModel> ChoiceTransitionSource(this PassModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ChoiceTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ChoiceTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ChoiceTransitionSourceEndModel> ChoiceTransitionSource(this LambdaInvokeModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ChoiceTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ChoiceTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ChoiceTransitionSourceEndModel> ChoiceTransitionSource(this SQSSendMessageModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ChoiceTransitionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ChoiceTransitionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ChoiceTransitionEndModel> ChoiceTransitionEnds(this ChoiceModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsChoiceTransitionEndModel())
                .Select(ChoiceTransitionEndModel.Create)
                .ToList();
        }

    }
}