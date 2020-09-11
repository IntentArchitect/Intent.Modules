using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using System.Net.Mime;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor.Parser
{
    public class JavaFileFactoryListener : Java9BaseListener
    {
        public JavaFileFactoryListener(JavaFile javaFile)
        {
            File = javaFile;
            _nodeStack.Push(new JavaNodeContext(File));
        }

        public readonly JavaFile File;

        private readonly Stack<JavaNodeContext> _nodeStack = new Stack<JavaNodeContext>();
        private JavaNodeContext Current => _nodeStack.Peek();

        public override void EnterNormalClassDeclaration(Java9Parser.NormalClassDeclarationContext context)
        {
            _nodeStack.Push(InsertOrUpdateNode(context, () => new JavaClass(context, Current.Node)));
        }

        public override void ExitNormalClassDeclaration(Java9Parser.NormalClassDeclarationContext context)
        {
            _nodeStack.Pop();
        }

        public override void EnterNormalInterfaceDeclaration(Java9Parser.NormalInterfaceDeclarationContext context)
        {
            _nodeStack.Push(InsertOrUpdateNode(context, () => new JavaInterface(context, Current.Node)));
        }

        public override void ExitNormalInterfaceDeclaration(Java9Parser.NormalInterfaceDeclarationContext context)
        {
            _nodeStack.Pop();
        }

        public override void EnterImportDeclaration([NotNull] Java9Parser.ImportDeclarationContext context)
        {
            var import = new JavaImport(context, File);
            if (!File.ImportExists(import))
            {
                File.Imports.Add(import);
            }
        }

        public override void EnterConstructorDeclaration(Java9Parser.ConstructorDeclarationContext context)
        {
            _nodeStack.Push(InsertOrUpdateNode(context, () => new JavaConstructor(context, (JavaClass)Current.Node)));
        }

        public override void ExitConstructorDeclaration(Java9Parser.ConstructorDeclarationContext context)
        {
            _nodeStack.Pop();
        }

        public override void EnterMethodDeclaration(Java9Parser.MethodDeclarationContext context)
        {
            _nodeStack.Push(InsertOrUpdateNode(context, () => new JavaClassMethod(context, (JavaClass)Current.Node)));
        }

        public override void ExitMethodDeclaration(Java9Parser.MethodDeclarationContext context)
        {
            _nodeStack.Pop();
        }

        public override void EnterInterfaceMethodDeclaration(Java9Parser.InterfaceMethodDeclarationContext context)
        {
            _nodeStack.Push(InsertOrUpdateNode(context, () => new JavaInterfaceMethod(context, Current.Node)));
        }

        public override void ExitInterfaceMethodDeclaration(Java9Parser.InterfaceMethodDeclarationContext context)
        {
            _nodeStack.Pop();
        }

        public override void EnterFieldDeclaration(Java9Parser.FieldDeclarationContext context)
        {
            _nodeStack.Push(InsertOrUpdateNode(context, () => new JavaField(context, (JavaClass)Current.Node)));
        }

        public override void ExitFieldDeclaration(Java9Parser.FieldDeclarationContext context)
        {
            _nodeStack.Pop();
        }
        
        public override void EnterAnnotation(Java9Parser.AnnotationContext context)
        {
            var existing = Current.Node.TryGetAnnotation(context);
            if (existing == null)
            {
                Current.Node.InsertAnnotation(Current.AnnotationIndex, new JavaAnnotation(context, Current.Node));
            }
            else
            {
                existing.UpdateContext(context);
            }

            Current.AnnotationIndex++;
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