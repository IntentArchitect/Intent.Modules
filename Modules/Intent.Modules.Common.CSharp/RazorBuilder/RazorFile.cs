#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.Common.CSharp.RazorBuilder
{
    internal class RazorFile<TTemplateModel> : RazorFileNodeBase<RazorFile<TTemplateModel>, IRazorFile>, IRazorFile
    {
        protected bool IsBuilt;
        private bool _afterBuildRun;
        private string? _fileExtension;
        private string? _relativeLocation;
        private string? _namespace;
        private string? _fileName;
        private OverwriteBehaviour _overwriteBehaviour = OverwriteBehaviour.Always;

        private readonly string _className;
        private readonly HashSet<string> _knownUsings = new();
        private readonly List<(Action Invoke, int Order)> _configurations = new();
        private readonly List<(Action Invoke, int Order)> _configurationsAfter = new();

        public RazorFile(RazorTemplateBase<TTemplateModel> template, string className) : base(null!)
        {
            Template = template as IRazorFileTemplate ?? throw new InvalidOperationException($"{nameof(template)} must implement {nameof(IRazorFileTemplate)}");
            File = this;
            RazorFile = this;
            _className = className;
            _namespace = template.GetNamespace();
            _relativeLocation = template.GetFolderPath();
        }

        public IRazorFile WithFileExtension(string fileExtension)
        {
            _fileExtension = fileExtension;
            return this;
        }

        public IRazorFile WithFileName(string fileName)
        {
            _fileName = fileName;
            return this;
        }

        public IRazorFile WithRelativeLocation(string relativeLocation)
        {
            _relativeLocation = relativeLocation;
            return this;
        }

        public IRazorFile WithNamespace(string @namespace)
        {
            _namespace = @namespace;
            return this;
        }

        public IRazorFile WithOverwriteBehaviour(OverwriteBehaviour overwriteBehaviour)
        {
            _overwriteBehaviour = overwriteBehaviour;
            return this;
        }

        ITemplateFileConfig IFileBuilderBase<IRazorFile>.GetConfig() => GetConfig();

        public RazorFileConfig GetConfig()
        {
            return new RazorFileConfig(
                className: _className,
                @namespace: _namespace ?? throw new InvalidOperationException(
                    $"Could not automatically determine namespace for file, either ensure the template extends " +
                    $"{nameof(CSharpTemplateBase<TTemplateModel>)} or manually call the {nameof(WithNamespace)} method."),
                relativeLocation: _relativeLocation ?? throw new InvalidOperationException(
                    $"Could not automatically determine relative location for file, either ensure the template extends " +
                    $"{nameof(IntentTemplateBase<TTemplateModel>)} or manually call the {nameof(WithRelativeLocation)} method."),
                overwriteBehaviour: _overwriteBehaviour,
                fileName: _fileName,
                fileExtension: _fileExtension,
                dependsUpon: default);
        }

        ICSharpTemplate ICSharpFile.Template => Template;
        public IRazorFileTemplate Template { get; }

        public IList<IRazorDirective> Directives { get; } = new List<IRazorDirective>();

        public IRazorFile AddUsing(string @namespace)
        {
            if (!_knownUsings.Add(@namespace))
            {
                return this;
            }

            Template.AddUsing(@namespace);
            Directives.Add(new RazorDirective("using", new CSharpStatement(@namespace.Replace("using ", ""))));
            return this;
        }

        public string GetModelType<TModel>(TModel model) where TModel : IMetadataModel, IHasName
        {
            if (Template == null)
            {
                throw new InvalidOperationException("Cannot add property with model. Please add the template as an argument to this CSharpFile's constructor.");
            }

            var type = model switch
            {
                IHasTypeReference hasType => Template.GetTypeName(hasType.TypeReference),
                ITypeReference typeRef => Template.GetTypeName(typeRef),
                _ => throw new ArgumentException($"model {model.Name} must implement either IHasTypeReference or ITypeReference", nameof(model))
            };
            return type;
        }

        public IRazorFile Configure(Action<IRazorFile> configure)
        {
            _configurations.Add((() => configure(this), int.MinValue));
            return this;
        }

        public override string ToString()
        {
            return GetText("");
        }

        public override string GetText(string indentation)
        {
            if (!IsBuilt)
            {
                throw new Exception("RazorFile has not been built. Call .Build() before this invocation.");
            }
            var sb = new StringBuilder();

            var orderedDirectives = Directives.OrderBy(x => x.Order).ToList();
            for (var index = 0; index < orderedDirectives.Count; index++)
            {
                var directive = orderedDirectives[index];
                sb.AppendLine(directive.ToString());

                if (index == orderedDirectives.Count - 1)
                {
                    sb.AppendLine();
                }
            }

            foreach (var node in ChildNodes)
            {
                sb.Append(node.GetText(""));
            }

            return sb.ToString();
        }

        public IRazorFile OnBuild(Action<IRazorFile> configure, int order = 0)
        {
            if (IsBuilt)
            {
                throw new Exception("This file has already been built. " +
                                    "Consider registering your configuration in the AfterBuild(...) method.");
            }

            _configurations.Add((() => configure(this), order));
            return this;
        }

        public IRazorFile AfterBuild(Action<IRazorFile> configure, int order = 0)
        {
            if (_afterBuildRun)
            {
                throw new Exception("The AfterBuild step has already been run for this file.");
            }

            _configurationsAfter.Add((() => configure(this), order));
            return this;
        }

        #region IFileBuilderBase implementation

        IReadOnlyCollection<(Action Invoke, int Order)> IFileBuilderBase.GetConfigurationDelegates()
        {
            if (_configurations.Count == 0)
            {
                return [];
            }

            var toReturn = _configurations.ToArray();
            _configurations.Clear();
            return toReturn;
        }

        void IFileBuilderBase.MarkCompleteBuildAsDone()
        {
            IsBuilt = true;
        }

        IReadOnlyCollection<(Action Invoke, int Order)> IFileBuilderBase.GetConfigurationAfterDelegates()
        {
            if (_configurationsAfter.Count == 0)
            {
                return [];
            }

            var toReturn = _configurationsAfter.ToList();
            _configurationsAfter.Clear();
            return toReturn;
        }

        void IFileBuilderBase.MarkAfterBuildAsDone()
        {
            if (_configurations.Any())
            {
                throw new Exception("Pending configurations have not been executed. Please contact support@intentarchitect.com for assistance.");
            }

            _afterBuildRun = true;
        }

        #endregion
    }
}