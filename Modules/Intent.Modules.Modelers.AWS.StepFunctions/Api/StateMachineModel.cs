using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.AWS.StepFunctions.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class StateMachineModel : IMetadataModel, IHasStereotypes, IHasName, IHasFolder
    {
        public const string SpecializationType = "State Machine";
        public const string SpecializationTypeId = "0fed6f31-391c-4402-a9e9-532ca4b11e23";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public StateMachineModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Folder = _element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId ? new FolderModel(_element.ParentElement) : null;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public FolderModel Folder { get; }

        public IElement InternalElement => _element;

        public IList<ChoiceModel> Choices => _element.ChildElements
            .GetElementsOfType(ChoiceModel.SpecializationTypeId)
            .Select(x => new ChoiceModel(x))
            .ToList();

        public IList<ParallelModel> Parallels => _element.ChildElements
            .GetElementsOfType(ParallelModel.SpecializationTypeId)
            .Select(x => new ParallelModel(x))
            .ToList();

        public IList<WaitModel> Waits => _element.ChildElements
            .GetElementsOfType(WaitModel.SpecializationTypeId)
            .Select(x => new WaitModel(x))
            .ToList();

        public IList<SuccessModel> Successes => _element.ChildElements
            .GetElementsOfType(SuccessModel.SpecializationTypeId)
            .Select(x => new SuccessModel(x))
            .ToList();

        public IList<FailModel> Fails => _element.ChildElements
            .GetElementsOfType(FailModel.SpecializationTypeId)
            .Select(x => new FailModel(x))
            .ToList();

        public StartModel Start => _element.ChildElements
            .GetElementsOfType(StartModel.SpecializationTypeId)
            .Select(x => new StartModel(x))
            .SingleOrDefault();

        public EndModel End => _element.ChildElements
            .GetElementsOfType(EndModel.SpecializationTypeId)
            .Select(x => new EndModel(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(StateMachineModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StateMachineModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class StateMachineModelExtensions
    {

        public static bool IsStateMachineModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == StateMachineModel.SpecializationTypeId;
        }

        public static StateMachineModel AsStateMachineModel(this ICanBeReferencedType type)
        {
            return type.IsStateMachineModel() ? new StateMachineModel((IElement)type) : null;
        }
    }
}