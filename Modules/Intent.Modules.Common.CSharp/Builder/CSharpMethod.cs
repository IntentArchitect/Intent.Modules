using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Common.CSharp.Builder
{
    public class CSharpMethod
    {
        private readonly IList<CSharpParameter> _parameters = new List<CSharpParameter>();
        private readonly IList<string> _statements = new List<string>();
        public string ReturnType { get; private set; }
        public string Name { get; private set; }

        public string AccessModifier { get; private set; } = "public ";
        public string OverrideModifier { get; private set; } = "";
        public CSharpMethod(string returnType, string name)
        {
            ReturnType = returnType;
            Name = name;
        }

        public CSharpParameter AddParameter(string type, string name)
        {
            var param = new CSharpParameter(type, name);
            _parameters.Add(param);
            return param;
        }

        public CSharpMethod AddStatement(string statement)
        {
            _statements.Add(statement);
            return this;
        }

        public CSharpMethod Protected()
        {
            AccessModifier = "protected ";
            return this;
        }
        public CSharpMethod Private()
        {
            AccessModifier = "private ";
            return this;
        }

        public CSharpMethod Override()
        {
            OverrideModifier = "override ";
            return this;
        }

        public CSharpMethod New()
        {
            OverrideModifier = "new ";
            return this;
        }

        public CSharpMethod Virtual()
        {
            OverrideModifier = "virtual ";
            return this;
        }

        public string ToString(string indentation)
        {
            return $@"{indentation}{AccessModifier}{OverrideModifier}{ReturnType} {Name}({string.Join(", ", _parameters.Select(x => x.ToString()))})
{indentation}{{{(_statements.Any() ? $@"
{indentation}    {string.Join($@"
{indentation}    ", _statements)}" : string.Empty)}
{indentation}}}";
        }
    }

    public class CSharpParameter
    {
        public string Type { get; }
        public string Name { get; }

        public CSharpParameter(string type, string name)
        {
            Type = type;
            Name = name;
        }

        public override string ToString()
        {
            return $@"{Type} {Name}";
        }
    }
}