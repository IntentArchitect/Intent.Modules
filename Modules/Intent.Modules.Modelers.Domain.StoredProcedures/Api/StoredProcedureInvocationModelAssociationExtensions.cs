using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain.StoredProcedures.Api
{
    public static class StoredProcedureInvocationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<StoredProcedureInvocationTargetEndModel> StoredProcedureInvocationTargets(this OperationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StoredProcedureInvocationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => StoredProcedureInvocationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StoredProcedureInvocationSourceEndModel> StoredProcedureInvocationSources(this StoredProcedureModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StoredProcedureInvocationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => StoredProcedureInvocationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentIgnore]
        public static IElementToElementMapping GetMapInvocationMapping(this StoredProcedureInvocationTargetEndModel model)
        {
            return model.Mappings.SingleOrDefault(x => x.TypeId == "a7dbfc5c-f4f4-4f61-a176-6b652192ebfc");
        }

        [IntentIgnore]
        public static IElementToElementMapping GetMapResultMapping(this StoredProcedureInvocationTargetEndModel model)
        {
            return model.Mappings.SingleOrDefault(x => x.TypeId == "0af211bd-11c4-4981-b7bc-c42923a884d8");
        }
    }
}