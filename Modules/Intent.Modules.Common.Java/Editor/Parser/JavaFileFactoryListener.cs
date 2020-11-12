using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using System.Net.Mime;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor.Parser
{
    public class JavaFileFactoryListener : JavaParserBaseListener
    {
        public JavaFileFactoryListener(JavaFile javaFile)
        {
            File = javaFile;
            _nodeStack.Push(new JavaNodeContext(File));
        }

        public readonly JavaFile File;

        private readonly Stack<JavaNodeContext> _nodeStack = new Stack<JavaNodeContext>();
        private JavaNodeContext Current => _nodeStack.Peek();

        public override void EnterClassDeclaration(JavaParser.ClassDeclarationContext context)
        {
            _nodeStack.Push(InsertOrUpdateNode((JavaParser.TypeDeclarationContext)context.Parent, () => new JavaClass((JavaParser.TypeDeclarationContext) context.Parent, Current.Node)));
            ApplyAnnotations(Current);
        }

        public override void ExitClassDeclaration(JavaParser.ClassDeclarationContext context)
        {
            _nodeStack.Pop();
        }

        public override void EnterInterfaceDeclaration(JavaParser.InterfaceDeclarationContext context)
        {
            _nodeStack.Push(InsertOrUpdateNode(context, () => new JavaInterface(context, Current.Node)));
            ApplyAnnotations(Current);
        }

        public override void ExitInterfaceDeclaration(JavaParser.InterfaceDeclarationContext context)
        {
            _nodeStack.Pop();
        }

        public override void EnterPackageDeclaration(JavaParser.PackageDeclarationContext context)
        {
            var package = new JavaPackage(context, File);
            File.Package = package;
        }

        public override void EnterImportDeclaration([NotNull] JavaParser.ImportDeclarationContext context)
        {
            //var import = new JavaImport(context, File);
            //if (!File.ImportExists(import))
            //{
            //    File.Imports.Add(import);
            //}
            InsertOrUpdateNode(context, () => new JavaImport(context, File));
        }

        public override void EnterConstructorDeclaration(JavaParser.ConstructorDeclarationContext context)
        {
            _nodeStack.Push(InsertOrUpdateNode(context, () => new JavaConstructor(context, (JavaClass)Current.Node)));
            ApplyAnnotations(Current);
        }

        public override void ExitConstructorDeclaration(JavaParser.ConstructorDeclarationContext context)
        {
            _nodeStack.Pop();
        }

        public override void EnterMethodDeclaration(JavaParser.MethodDeclarationContext context)
        {
            _nodeStack.Push(InsertOrUpdateNode(context, () => new JavaMethod(context, (JavaClass)Current.Node)));
            ApplyAnnotations(Current);
        }

        public override void ExitMethodDeclaration(JavaParser.MethodDeclarationContext context)
        {
            _nodeStack.Pop();
        }

        public override void EnterFormalParameter(JavaParser.FormalParameterContext context)
        {
            InsertOrUpdateNode(context, () => new JavaParameter(context, Current.Node));
        }

        public override void EnterInterfaceMethodDeclaration(JavaParser.InterfaceMethodDeclarationContext context)
        {
            _nodeStack.Push(InsertOrUpdateNode(context, () => new JavaInterfaceMethod(context, Current.Node)));
            ApplyAnnotations(Current);
        }

        public override void ExitInterfaceMethodDeclaration(JavaParser.InterfaceMethodDeclarationContext context)
        {
            _nodeStack.Pop();
        }

        public override void EnterFieldDeclaration(JavaParser.FieldDeclarationContext context)
        {
            _nodeStack.Push(InsertOrUpdateNode(context, () => new JavaField(context, (JavaClass)Current.Node)));
            ApplyAnnotations(Current);
        }

        public override void ExitFieldDeclaration(JavaParser.FieldDeclarationContext context)
        {
            _nodeStack.Pop();
        }
        
        private ICollection<JavaParser.AnnotationContext> _annotations = new List<JavaParser.AnnotationContext>();

        private void ApplyAnnotations(JavaNodeContext nodeContext)
        {
            foreach (var annotationContext in _annotations)
            {
                var existing = nodeContext.Node.TryGetAnnotation(annotationContext);
                if (existing == null)
                {
                    nodeContext.Node.InsertAnnotation(Current.AnnotationIndex, new JavaAnnotation(annotationContext, nodeContext.Node));
                }
                else
                {
                    existing.UpdateContext(annotationContext);
                }

                nodeContext.AnnotationIndex++;
            }
            _annotations.Clear();
        }
        public override void EnterAnnotation(JavaParser.AnnotationContext context)
        {
            _annotations.Add(context);
            //var existing = Current.Node.TryGetAnnotation(context);
            //if (existing == null)
            //{
            //    Current.Node.InsertAnnotation(Current.AnnotationIndex, new JavaAnnotation(context, Current.Node));
            //}
            //else
            //{
            //    existing.UpdateContext(context);
            //}

            //Current.AnnotationIndex++;
        }

        private JavaNodeContext InsertOrUpdateNode<TRuleContext>(TRuleContext context, Func<JavaNode> createNode)
            where TRuleContext : ParserRuleContext
        {
            var node = Current.Node.TryGetChild(context);
            if (node == null)
            {
                node = createNode();
                Current.Node.InsertChild(Current.ChildIndex, node);
            }
            else
            {
                node.UpdateContext(context);
            }


            if (Current.ChildIndex < Current.Node.Children.Count)
            {
                Current.ChildIndex++;
            }

            return new JavaNodeContext(node);
        }

        private class JavaNodeContext
        {
            public JavaNodeContext(JavaNode node)
            {
                Node = node;
                ChildIndex = 0;
                AnnotationIndex = 0;
            }
            public JavaNode Node { get; }
            public int ChildIndex { get; set; }
            public int AnnotationIndex { get; set; }
        }
    }
}