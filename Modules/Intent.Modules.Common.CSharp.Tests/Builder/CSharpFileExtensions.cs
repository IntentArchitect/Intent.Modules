using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

internal static class CSharpFileExtensions
{
    public static CSharpFile CompleteBuild(this CSharpFile file)
    {
        while (true)
        {
            var actions = file.GetConfigurationDelegates();
            if (actions.Count == 0)
            {
                break;
            }

            foreach (var action in actions)
            {
                action.Invoke();
            }
        }

        file.MarkCompleteBuildAsDone();

        return file;
    }
}