using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
    internal class CSharpProject : ICSharpProject, IHasStereotypes
	{
        private static readonly MajorMinorVersion LatestLanguageVersion = MajorMinorVersion.Create(11, 0);
        private static readonly MajorMinorVersion PreviewLanguageVersion = MajorMinorVersion.Create(11, 0);
        private readonly Lazy<bool> _isNullableAwareContext;
        private readonly IOutputTarget _outputTarget;

        public CSharpProject(IOutputTarget outputTarget)
        {
            _outputTarget = outputTarget;
            _isNullableAwareContext = new Lazy<bool>(GetIsNullableAwareContext);

            if (_outputTarget.Metadata.TryGetValue("Language Version", out var languageVersion))
            {
                LanguageVersion = languageVersion as string;
            }
            if (_outputTarget.Metadata.TryGetValue("Nullable Enabled", out var nullableEnabled))
            {
                NullableEnabled = nullableEnabled as bool? ?? false;
            }
            if (_outputTarget.Metadata.TryGetValue("Target Frameworks", out var targetFrameworks))
            {
                TargetFrameworks = targetFrameworks as IEnumerable<string>;
            }
            if (_outputTarget.Metadata.TryGetValue("InternalElement", out var internalElement))
            {
                InternalElement = internalElement as IElement;
            }
        }

        public string Id => _outputTarget.Id;
        public string Name => _outputTarget.Name;
        public string Location => _outputTarget.Location;
        public string RelativeLocation => _outputTarget.RelativeLocation;
        public string Type => _outputTarget.Type;
        public IOutputTarget Parent => _outputTarget.Parent;
        public IEnumerable<ITemplate> TemplateInstances => _outputTarget.TemplateInstances;
        public IDictionary<string, object> Metadata => _outputTarget.Metadata;
        public IApplication Application => _outputTarget.Application;
        public ISoftwareFactoryExecutionContext ExecutionContext => _outputTarget.ExecutionContext;
        public string LanguageVersion { get; }
        public bool NullableEnabled { get; }
        public IElement InternalElement { get; }
        public IEnumerable<string> TargetFrameworks { get; }
        public bool IsNullableAwareContext() => _isNullableAwareContext.Value;
        public bool IsNetCore2App => GetSupportedFrameworks().Any(x => x.StartsWith("netcoreapp2"));
        public bool IsNetCore3App => GetSupportedFrameworks().Any(x => x.StartsWith("netcoreapp3"));
        /// <inheritdoc />
        [Obsolete(WillBeRemovedIn.Version4)]
        public bool IsNet4App => IsNetApp(4);
        /// <inheritdoc />
        [Obsolete(WillBeRemovedIn.Version4)]
        public bool IsNet5App => IsNetApp(5);
        /// <inheritdoc />
        [Obsolete(WillBeRemovedIn.Version4)]
        public bool IsNet6App => IsNetApp(6);

        public bool IsNetApp(byte version)
        {
            var startsWith = $"net{version:D}";
            return GetSupportedFrameworks().Any(x => x.StartsWith(startsWith));
        }

        public bool TryGetMaxNetAppVersion(out MajorMinorVersion majorMinorVersion)
        {
            return TryGetMaxNetAppVersion(GetSupportedFrameworks().ToArray(), out majorMinorVersion);
        }

        public MajorMinorVersion GetMaxNetAppVersion()
        {
            return GetMaxNetAppVersion(GetSupportedFrameworks().ToArray());
        }

        public Version[] TargetDotNetFrameworks => GetSupportedFrameworks()
            .Select(x => Version.TryParse($"{x.RemovePrefix("netcoreapp", "net")}.0", out var ver)
                ? ver
                : null)
            .Where(x => x != null)
            .ToArray();

		public IEnumerable<IStereotype> Stereotypes => InternalElement.Stereotypes;

		public bool Equals(IOutputTarget other)
        {
            return _outputTarget.Equals(other);
        }

        public IList<IOutputTarget> GetTargetPath()
        {
            return _outputTarget.GetTargetPath();
        }

        public bool HasRole(string role)
        {
            return _outputTarget.HasRole(role);
        }

        public bool OutputsTemplate(string templateId)
        {
            return _outputTarget.OutputsTemplate(templateId);
        }

        public IEnumerable<string> GetSupportedFrameworks()
        {
            return _outputTarget.GetSupportedFrameworks();
        }

        public bool HasTemplateInstances(string templateId)
        {
            return _outputTarget.HasTemplateInstances(templateId);
        }

        public bool HasTemplateInstances(string templateId, Func<ITemplate, bool> predicate)
        {
            return _outputTarget.HasTemplateInstances(templateId, predicate);
        }

        private bool GetIsNullableAwareContext()
        {
            if (!NullableEnabled)
            {
                return false;
            }

            return GetLanguageVersion() >= MajorMinorVersion.Create(8, 0);
        }

        public MajorMinorVersion GetLanguageVersion()
        {
            return LanguageVersion switch
            {
                "latest" => LatestLanguageVersion,
                "preview" => PreviewLanguageVersion,
                "default" => TargetFrameworks.Select(GetDefaultLanguageVersion).DefaultIfEmpty().Min(),
                _ => MajorMinorVersion.Parse(LanguageVersion)
            };
        }

        /// <summary>
        /// Based on data from the below links, returns the default language version for the
        /// provided <paramref name="frameworkMoniker"/>.
        /// <br/>
        /// https://docs.microsoft.com/dotnet/csharp/language-reference/configure-language-version#defaults
        /// <br/>
        /// https://docs.microsoft.com/dotnet/standard/frameworks#supported-target-frameworks
        /// </summary>
        private MajorMinorVersion GetDefaultLanguageVersion(string frameworkMoniker)
        {
            switch (frameworkMoniker)
            {
                case "net11":
                case "net20":
                case "net35":
                case "net40":
                case "net403":
                case "net45":
                case "net451":
                case "net452":
                case "net46":
                case "net461":
                case "net462":
                case "net47":
                case "net471":
                case "net472":
                case "net48":
                    return MajorMinorVersion.Create(7, 3);
                case "netcoreapp1.0":
                case "netcoreapp1.1":
                case "netcoreapp2.0":
                case "netcoreapp2.1":
                case "netcoreapp2.2":
                    return MajorMinorVersion.Create(7, 3);
                case "netcoreapp3.0":
                case "netcoreapp3.1":
                    return MajorMinorVersion.Create(8, 0);
                case "netstandard1.0":
                case "netstandard1.1":
                case "netstandard1.2":
                case "netstandard1.3":
                case "netstandard1.4":
                case "netstandard1.5":
                case "netstandard1.6":
                case "netstandard2.0":
                    return MajorMinorVersion.Create(7, 3);
                case "netstandard2.1":
                    // Language version 8.0
                    return MajorMinorVersion.Create(8, 0);
            }

            // Match for strings like net5.0, net6.0, etc.
            // net5.0 is version 9.0
            if (frameworkMoniker.StartsWith("net") &&
                frameworkMoniker.Contains(".") &&
                decimal.TryParse(frameworkMoniker[3..], NumberStyles.Any, CultureInfo.InvariantCulture, out var version) &&
                version >= 5)
            {
                switch (version)
                {
                    case 5:
                        return MajorMinorVersion.Create(9, 0);
                    case 6:
                        return MajorMinorVersion.Create(10, 0);
                    case 7:
                        return MajorMinorVersion.Create(11, 0);
                    case 8:
                        return MajorMinorVersion.Create(12, 0);
                    default:
                        // See https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version#defaults
                        Logging.Log.Warning(
                            $"Assuming language version \"{PreviewLanguageVersion}\" for project \"{Name}\" targeting " +
                            $"\"{frameworkMoniker}\". Try seeing if a new version of the \"Intent.Modules.Common.CSharp\"" +
                            " module is available or notify Intent Architect support.");

                        return PreviewLanguageVersion;
                }
            }

            throw new Exception($"Could not determine default language version for framework moniker \"{frameworkMoniker}\"");
        }

        internal static MajorMinorVersion GetMaxNetAppVersion(string[] supportedFrameworks)
        {
            if (TryGetMaxNetAppVersion(supportedFrameworks, out var majorMinorVersion))
            {
                return majorMinorVersion;
            }

            throw new InvalidOperationException($"Project's frameworks ({string.Join('.', supportedFrameworks)}) do not target .NET 5 or greater.");
        }

        internal static bool TryGetMaxNetAppVersion(string[] supportedFrameworks, out MajorMinorVersion majorMinorVersion)
        {
            majorMinorVersion = supportedFrameworks
                .Select(frameworkMoniker =>
                {
                    // Match for strings like net5.0, net6.0, etc.
                    if (!frameworkMoniker.StartsWith("net") ||
                        !frameworkMoniker.Contains(".") ||
                        !decimal.TryParse(frameworkMoniker[3..], NumberStyles.Any, CultureInfo.InvariantCulture, out var version) ||
                        version < 5)
                    {
                        return default;
                    }

                    var parts = frameworkMoniker[3..].Split('.');
                    if (parts.Length != 2 ||
                        !int.TryParse(parts[0], out var major) ||
                        !int.TryParse(parts[1], out var minor))
                    {
                        return default;
                    }

                    return MajorMinorVersion.Create(major, minor);
                })
                .Where(x => x != default)
                .DefaultIfEmpty()
                .Max();

            return majorMinorVersion != default;
        }
    }
}