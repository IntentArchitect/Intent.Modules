using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.AppStartup;

public interface IAppStartupTemplate : ICSharpFileBuilderTemplate
{
    public static string RoleName => "AppStartupTemplate";
    IAppStartupFile StartupFile { get; }
}