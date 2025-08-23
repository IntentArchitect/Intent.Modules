#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

// ReSharper disable once CheckNamespace
namespace Intent.Modules.Constants
{
    public static class VisualStudioProjectTypeIds
    {
        public const string CSharpLibrary = "0FEBBF41-7C8E-4F98-85A5-F8B5236CFD7D";
        public const string WebApiApplication = "8AF747CF-58F0-449C-8B95-46080FEFC8C0";
        public const string WcfApplication = "3CDFF513-03D8-4BAB-9435-160108A086A3";
        public const string ConsoleAppNetFramework = "673AAE96-C9B1-4B7E-9A52-ADE5F9218CFC";
        public const string NodeJsConsoleApplication = "CC13FD07-C783-4B0D-A641-4A861A22F087";
        public const string SQLServerDatabaseProject = "00D1A9C2-B5F0-4AF3-8072-F6C62B433612";
        public const string ServiceFabricProject = "A07B5EB6-E848-4116-A8D0-A826331D98C6";

        // The above GUIDs are specific project type GUIDs that Visual Studio understands for
        // old-school non-SDK style .csproj files as per the following:
        // https://github.com/VISTALL/visual-studio-project-type-guids

        // The GUIDs below are random as SDK style .csproj files don't use project type GUIDs:
        public const string CoreWebApp = "FFD54A85-9362-48AC-B646-C93AB9AC63D2";
        public const string CoreCSharpLibrary = "52B49DB5-3EA9-4095-B1A7-DF1AC22D7DAE";
        public const string AzureFunctionsProject = "73e51385-5e20-4e2c-aa0b-6eb2dc8de52e";
        public const string CoreConsoleApp = "27b265c8-e185-4c33-9908-8d23d5e945d1";

        //This seems to be the new standard for these things
        // https://github.com/dotnet/project-system/blob/main/docs/opening-with-new-project-system.md
        public const string SdkCSharpProject = "9A19103F-16F7-4668-BE54-9A1E7A4F7556";
    }
}
