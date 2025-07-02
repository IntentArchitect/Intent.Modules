#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Intent.Compatibility;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using NuGet.Versioning;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CheckSdkClientVersionRequirement : FactoryExtensionBase
    {
        private const string SoftwareFactorySdkPackageName = "Intent.SoftwareFactory.SDK";
        private const string PersistenceSdkPackageName = "Intent.Persistence.SDK";
        private readonly ISoftwareFactorySdkCompatibilityChecker _softwareFactorySdkCompatibilityChecker;
        private readonly IPersistenceSdkCompatibilityChecker _persistenceSdkCompatibilityChecker;

        public CheckSdkClientVersionRequirement(
            ISoftwareFactorySdkCompatibilityChecker softwareFactorySdkCompatibilityChecker,
            IPersistenceSdkCompatibilityChecker persistenceSdkCompatibilityChecker)
        {
            _softwareFactorySdkCompatibilityChecker = softwareFactorySdkCompatibilityChecker;
            _persistenceSdkCompatibilityChecker = persistenceSdkCompatibilityChecker;
        }


        public override string Id => "Intent.ModuleBuilder.CheckSdkClientVersionRequirement";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        //protected override void OnAfterCommitChanges(IApplication application)
        protected override void OnAfterTemplateExecution(IApplication application)
        {
            if (!TryGetSupportedClientVersions(application, out var supportedRange) ||
                supportedRange.MinVersion == null)
            {
                Logging.Log.Debug("Skipping SDK compatibility check as no valid minimum supported client version found .imod file");
                return;
            }

            // Initial run perhaps when file does not exist
            var workingDirectory = GetRootExecutionLocation(application);
            if (workingDirectory == null ||
                !Directory.Exists(Path.GetFullPath(workingDirectory)))
            {
                Logging.Log.Debug("Skipping SDK compatibility check as could not find location for the existing .csproj file");
                return;
            }

            var csprojFiles = Directory.GetFiles(workingDirectory, "*.csproj", new EnumerationOptions
            {
                MatchCasing = MatchCasing.CaseInsensitive,
                RecurseSubdirectories = false
            });

            if (csprojFiles.Length != 1)
            {
                Logging.Log.Debug($"Skipping SDK compatibility check as the count of .csproj files was {csprojFiles.Length} in \"{workingDirectory}\"");
                return;
            }

            var csProjDocument = XDocument.Parse(File.ReadAllText(csprojFiles[0]));
            var softwareFactorySdkVersion = GetPackageVersionFromCsproj(csProjDocument, SoftwareFactorySdkPackageName);
            var persistenceSdkVersion = GetPackageVersionFromCsproj(csProjDocument, PersistenceSdkPackageName);

            if (softwareFactorySdkVersion == null &&
                persistenceSdkVersion == null)
            {
                base.OnAfterCommitChanges(application);
                return;
            }

            var throwException = false;
            var exceptionText = new StringBuilder(
                $"""
                The <supportedClientVersions/> element value in the .imodspec does not support one or more referenced SDK NuGet package versions.
                <supportedClientVersions/> value: {supportedRange}
                
                """);

            if (softwareFactorySdkVersion != null)
            {
                var minVersionRequired = NuGetVersion.Parse(_softwareFactorySdkCompatibilityChecker.GetMinimumRequiredProductVersion(
                    softwareFactorySdkVersion.ToString(),
                    throwErrorOnUnknownVersion: true));

                if ((supportedRange.IsMinInclusive && supportedRange.MinVersion < minVersionRequired) ||
                    (!supportedRange.IsMinInclusive && supportedRange.MinVersion <= minVersionRequired))
                {
                    throwException = true;
                    exceptionText.AppendLine();
                    exceptionText.AppendLine($"Resolved {SoftwareFactorySdkPackageName} version: {softwareFactorySdkVersion}");
                    exceptionText.AppendLine($"Minimum required Intent Architect version for {SoftwareFactorySdkPackageName} version: {minVersionRequired}");
                }
            }

            if (persistenceSdkVersion != null)
            {
                var minVersionRequired = NuGetVersion.Parse(_persistenceSdkCompatibilityChecker.GetMinimumRequiredProductVersion(
                    persistenceSdkVersion.ToString(),
                    throwErrorOnUnknownVersion: true));

                if ((supportedRange.IsMinInclusive && supportedRange.MinVersion < minVersionRequired) ||
                    (!supportedRange.IsMinInclusive && supportedRange.MinVersion <= minVersionRequired))
                {
                    throwException = true;
                    exceptionText.AppendLine();
                    exceptionText.AppendLine($"Resolved {PersistenceSdkPackageName} version: {persistenceSdkVersion}");
                    exceptionText.AppendLine($"Minimum required Intent Architect version for {PersistenceSdkPackageName} version: {minVersionRequired}");
                }
            }

            if (throwException)
            {
                throw new FriendlyException(exceptionText.ToString());
            }

            base.OnAfterCommitChanges(application);
        }

        /// <summary>
        /// Retrieves from the .imodspec file.
        /// </summary>
        private static bool TryGetSupportedClientVersions(IApplication application, [NotNullWhen(true)] out VersionRange? supportedRange)
        {
            var imodSpecTemplate = application.FindTemplateInstance<IModSpecTemplate>(IModSpecTemplate.TemplateId);
            var doc = XDocument.Parse(imodSpecTemplate.TransformText());
            var supportedVersions = doc.Root?
                .Element("supportedClientVersions")?
                .Value;

            if (supportedVersions != null &&
                VersionRange.TryParse(supportedVersions, out supportedRange))
            {
                return true;
            }

            supportedRange = null;
            return false;
        }

        private static NuGetVersion? GetPackageVersionFromCsproj(XDocument csProjectDocument, string packageName)
        {
            var packageVersion = csProjectDocument
                .Descendants("PackageReference")
                .FirstOrDefault(x => (string)x.Attribute("Include")! == packageName)
                ?.Attribute("Version")?.Value;

            return packageVersion != null
                ? NuGetVersion.Parse(packageVersion)
                : null;
        }

        /// <summary>
        /// Runs "dotnet list package --include-transitive --format json", returns <see langword="true"/>
        /// if the result contains the "Intent.SoftwareFactory.SDK" NuGet package and populates the
        /// <paramref name="sdkPackageVersion"/> <see langword="out"/> parameter.
        /// </summary>
        /// <remarks>
        /// Slower than <see cref="TryGetPackageVersionFromCsproj"/> so that should be tried first.
        /// </remarks>
        private static bool TryGetSdkVersionWithDotnetPackageList(string workingDirectory, [NotNullWhen(true)] out string? sdkPackageVersion)
        {
            // Otherwise the version in the "obj" folder is out of date and the version it returns
            // may be incorrect. This is easy to reproduce by changing the in the <PackageReference />
            // element in something like VS Code (with VS closed) and then immediately doing "dotnet
            // list package".
            RunDotnetRestore(workingDirectory);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "list package --include-transitive --format json",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory,
                    EnvironmentVariables =
                    {
                        ["MSBUILDDISABLENODEREUSE"] = "1",
                        ["DOTNET_CLI_UI_LANGUAGE"] = "en"
                    }
                }
            };

            var output = new StringBuilder();
            process.OutputDataReceived += (_, args) => output.AppendLine(args.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            var deserialized = JsonSerializer.Deserialize<ListedPackages>(output.ToString(), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })!;

            var sdkPackage = deserialized.Projects
                .SelectMany(x => x.Frameworks)
                .SelectMany(x => x.AllPackages)
                .FirstOrDefault(x => string.Equals(x.Id, "Intent.SoftwareFactory.SDK"));

            sdkPackageVersion = sdkPackage?.ResolvedVersion;
            return sdkPackageVersion != null;
        }

        private static void RunDotnetRestore(string workingDirectory)
        {
            var dotnetRestoreProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "restore",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory,
                    EnvironmentVariables =
                    {
                        ["MSBUILDDISABLENODEREUSE"] = "1",
                        ["DOTNET_CLI_UI_LANGUAGE"] = "en"
                    }
                }
            };

            dotnetRestoreProcess.Start();
            dotnetRestoreProcess.WaitForExit();
        }

        private static string GetRootExecutionLocation(IApplication application)
        {
            var path = application.OutputTargets.FirstOrDefault(x =>
                x.HasTemplateInstances("Intent.ModuleBuilder.IModSpecFile") ||
                x.HasTemplateInstances("Intent.ApplicationTemplate.Builder.Templates.IatSpecFile") ||
                x.IsVSProject())?.Location!;

            return Path.GetFullPath(path);
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        // ReSharper disable ClassNeverInstantiated.Local,UnusedAutoPropertyAccessor.Local,UnusedMember.Local
        private class ListedPackages
        {
            public int Version { get; init; }
            public string Parameters { get; init; }
            public Project[] Projects { get; init; }
        }

        private class Project
        {
            public string Path { get; init; }
            public ProjectFramework[] Frameworks { get; init; }
        }

        private class ProjectFramework
        {
            public string Framework { get; init; }
            public Package[] TopLevelPackages { get; init; }
            public Package[] TransitivePackages { get; init; }

            public IEnumerable<Package> AllPackages => TopLevelPackages.Concat(TransitivePackages);
        }

        private class Package
        {
            public string Id { get; init; }
            public string? RequestedVersion { get; init; }
            public string ResolvedVersion { get; init; }
        }
        // ReSharper restore ClassNeverInstantiated.Local,UnusedAutoPropertyAccessor.Local,UnusedMember.Local
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    }
}