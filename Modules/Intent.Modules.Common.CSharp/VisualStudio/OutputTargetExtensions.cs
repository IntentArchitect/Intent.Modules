using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
    public interface ICSharpProject : IOutputTarget
    {
        string LanguageVersion { get; }
        bool NullableEnabled { get; }
        bool IsNetCore2App { get; }
        bool IsNetCore3App { get; }
        bool IsNet4App { get; }
        bool IsNet5App { get; }
        bool IsNullableAwareContext();
    }

    class CSharpProject : ICSharpProject
    {
        private readonly IOutputTarget _outputTarget;

        public CSharpProject(IOutputTarget outputTarget)
        {
            _outputTarget = outputTarget;
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
        public IEnumerable<string> TargetFrameworks { get; }

        public bool IsNullableAwareContext()
        {
            if (LanguageVersion == "latest" || (decimal.TryParse(LanguageVersion, out var langVersion) && langVersion >= 8.0m))
            {
                return NullableEnabled;
            }

            if (LanguageVersion == "default" && TargetFrameworks.All(x => x.StartsWith("netcoreapp3") || x.Equals("netstandard2.1") || x.Equals("net5.0")))
            {
                return NullableEnabled;
            }

            return false;
        }

        public bool IsNetCore2App => GetSupportedFrameworks().Any(x => x.StartsWith("netcoreapp2"));

        public bool IsNetCore3App => GetSupportedFrameworks().Any(x => x.StartsWith("netcoreapp3"));

        public bool IsNet4App => GetSupportedFrameworks().Any(x => x.StartsWith("net4"));

        public bool IsNet5App => GetSupportedFrameworks().Any(x => x.StartsWith("net5"));

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

        public ITemplateTargetInfo AsTarget(string subLocation = null)
        {
            return _outputTarget.AsTarget(subLocation);
        }

        public bool HasTemplateInstances(string templateId)
        {
            return _outputTarget.HasTemplateInstances(templateId);
        }

        public bool HasTemplateInstances(string templateId, Func<ITemplate, bool> predicate)
        {
            return _outputTarget.HasTemplateInstances(templateId, predicate);
        }
    }

    public static class OutputTargetExtensions
    {
        public static ICSharpProject GetProject(this IOutputTarget outputTarget)
        {
            return new CSharpProject(outputTarget.GetTargetPath()[0]);
        }
    }
}
