using System;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor.Parser
{


    public class JavaNodeListener : Java9BaseListener
    {
        private readonly JavaNode _node;
        private int _childIndex;

        public JavaNodeListener(JavaNode node)
        {
            _node = node;
        }

        //public override void EnterAnnotation(Java9Parser.AnnotationContext context)
        //{
        //    var existing = _node.TryGetAnnotation(context);
        //    if (existing == null)
        //    {
        //        _node.InsertAnnotation(_annotationIndex, new JavaAnnotation(context, _node));
        //    }
        //    else
        //    {
        //        existing.UpdateContext(context);
        //    }

        //    _annotationIndex++;
        //}

        protected JavaNode InsertOrUpdateNode<TRuleContext>(TRuleContext context, Func<JavaNode> createNode)
            where TRuleContext : ParserRuleContext
        {
            var node = _node.TryGetChild(context);
            if (node == null)
            {
                node = createNode();
                _node.InsertChild(_childIndex, node);
            }
            else
            {
                node.UpdateContext(context);
            }

            if (_childIndex < _node.Children.Count)
            {
                _childIndex++;
            }

            return node;
        }

    }
}