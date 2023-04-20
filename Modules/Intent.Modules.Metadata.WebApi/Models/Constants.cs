using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Metadata.WebApi.Models;

internal static class Constants
{
    public static class ElementTypeIds
    {
        public const string Operation = OperationModel.SpecializationTypeId;
        public const string Command = CommandModel.SpecializationTypeId;
        public const string Query = QueryModel.SpecializationTypeId;
        public const string Folder = FolderModel.SpecializationTypeId;
        public const string Service = ServiceModel.SpecializationTypeId;
    }
}