using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.Common.CSharp.RazorBuilder
{
    public class RazorFile : RazorFileNodeBase<RazorFile>, ICSharpFile
    {
        protected bool _isBuilt;
        private bool _afterBuildRun;

        private readonly HashSet<string> _knownUsings = new();
        private readonly List<(Action Invoke, int Order)> _configurations = new();
        private readonly List<(Action Invoke, int Order)> _configurationsAfter = new();
        public RazorFile(ICSharpTemplate template) : base(null)
        {
            Template = template;
            File = this;
            RazorFile = this;
        }

        public ICSharpTemplate Template { get; protected set; }
        public IList<RazorDirective> Directives { get; } = new List<RazorDirective>();

        public RazorFile AddUsing(string @namespace)
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

        public RazorFile Configure(Action<RazorFile> configure)
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
            if (!_isBuilt)
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

        public RazorFile OnBuild(Action<RazorFile> configure, int order = 0)
        {
            if (_isBuilt)
            {
                throw new Exception("This file has already been built. " +
                                    "Consider registering your configuration in the AfterBuild(...) method.");
            }

            _configurations.Add((() => configure(this), order));
            return this;
        }

        public RazorFile AfterBuild(Action<RazorFile> configure, int order = 0)
        {
            if (_afterBuildRun)
            {
                throw new Exception("The AfterBuild step has already been run for this file.");
            }

            _configurationsAfter.Add((() => configure(this), order));
            return this;
        }

        internal IReadOnlyCollection<(Action Invoke, int Order)> GetConfigurationDelegates()
        {
            if (_configurations.Count == 0)
            {
                return [];
            }

            var toReturn = _configurations.ToArray();
            _configurations.Clear();
            return toReturn;
        }

        internal RazorFile MarkCompleteBuildAsDone()
        {
            _isBuilt = true;
            return this;
        }

        internal IReadOnlyCollection<(Action Invoke, int Order)> GetConfigurationAfterDelegates()
        {
            if (_configurationsAfter.Count == 0)
            {
                return Array.Empty<(Action Invoke, int Order)>();
            }

            var toReturn = _configurationsAfter.ToList();
            _configurationsAfter.Clear();
            return toReturn;
        }

        internal RazorFile MarkAfterBuildAsDone()
        {
            if (_configurations.Any())
            {
                throw new Exception("Pending configurations have not been executed. Please contact support@intentarchitect.com for assistance.");
            }

            _afterBuildRun = true;
            return this;
        }
    }
}