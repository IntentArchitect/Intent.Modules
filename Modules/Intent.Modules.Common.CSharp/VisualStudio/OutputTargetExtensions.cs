using Intent.Engine;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
    public static class OutputTargetExtensions
    {
        public static ICSharpProject GetProject(this IOutputTarget outputTarget)
        {
            return new CSharpProject(outputTarget.GetTargetPath()[0]);
        }
    }
}
