using Intent.Metadata.Security.Api;

namespace Intent.Modules.Metadata.Security.Models;

internal static class Constants
{
    public static class Stereotypes
    {
        public static class Secured
        {
            public const string Id = ServiceModelStereotypeExtensions.Secured.DefinitionId;

            public static class Properties
            {
                public const string CommaSeparatedRoles = "2b39acef-6079-48c9-b85e-2b0981f9de70";
                public const string CommaSeparatedPolicies = "ae5251ff-40a1-4e46-be66-6b176f329f98";
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