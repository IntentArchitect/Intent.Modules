using System.Linq;
using Intent.Modules.Common.TypeScript.Editor;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Weaving
{
    public class TypeScriptFileMerger
    {
        private readonly TypeScriptFile _existingFile;
        private readonly TypeScriptFile _outputFile;

        public TypeScriptFileMerger(TypeScriptFile existingFile, TypeScriptFile outputFile)
        {
            _existingFile = existingFile;
            _outputFile = outputFile;
        }

        public string GetMergedFile()
        {
            foreach (var import in _outputFile.Imports())
            {
                if (!_existingFile.ImportExists(import))
                {
                    _existingFile.AddImport(import);
                }
            }

            var existingClasses = _existingFile.ClassDeclarations();
            var outputClasses = _outputFile.ClassDeclarations();

            var toAdd = outputClasses.Except(existingClasses).ToList();
            var toUpdate = existingClasses.Where(x => !x.IsIgnored()).Intersect(outputClasses).ToList();
            var toRemove = existingClasses.Where(x => !x.IsIgnored()).Except(outputClasses).ToList();

            foreach (var @class in toAdd)
            {
                _existingFile.AddClass(@class.GetTextWithComments());
            }

            foreach (var @class in toRemove)
            {
                @class.Remove();
            }

            foreach (var existingClass in toUpdate)
            {
                var outputClass = outputClasses.Single(x => x.Equals(existingClass));
                MergeClasses(existingClass, outputClass);
            }

            return _existingFile.GetSource();
        }

        public void MergeClasses(TypeScriptClass existingClass, TypeScriptClass outputClass)
        {
            var existingMethods = existingClass.Methods();
            var outputMethods = outputClass.Methods();

            if (existingMethods.All(x => !x.IsIgnored()))
            {
                existingClass.ReplaceWith(outputClass.GetTextWithComments());
                return;
            }

            var toAdd = outputMethods.Except(existingMethods).ToList();
            var toUpdate = existingMethods.Where(x => !x.IsIgnored()).Intersect(outputMethods).ToList();
            var toRemove = existingMethods.Where(x => !x.IsIgnored()).Except(outputMethods).ToList();

            foreach (var method in toAdd)
            {
                existingClass.AddMethod(method.GetTextWithComments());
            }

            foreach (var method in toRemove)
            {
                method.Remove();
            }

            foreach (var method in toUpdate)
            {
                var outputMethod = outputClass.Methods().Single(x => x.Equals(method));
                method.ReplaceWith(outputMethod.GetTextWithComments());
            }
        }
    }
}