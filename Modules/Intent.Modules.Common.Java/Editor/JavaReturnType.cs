namespace Intent.Modules.Common.Java.Editor
{
    public class JavaReturnType : JavaNode<JavaParser.TypeTypeOrVoidContext>
    {
        public JavaReturnType(JavaParser.TypeTypeOrVoidContext context, JavaNode parent) : base(context, parent)
        {

        }

        public override string GetIdentifier(JavaParser.TypeTypeOrVoidContext context)
        {
            return context.GetText();
        }
    }
}