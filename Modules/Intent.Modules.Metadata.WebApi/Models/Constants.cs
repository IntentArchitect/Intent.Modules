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

    public static class Stereotypes
    {
        public static class Authorize
        {
            public const string Id = "b06358cd-aed3-4c39-96cf-abb131e4ecde";

            public static class Properties
            {
                public const string RolesLegacy = "98f96218-c5a7-4656-96b6-f173947028f7";
                public const string PoliciesLegacy = "ced1460d-1bd9-4ec2-92b3-51bdbf173315";
                public const string Roles = "9c5cd2b9-52d8-47a0-96a5-f0026c3b9e48";
                public const string Policies = "2d95351e-d2e5-46da-8361-6c29bc690f98";
            }
        }

        public static class Secured
        {
            public const string Id = "a9eade71-1d56-4be7-a80c-81046c0c978b";

            public static class Properties
            {
                public const string RolesLegacy = "2b39acef-6079-48c9-b85e-2b0981f9de70";
                public const string PoliciesLegacy = "ae5251ff-40a1-4e46-be66-6b176f329f98";
                public const string Roles = "28bbe8bb-8d31-44c7-b642-ff0e279ab85f";
                public const string Policies = "68cbcd05-cd5c-49f3-a982-8ee9caf554bb";
            }
        }

        public static class Unsecured
        {
            public const string Id = "8b65c29e-1448-43ac-a92a-0e0f86efd6c6";
        }
    }
}