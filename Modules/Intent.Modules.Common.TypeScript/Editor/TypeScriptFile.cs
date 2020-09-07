using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptFile : TypeScriptNode
    {
        public TypeScriptFile(Node node, TypeScriptFileEditor editor) : base(node, editor)
        {
        }

        public override string GetIdentifier(Node node)
        {
            return null;
        }

        public override void AddNode(TypeScriptNode node)
        {
            if (Imports.Count > 0)
            {
                InsertAfter(Imports.Last(), node);
                return;
            }
            base.AddNode(node);
        }

        public IReadOnlyList<TypeScriptFileImport> Imports => GetChildren<TypeScriptFileImport>(); //{ get; } = new List<TypeScriptFileImport>();
        public IReadOnlyList<TypeScriptClass> Classes => Children.Where(x => x is TypeScriptClass).Cast<TypeScriptClass>().ToList();
        public IReadOnlyList<TypeScriptInterface> Interfaces => Children.Where(x => x is TypeScriptInterface).Cast<TypeScriptInterface>().ToList();

        public void AddImport(TypeScriptFileImport import)
        {
            if (Imports.Any())
            {
                var existingLocation = Imports.FirstOrDefault(x => x.Location == import.Location);
                if (existingLocation != null)
                {
                    foreach (var importType in import.Types)
                    {
                        if (!existingLocation.HasType(importType))
                        {
                            existingLocation.AddType(importType);
                        }
                    }
                }
                else
                {
                    Editor.InsertAfter(Imports.Last(), import.GetTextWithComments());
                }
            }
            else
            {
                Editor.InsertBefore(this, import.GetTextWithComments());
            }
        }

        public void AddImportIfNotExists(string className, string location)
        {
            if (Imports.Any())
            {
                if (Imports.All(x => x.Location != location && x.Types.All(t => t != className)))
                {
                    Editor.InsertAfter(Imports.Last(), $@"
import {{ {className} }} from '{location}';");
                }
            }
            else
            {
                Editor.InsertBefore(this, $@"
import {{ {className} }} from '{location}';");
            }
        }

        public void NormalizeImports()
        {
            var i = 0;
            while (i < Imports.Count)
            {
                var import = Imports[i];
                var sameLocationImports = Imports.Where(x => !x.Equals(import) && x.Location == import.Location).ToArray();
                foreach (var type in sameLocationImports.SelectMany(x => x.Types))
                {
                    if (!import.HasType(type))
                    {
                        import.AddType(type);
                    }
                }

                foreach (var toRemove in sameLocationImports)
                {
                    RemoveChild(toRemove);
                }

                i++;
            }
        }

        public override string ToString()
        {
            return GetSource();
        }

        public string GetSource()
        {
            return Editor.GetSource();
        }
    }
}