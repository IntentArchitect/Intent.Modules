using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.Common.CSharp.Razor
{
    public class RazorFile : RazorFileNodeBase<RazorFile>, ICSharpFile
    {
        private Action<RazorFile> _configure;
        private readonly IList<(Action Invoke, int Order)> _configurations = new List<(Action Invoke, int Order)>();
        private readonly IList<(Action Invoke, int Order)> _configurationsAfter = new List<(Action Invoke, int Order)>();
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
            _configure = configure;
            return this;
        }

        public override string ToString()
        {
            return GetText("");
        }

        protected bool _isBuilt = false;
        private bool _afterBuildRun;
        public virtual RazorFile Build()
        {
            // TODO: Need to address same concerns as the CSharpFileBuilderFactoryExtension does for the CSharpFile:
            _configure(this);
            while (_configurations.Count > 0)
            {
                var configuration = _configurations.MinBy(x => x.Order);
                configuration.Invoke();
                _configurations.Remove(configuration);
            }
            _isBuilt = true;
            while (_configurationsAfter.Count > 0)
            {
                var configuration = _configurationsAfter.MinBy(x => x.Order);
                configuration.Invoke();
                _configurationsAfter.Remove(configuration);
            }
            return this;
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
    }

    public interface IRazorFileNode : ICSharpCodeContext
    {
        string GetText(string indentation);
        void AddChildNode(IRazorFileNode node);
        void InsertChildNode(int index, IRazorFileNode node);
        IList<IRazorFileNode> ChildNodes { get; }
        IRazorFileNode Parent { get; internal set; }
    }

    public abstract class RazorFileNodeBase<T> : CSharpMetadataBase<T>, IRazorFileNode 
        where T : RazorFileNodeBase<T>
    {
        protected RazorFileNodeBase(RazorFile file)
        {
            File = file;
            RazorFile = file;
        }

        public RazorFile RazorFile { get; protected set; }

        public IList<IRazorFileNode> ChildNodes { get; } = new List<IRazorFileNode>();

        public T AddHtmlElement(string name, Action<HtmlElement> configure = null)
        {
            var htmlElement = new HtmlElement(name, RazorFile);
            AddChildNode(htmlElement);
            configure?.Invoke(htmlElement);
            return (T)this;
        }

        public T AddHtmlElement(HtmlElement htmlElement)
        {
            AddChildNode(htmlElement);
            return (T)this;
        }

        public T AddEmptyLine()
        {
            AddChildNode(new EmptyLine(RazorFile));
            return (T)this;
        }

        public T AddCodeBlock(CSharpStatement expression, Action<RazorCodeDirective> configure = null)
        {
            var razorCodeBlock = new RazorCodeDirective(expression, RazorFile);
            AddChildNode(razorCodeBlock);
            configure?.Invoke(razorCodeBlock);
            return (T)this;
        }

        public abstract string GetText(string indentation);


        public void AddChildNode(IRazorFileNode node)
        {
            InsertChildNode(ChildNodes.Count, node);
        }

        public void InsertChildNode(int index, IRazorFileNode node)
        {
            node.Parent = this;
            ChildNodes.Insert(index, node);
        }

        public new IRazorFileNode Parent { get; set; }
    }

    public class RazorDirective
    {
        public string Keyword { get; }
        public ICSharpExpression Expression { get; }

        public int Order { get; set; }

        public RazorDirective(string keyword, ICSharpExpression expression = null)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(keyword));
            }

            Keyword = keyword;
            Expression = expression;
            switch (keyword)
            {
                case "page":
                    Order = 0;
                    break;
                case "using":
                    Order = 1;
                    break;
                case "inject":
                    Order = 2;
                    break;
                default:
                    Order = 0;
                    break;
            }
        }

        public override string ToString()
        {
            return $"@{Keyword}{(Expression != null ? $" {Expression}" : "")}";
        }
    }


    public class RazorCodeDirective : RazorFileNodeBase<RazorCodeDirective>, IRazorFileNode
    {
        public ICSharpExpression Expression { get; set; }
        public RazorCodeDirective(ICSharpExpression expression, RazorFile file) : base(file)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public override string GetText(string indentation)
        {
            return $@"{indentation}@{Expression.GetText(indentation)?.TrimStart()} {{
{string.Join("", ChildNodes.Select(x => x.GetText($"{indentation}    ")))}
{indentation}}}
";
        }
    }

    public class RazorCodeBlock : RazorFileNodeBase<RazorCodeBlock>, IRazorFileNode, IBuildsCSharpMembers
    {
        public ICSharpExpression Expression { get; set; }
        public IList<ICodeBlock> Declarations { get; } = new List<ICodeBlock>();

        public RazorCodeBlock(RazorFile file) : base(file)
        {
        }

        public IBuildsCSharpMembers AddField(string type, string name, Action<CSharpField> configure = null)
        {
            var field = new CSharpField(type, name)
            {
                BeforeSeparator = CSharpCodeSeparatorType.NewLine,
                AfterSeparator = CSharpCodeSeparatorType.NewLine
            };
            Declarations.Add(field);
            configure?.Invoke(field);
            return this;
        }

        public IBuildsCSharpMembers AddProperty(string type, string name, Action<CSharpProperty> configure = null)
        {
            var property = new CSharpProperty(type, name, RazorFile)
            {
                BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
                AfterSeparator = CSharpCodeSeparatorType.EmptyLines
            };
            Declarations.Add(property);
            configure?.Invoke(property);
            return this;
        }

        public IBuildsCSharpMembers AddMethod(string type, string name, Action<CSharpClassMethod> configure = null)
        {
            var method = new CSharpClassMethod(type, name, RazorFile)
            {
                BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
                AfterSeparator = CSharpCodeSeparatorType.EmptyLines
            };
            Declarations.Add(method);
            configure?.Invoke(method);
            return this;
        }

        public IBuildsCSharpMembers AddClass(string name, Action<CSharpClass> configure = null)
        {
            var @class = new CSharpClass(name, RazorFile)
            {
                BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
                AfterSeparator = CSharpCodeSeparatorType.EmptyLines
            };
            Declarations.Add(@class);
            configure?.Invoke(@class);
            return this;
        }

        public override string GetText(string indentation)
        {
            return $@"{indentation}@code {{
{string.Join("", ChildNodes.Select(x => x.GetText($"{indentation}    ")))}{string.Join(@"
", Declarations.ConcatCode(indentation + "    "))}
{indentation}}}
";
        }
    }

    public class EmptyLine : RazorFileNodeBase<HtmlElement>
    {
        public EmptyLine(RazorFile file) : base(file)
        {
        }
        public override string GetText(string indentation)
        {
            return Environment.NewLine;
        }
    }

    public class HtmlElement : RazorFileNodeBase<HtmlElement>, IRazorFileNode
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public Dictionary<string, HtmlAttribute> Attributes { get; set; } = new Dictionary<string, HtmlAttribute>();

        public HtmlElement(string name, RazorFile file) : base(file)
        {
            Name = name;
        }

        public HtmlElement AddAttribute(string name, string value = null)
        {
            Attributes.Add(name, new HtmlAttribute(name, value));
            return this;
        }

        public HtmlElement AddClass(string className)
        {
            if (!Attributes.TryGetValue("class", out var classAttr))
            {
                Attributes.Add("class", new HtmlAttribute("class", className));
            }
            else
            {
                if (classAttr.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).All(x => x != className))
                {
                    classAttr.Value += $" {classAttr}";
                }
            }
            return this;
        }

        public HtmlElement AddAttributeIfNotEmpty(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return this;
            }
            return AddAttribute(name, value);
        }

        public HtmlElement SetAttribute(string name, string value = null)
        {
            Attributes[name] = new HtmlAttribute(name, value);
            return this;
        }

        public HtmlAttribute GetAttribute(string name)
        {
            return Attributes.TryGetValue(name, out var attribute) ? attribute : null;
        }

        public bool HasAttribute(string name, string value = null)
        {
            return Attributes.TryGetValue(name, out var attribute) && (value == null || attribute.Value == value);
        }

        public HtmlElement WithText(string text)
        {
            Text = text;
            return this;
        }


        public HtmlElement AddAbove(IRazorFileNode node)
        {
            Parent.InsertChildNode(Parent.ChildNodes.IndexOf(this), node);
            return this;
        }

        public HtmlElement AddAbove(params IRazorFileNode[] nodes)
        {
            foreach (var node in nodes)
            {
                AddAbove(node);
            }
            return this;
        }

        public HtmlElement AddBelow(IRazorFileNode node)
        {
            Parent.InsertChildNode(Parent.ChildNodes.IndexOf(this) + 1, node);
            return this;
        }

        public HtmlElement AddBelow(params IRazorFileNode[] nodes)
        {
            foreach (var node in nodes.Reverse())
            {
                AddBelow(node);
            }
            return this;
        }

        public void Remove()
        {
            Parent.ChildNodes.Remove(this);
        }

        public override string GetText(string indentation)
        {
            var sb = new StringBuilder();
            var requiresEndTag = !string.IsNullOrWhiteSpace(Text) || ChildNodes.Any() || Name is "script";
            sb.Append($"{indentation}<{Name}{FormatAttributes(indentation)}{(!requiresEndTag ? " /" : "")}>{(requiresEndTag && (ChildNodes.Any() || Attributes.Count > 1) ? Environment.NewLine : "")}");

            foreach (var e in ChildNodes)
            {
                sb.Append(e.GetText($"{indentation}    "));
            }

            if (!string.IsNullOrWhiteSpace(Text))
            {
                if (ChildNodes.Any() || Attributes.Count > 1)
                {
                    sb.AppendLine($"{indentation}    {Text}");
                }
                else
                {
                    sb.Append(Text);
                }
            }

            if (requiresEndTag)
            {
                if (Attributes.Count > 1 || ChildNodes.Any())
                {
                    sb.Append($"{(!ChildNodes.Any() && Attributes.Count <= 1 ? Environment.NewLine : "")}{indentation}</{Name}>");
                }
                else
                {
                    sb.Append($"</{Name}>");
                }
            }

            sb.AppendLine();

            return sb.ToString();
        }

        private string FormatAttributes(string indentation)
        {
            var separateLines = Name is not "link" and not "meta";
            return string.Join(separateLines 
                    ? $"{Environment.NewLine}{indentation}{new string(' ', Name.Length + 1)}" 
                    : "", 
                Attributes.Values.Select(attribute => $" {attribute.Name}{(attribute.Value != null ? $"=\"{attribute.Value}\"" : "")}"));
        }

        public override string ToString()
        {
            return GetText("");
        }
    }

    public record HtmlAttribute(string Name, string Value)
    {
        public string Value { get; set; } = Value;
    }
}