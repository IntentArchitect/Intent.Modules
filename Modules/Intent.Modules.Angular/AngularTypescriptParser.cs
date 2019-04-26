using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Plugins;
using Intent.Plugins.FactoryExtensions;

namespace Intent.Modules.Angular
{
    [Description("Angular Typescript Parser")]
    public class AngularTypescriptParser : FactoryExtensionBase, ITransformOutput
    {
        public override string Id => "Intent.Angular.Parser";

        public void Transform(IOutputFile output)
        {
            if (output.Template is ITypescriptFile)
            {
                var tsFile = (ITypescriptFile)output;
                var updater = new AngularTypescriptMerger(tsFile);
            }
        }
    }

    public class AngularTypescriptMerger
    {
        private readonly ITypescriptFile _tsFile;

        public AngularTypescriptMerger(ITypescriptFile tsFile)
        {
            _tsFile = tsFile;
        }

        public string Merge(string content)
        {


            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var currentImports = GetImports(lines);




            return string.Join(Environment.NewLine, lines);
        }

        private IEnumerable<ITypescriptFileImport> GetImports(IEnumerable<string> lines)
        {
            var imports = new List<ITypescriptFileImport>();
            foreach (var line in lines)
            {
                if (line.StartsWith("import "))
                {
                    imports.Add(new TypescriptFileImport(line));
                }
            }
            return imports;
        }
    }

    public interface ITypescriptFileImport
    {
        string Line { get; }
        string Classes { get; }
        string Location { get; }
    }

    class TypescriptFileImport : ITypescriptFileImport
    {
        public TypescriptFileImport(string line)
        {
            Line = line;
            Classes = line.Slice(line.IndexOf('{') + 1, line.IndexOf('}') - 1).Trim();
            Location = line.Substring(line.IndexOf(" from ", StringComparison.Ordinal) + 4)
                .Replace("\'", "").Replace("\"", "").Replace(";", "");
        }

        public string Line { get; }
        public string Classes { get; }
        public string Location { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypescriptFileImport)obj);
        }

        protected bool Equals(TypescriptFileImport other)
        {
            return string.Equals(Classes, other.Classes) && string.Equals(Location, other.Location);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Classes != null ? Classes.GetHashCode() : 0) * 397) ^ (Location != null ? Location.GetHashCode() : 0);
            }
        }
    }

    public interface ITypescriptFile
    {
        IEnumerable<ITypescriptFileImport> RequiredImports { get; }
    }
}