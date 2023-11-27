namespace Intent.Modules.Common.FileBuilders;

public interface IFileBuilderBase
{
    void StartBuild();
    void CompleteBuild();
    void AfterBuild();
}