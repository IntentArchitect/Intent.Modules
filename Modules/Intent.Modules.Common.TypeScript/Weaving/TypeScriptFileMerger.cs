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

            // ------------------- CLASSES -------------------- //
            {
                var existingClasses = _existingFile.ClassDeclarations();
                var outputClasses = _outputFile.ClassDeclarations();

                var toAdd = outputClasses.Except(existingClasses).ToList();
                var toUpdate = existingClasses.Where(x => !x.IsIgnored()).Intersect(outputClasses).ToList();
                var toRemove = existingClasses.Where(x => !x.IsIgnored()).Except(outputClasses).ToList();

                foreach (var existingClass in toUpdate)
                {
                    var outputClass = outputClasses.Single(x => x.Equals(existingClass));
                    MergeClasses(existingClass, outputClass);
                }

                foreach (var @class in toAdd)
                {
                    _existingFile.AddClass(@class.GetTextWithComments());
                }

                foreach (var @class in toRemove)
                {
                    @class.Remove();
                }
            }

            // ------------------- INTERFACES -------------------- //
            {
                var existingInterfaces = _existingFile.InterfaceDeclarations();
                var outputInterfaces = _outputFile.InterfaceDeclarations();

                var toAdd = outputInterfaces.Except(existingInterfaces).ToList();
                var toUpdate = existingInterfaces.Where(x => !x.IsIgnored()).Intersect(outputInterfaces).ToList();
                var toRemove = existingInterfaces.Where(x => !x.IsIgnored()).Except(outputInterfaces).ToList();

                foreach (var existingInterface in toUpdate)
                {
                    var outputInterface = outputInterfaces.Single(x => x.Equals(existingInterface));
                    MergeInterfaces(existingInterface, outputInterface);
                }

                foreach (var @interface in toAdd)
                {
                    _existingFile.AddInterface(@interface.GetTextWithComments());
                }

                foreach (var @interface in toRemove)
                {
                    @interface.Remove();
                }
            }


            return _existingFile.GetSource();
        }

        private static void MergeClasses(TypeScriptClass existingClass, TypeScriptClass outputClass)
        {
            if (!existingClass.IsMerged() &&
                (!existingClass.HasConstructor() || !existingClass.Constructor().IsIgnored()) &&
                existingClass.Methods().All(x => !x.IsIgnored()) &&
                existingClass.Properties().All(x => !x.IsIgnored()))
            {
                existingClass.ReplaceWith(outputClass.GetTextWithComments());
                return;
            }

            MergeDecorators(existingClass, outputClass);
            MergeConstructor(existingClass, outputClass);
            MergeProperties(existingClass, outputClass);
            MergeMethods(existingClass, outputClass);
        }

        private void MergeInterfaces(TypeScriptClass existingInterface, TypeScriptClass outputInterface)
        {
            if (!existingInterface.IsMerged() &&
                existingInterface.Methods().All(x => !x.IsIgnored()) &&
                existingInterface.Properties().All(x => !x.IsIgnored()))
            {
                existingInterface.ReplaceWith(outputInterface.GetTextWithComments());
                return;
            }

            MergeDecorators(existingInterface, outputInterface);
            MergeProperties(existingInterface, outputInterface);
            MergeMethods(existingInterface, outputInterface);
        }

        private static void MergeDecorators(TypeScriptClass existingClass, TypeScriptClass outputClass)
        {
            var existingDecorators = existingClass.Decorators();
            var outputDecorators = outputClass.Decorators();
            var toAdd = outputDecorators.Except(existingDecorators).ToList();
            var toUpdate = existingDecorators.Intersect(outputDecorators).ToList();
            var toRemove = existingDecorators.Except(outputDecorators).ToList();

            foreach (var decorator in toUpdate)
            {
                var outputDecorator = outputClass.Decorators().Single(x => x.Equals(decorator));
                decorator.ReplaceWith(outputDecorator.GetTextWithComments());
            }

            foreach (var decorator in toAdd)
            {
                existingClass.AddDecorator(decorator.GetTextWithComments());
            }

            foreach (var decorator in toRemove)
            {
                if (!existingClass.IsIgnored() && !existingClass.IsMerged())
                {
                    decorator.Remove();
                }
            }
        }

        private static void MergeConstructor(TypeScriptClass existingClass, TypeScriptClass outputClass)
        {
            if (existingClass.HasConstructor())
            {
                var constructor = existingClass.Constructor();
                if (constructor.IsIgnored())
                {
                    return;
                }
                if (outputClass.HasConstructor())
                {
                    constructor.ReplaceWith(outputClass.Constructor().GetTextWithComments());
                }
                else if (!existingClass.IsMerged())
                {
                    existingClass.RemoveConstructor(constructor);
                }
            }
            else if (outputClass.HasConstructor())
            {
                existingClass.AddConstructor(outputClass.Constructor().GetTextWithComments());
            }
        }

        private static void MergeProperties(TypeScriptClass existingClass, TypeScriptClass outputClass)
        {
            var existingProperties = existingClass.Properties();
            var outputProperties = outputClass.Properties();
            var toAdd = outputProperties.Except(existingProperties).ToList();
            var toUpdate = existingProperties.Where(x => !x.IsIgnored()).Intersect(outputProperties).ToList();
            var toRemove = existingProperties.Where(x => !x.IsIgnored()).Except(outputProperties).ToList();

            foreach (var property in toUpdate)
            {
                var outputMethod = outputClass.Properties().Single(x => x.Equals(property));
                property.ReplaceWith(outputMethod.GetTextWithComments());
            }

            foreach (var property in toAdd)
            {
                existingClass.AddProperty(property.GetTextWithComments());
            }

            foreach (var property in toRemove)
            {
                if (!existingClass.IsMerged() || property.IsManaged())
                {
                    property.Remove();
                }
            }
        }

        private static void MergeMethods(TypeScriptClass existingClass, TypeScriptClass outputClass)
        {
            var existingMethods = existingClass.Methods();
            var outputMethods = outputClass.Methods();
            var toAdd = outputMethods.Except(existingMethods).ToList();
            var toUpdate = existingMethods.Where(x => !x.IsIgnored()).Intersect(outputMethods).ToList();
            var toRemove = existingMethods.Where(x => !x.IsIgnored()).Except(outputMethods).ToList();

            foreach (var method in toUpdate)
            {
                var outputMethod = outputClass.Methods().Single(x => x.Equals(method));
                method.ReplaceWith(outputMethod.GetTextWithComments());
            }

            foreach (var method in toAdd)
            {
                existingClass.AddMethod(method.GetTextWithComments());
            }

            foreach (var method in toRemove)
            {
                if (!existingClass.IsMerged() || method.IsManaged())
                {
                    method.Remove();
                }
            }
        }
    }
}