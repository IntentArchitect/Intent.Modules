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
        private readonly IVersionCompatibility _versionCompatibility;

        public CheckSdkClientVersionRequirement(IVersionCompatibility versionCompatibility)
        {
            _versionCompatibility = versionCompatibility;
        }


        public override string Id => "Intent.ModuleBuilder.CheckSdkClientVersionRequirement";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        //protected override void OnAfterCommitChanges(IApplication application)
        protected override void OnAfterTemplateExecution(IApplication application)
        {
            var workingDirectory = GetRootExecutionLocation(application);
            if (workingDirectory == null)
            {
                Logging.Log.Failure("Could not find location to run dotnet build command.");
                return;
            }

            // Initial run perhaps when file does not exist
            if (!Directory.Exists(Path.GetFullPath(workingDirectory)))
            {
                return;
            }

            var csprojFiles = Directory.GetFiles(workingDirectory, "*.csproj", new EnumerationOptions
            {
                MatchCasing = MatchCasing.CaseInsensitive,
                RecurseSubdirectories = false
            });

            if (csprojFiles.Length != 1)
            {
                Logging.Log.Debug($"Skipping check for SDK compatibility because the count of .csproj files was {csprojFiles.Length} in \"{workingDirectory}\"");
                return;
            }

            if (!TryGetSdkVersionFromCsproj(csprojFiles[0], out var sdkPackageVersion) &&
                !TryGetSdkVersionWithDotnetPackageList(workingDirectory, out sdkPackageVersion))
            {
                base.OnAfterCommitChanges(application);
                return;
            }

            var minimumVersionRequired = _versionCompatibility.GetMinimumRequired(sdkPackageVersion, throwErrorOnUnknownVersion: true);
            var parsedMinimumVersionRequired = NuGetVersion.Parse(minimumVersionRequired);

            var supportedVersions = GetSupportedClientVersions(application, out var supportedRange);
            if (supportedRange.MinVersion != null &&
                (
                    (supportedRange.IsMinInclusive && supportedRange.MinVersion < parsedMinimumVersionRequired) ||
                    (!supportedRange.IsMinInclusive && supportedRange.MinVersion <= parsedMinimumVersionRequired)
                ))
            {
                throw new FriendlyException(
                    $"""
                    The <supportedClientVersions/> element value in the .imodspec does not support the currently referenced version of the Intent.SoftwareFactory.SDK NuGet package.
                    
                    <supportedClientVersions/> value: {supportedVersions}
                    Resolved Intent.SoftwareFactory.SDK version: {sdkPackageVersion}
                    Minimum required version for SDK version: {minimumVersionRequired}
                    """);
            }

            base.OnAfterCommitChanges(application);
        }

        /// <summary>
        /// Retrieves from the .imodspec file.
        /// </summary>
        private static string GetSupportedClientVersions(IApplication application, out VersionRange supportedRange)
        {
            var imodSpecTemplate = application.FindTemplateInstance<IModSpecTemplate>(IModSpecTemplate.TemplateId);
            var doc = XDocument.Parse(imodSpecTemplate.TransformText());
            var supportedVersions = doc.Root?
                .Element("supportedClientVersions")?
                .Value!;
            supportedRange = VersionRange.Parse(supportedVersions);
            return supportedVersions;
        }

        private static bool TryGetSdkVersionFromCsproj(string csProjPath, [NotNullWhen(true)] out string? sdkPackageVersion)
        {
            var doc = XDocument.Parse(File.ReadAllText(csProjPath));

            sdkPackageVersion = doc
                .Descendants("PackageReference")
                .FirstOrDefault(x => (string)x.Attribute("Include")! == "Intent.SoftwareFactory.SDK")
                ?.Attribute("Version")?.Value;
            return sdkPackageVersion != null;
        }

        /// <summary>
        /// Runs "dotnet list package --include-transitive --format json", returns <see langword="true"/>
        /// if the result contains the "Intent.SoftwareFactory.SDK" NuGet package and populates the
        /// <paramref name="sdkPackageVersion"/> <see langword="out"/> parameter.
        /// </summary>
        /// <remarks>
        /// Slower than <see cref="TryGetSdkVersionFromCsproj"/> so that should be tried first.
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