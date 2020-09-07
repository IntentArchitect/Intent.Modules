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
            if (_existingFile.GetSource().TrimStart().StartsWith("//@IntentIgnoreFile()"))
            {
                return _existingFile.GetSource();
            }

            MergeNodes(_existingFile, _outputFile);

            _existingFile.NormalizeImports();

            return _existingFile.GetSource();
        }

        private void MergeNodes(TypeScriptNode existingNode, TypeScriptNode outputNode)
        {
            MergeDecorators(existingNode, outputNode);
            
            var index = 0;
            foreach (var node in outputNode.Children)
            {
                var existing = existingNode.TryGetChild(node.Node);
                if (existing == null)
                {
                    
                    // toAdd:
                    if (existingNode.Children.Count == 0)
                    {
                        existingNode.AddNode(node);
                    }
                    else if (index == 0)
                    {
                        existingNode.InsertBefore(existingNode.Children[0], node);
                    }
                    else if (existingNode.Children.Count > index)
                    {
                        existingNode.InsertAfter(existingNode.Children[index - 1], node);
                    }
                    else
                    {
                        existingNode.InsertAfter(existingNode.Children.Last(), node);
                    }

                    index++;
                }
                else
                {
                    // toUpdate:
                    index++;
                    if (existing.IsIgnored())
                    {
                        continue;
                    }

                    if (existing.Children.All(x => !x.IsIgnored()) && !existing.IsMerged())
                    {
                        existing.ReplaceWith(node.GetTextWithComments()); // Overwrite
                        continue;
                    }
                    MergeNodes(existing, node);
                }
            }

            if (!existingNode.IsMerged())
            {
                var toRemove = existingNode.Children.Where(x => !(x is TypeScriptFileImport) && !x.IsIgnored()).Except(outputNode.Children).ToList();
                foreach (var node in toRemove)
                {
                    node.Remove();
                }
            }
        }

        private void MergeDecorators(TypeScriptNode existingNode, TypeScriptNode outputNode)
        {
            var index = 0;
            foreach (var node in outputNode.Decorators)
            {
                var existing = existingNode.TryGetDecorator(node.Node);
                if (existing == null)
                {
                    // toAdd:
                    //var text = node.GetTextWithComments();
                    if (existingNode.Decorators.Count == 0)
                    {
                        _existingFile.Editor.InsertBefore(existingNode.Node, node.GetTextWithComments());
                    }
                    else if (index == 0)
                    {
                        existingNode.InsertBefore(existingNode.Decorators[0], node);
                    }
                    else if (existingNode.Decorators.Count > index)
                    {
                        existingNode.InsertAfter(existingNode.Decorators[index - 1], node);
                    }
                    else
                    {
                        existingNode.InsertAfter(existingNode.Decorators.Last(), node);
                    }

                    index++;
                }
                else
                {
                    // toUpdate:
                    index++;
                    if (existing.IsIgnored())
                    {
                        continue;
                    }

                    if (existing.Decorators.All(x => !x.IsIgnored()) && !existing.IsMerged())
                    {
                        existing.ReplaceWith(node.GetTextWithComments()); // Overwrite
                        continue;
                    }
                    //MergeDecorators(existing, node); // maybe one day
                }
            }

            if (!existingNode.IsMerged())
            {
                var toRemove = existingNode.Decorators.Where(x => !x.IsIgnored()).Except(outputNode.Decorators).ToList();
                foreach (var node in toRemove)
                {
                    node.Remove();
                }
            }
        }


        //private void MergeFileVariables()
        //{
        //    var existingVariables = _existingFile.VariableDeclarations();
        //    var outputVariables = _outputFile.VariableDeclarations();

        //    var toAdds = outputVariables.Except(existingVariables).ToList();
        //    var toUpdates = existingVariables.Where(x => !x.IsIgnored()).Intersect(outputVariables).ToList();
        //    var toRemoves = existingVariables.Where(x => !x.IsIgnored()).Except(outputVariables).ToList();

        //    foreach (var toUpdate in toUpdates)
        //    {
        //        var outputVariable = outputVariables.Single(x => x.Equals(toUpdate));
        //        //MergeVariables(existingVariables, outputVariables);
        //        toUpdate.ReplaceWith(outputVariable.GetTextWithComments());
        //    }

        //    foreach (var toAdd in toAdds)
        //    {
        //        _existingFile.AddVariableDeclaration(toAdd.GetTextWithComments());
        //    }

        //    foreach (var toRemove in toRemoves)
        //    {
        //        toRemove.Remove();
        //    }
        //}

        //private void MergeFileExpressionStatements()
        //{
        //    var existingInstances = _existingFile.ExpressionStatements();
        //    var outputInstances = _outputFile.ExpressionStatements();

        //    var toAdds = outputInstances.Except(existingInstances).ToList();
        //    var toUpdates = existingInstances.Where(x => !x.IsIgnored()).Intersect(outputInstances).ToList();
        //    var toRemoves = existingInstances.Where(x => !x.IsIgnored()).Except(outputInstances).ToList();

        //    foreach (var toUpdate in toUpdates)
        //    {
        //        var inOutput = outputInstances.Single(x => x.Equals(toUpdate));
        //        toUpdate.ReplaceWith(inOutput.GetTextWithComments());
        //    }

        //    foreach (var toAdd in toAdds)
        //    {
        //        _existingFile.AddExpressionStatement(toAdd.GetTextWithComments());
        //    }

        //    foreach (var toRemove in toRemoves)
        //    {
        //        toRemove.Remove();
        //    }
        //}

        //private void MergeFileInterfaces()
        //{
        //    var existingInterfaces = _existingFile.InterfaceDeclarations();
        //    var outputInterfaces = _outputFile.InterfaceDeclarations();

        //    var toAdd = outputInterfaces.Except(existingInterfaces).ToList();
        //    var toUpdate = existingInterfaces.Where(x => !x.IsIgnored()).Intersect(outputInterfaces).ToList();
        //    var toRemove = existingInterfaces.Where(x => !x.IsIgnored()).Except(outputInterfaces).ToList();

        //    foreach (var existingInterface in toUpdate)
        //    {
        //        var outputInterface = outputInterfaces.Single(x => x.Equals(existingInterface));
        //        MergeInterfaces(existingInterface, outputInterface);
        //    }

        //    foreach (var @interface in toAdd)
        //    {
        //        _existingFile.AddInterface(@interface.GetTextWithComments());
        //    }

        //    foreach (var @interface in toRemove)
        //    {
        //        @interface.Remove();
        //    }
        //}

        //private void MergeFileClasses()
        //{
        //    var existingClasses = _existingFile.ClassDeclarations();
        //    var outputClasses = _outputFile.ClassDeclarations();

        //    var toAdd = outputClasses.Except(existingClasses).ToList();
        //    var toUpdate = existingClasses.Where(x => !x.IsIgnored()).Intersect(outputClasses).ToList();
        //    var toRemove = existingClasses.Where(x => !x.IsIgnored()).Except(outputClasses).ToList();

        //    foreach (var existingClass in toUpdate)
        //    {
        //        var outputClass = outputClasses.Single(x => x.Equals(existingClass));
        //        MergeClasses(existingClass, outputClass);
        //    }

        //    foreach (var @class in toAdd)
        //    {
        //        _existingFile.AddClass(@class.GetTextWithComments());
        //    }

        //    foreach (var @class in toRemove)
        //    {
        //        @class.Remove();
        //    }
        //}

        //private static void MergeClasses(TypeScriptClass existingClass, TypeScriptClass outputClass)
        //{
        //    if (!existingClass.IsMerged() &&
        //        (!existingClass.HasConstructor() || !existingClass.Constructor().IsIgnored()) &&
        //        existingClass.Methods().All(x => !x.IsIgnored()) &&
        //        existingClass.Properties().All(x => !x.IsIgnored()))
        //    {
        //        existingClass.ReplaceWith(outputClass.GetTextWithComments());
        //        return;
        //    }

        //    MergeDecorators(existingClass, outputClass);
        //    MergeConstructor(existingClass, outputClass);
        //    MergeProperties(existingClass, outputClass);
        //    MergeMethods(existingClass, outputClass);
        //}

        //private void MergeInterfaces(TypeScriptClass existingInterface, TypeScriptClass outputInterface)
        //{
        //    if (!existingInterface.IsMerged() &&
        //        existingInterface.Methods().All(x => !x.IsIgnored()) &&
        //        existingInterface.Properties().All(x => !x.IsIgnored()))
        //    {
        //        existingInterface.ReplaceWith(outputInterface.GetTextWithComments());
        //        return;
        //    }

        //    MergeDecorators(existingInterface, outputInterface);
        //    MergeProperties(existingInterface, outputInterface);
        //    MergeMethods(existingInterface, outputInterface);
        //}

        //private static void MergeDecorators(TypeScriptClass existingClass, TypeScriptClass outputClass)
        //{
        //    var existingDecorators = existingClass.Decorators();
        //    var outputDecorators = outputClass.Decorators();
        //    var toAdd = outputDecorators.Except(existingDecorators).ToList();
        //    var toUpdate = existingDecorators.Intersect(outputDecorators).ToList();
        //    var toRemove = existingDecorators.Except(outputDecorators).ToList();

        //    foreach (var decorator in toUpdate)
        //    {
        //        var outputDecorator = outputClass.Decorators().Single(x => x.Equals(decorator));
        //        decorator.ReplaceWith(outputDecorator.GetTextWithComments());
        //    }

        //    foreach (var decorator in toAdd)
        //    {
        //        existingClass.AddDecorator(decorator.GetTextWithComments());
        //    }

        //    foreach (var decorator in toRemove)
        //    {
        //        if (!existingClass.IsIgnored() && !existingClass.IsMerged())
        //        {
        //            decorator.Remove();
        //        }
        //    }
        //}

        //private static void MergeConstructor(TypeScriptClass existingClass, TypeScriptClass outputClass)
        //{
        //    if (existingClass.HasConstructor())
        //    {
        //        var constructor = existingClass.Constructor();
        //        if (constructor.IsIgnored())
        //        {
        //            return;
        //        }
        //        if (outputClass.HasConstructor())
        //        {
        //            constructor.ReplaceWith(outputClass.Constructor().GetTextWithComments());
        //        }
        //        else if (!existingClass.IsMerged())
        //        {
        //            existingClass.RemoveConstructor(constructor);
        //        }
        //    }
        //    else if (outputClass.HasConstructor())
        //    {
        //        existingClass.AddConstructor(outputClass.Constructor().GetTextWithComments());
        //    }
        //}

        //private static void MergeProperties(TypeScriptClass existingClass, TypeScriptClass outputClass)
        //{
        //    var existingProperties = existingClass.Properties();
        //    var outputProperties = outputClass.Properties();
        //    var toAdd = outputProperties.Except(existingProperties).ToList();
        //    var toUpdate = existingProperties.Where(x => !x.IsIgnored()).Intersect(outputProperties).ToList();
        //    var toRemove = existingProperties.Where(x => !x.IsIgnored()).Except(outputProperties).ToList();

        //    foreach (var property in toUpdate)
        //    {
        //        var outputMethod = outputClass.Properties().Single(x => x.Equals(property));
        //        property.ReplaceWith(outputMethod.GetTextWithComments());
        //    }

        //    foreach (var property in toAdd)
        //    {
        //        existingClass.AddProperty(property.GetTextWithComments());
        //    }

        //    foreach (var property in toRemove)
        //    {
        //        if (!existingClass.IsMerged() || property.IsManaged())
        //        {
        //            property.Remove();
        //        }
        //    }
        //}

        //private static void MergeMethods(TypeScriptClass existingClass, TypeScriptClass outputClass)
        //{
        //    var existingMethods = existingClass.Methods();
        //    var outputMethods = outputClass.Methods();
        //    var toAdd = outputMethods.Except(existingMethods).ToList();
        //    var toUpdate = existingMethods.Where(x => !x.IsIgnored()).Intersect(outputMethods).ToList();
        //    var toRemove = existingMethods.Where(x => !x.IsIgnored()).Except(outputMethods).ToList();

        //    foreach (var method in toUpdate)
        //    {
        //        var outputMethod = outputClass.Methods().Single(x => x.Equals(method));
        //        method.ReplaceWith(outputMethod.GetTextWithComments());
        //    }

        //    foreach (var method in toAdd)
        //    {
        //        existingClass.AddMethod(method.GetTextWithComments());
        //    }

        //    foreach (var method in toRemove)
        //    {
        //        if (!existingClass.IsMerged() || method.IsManaged())
        //        {
        //            method.Remove();
        //        }
        //    }
        //}
    }
}