using System;
using Intent.Templates;

namespace Intent.Modules.Common.FileBuilders;

public interface IFileBuilderBase<out T> : IFileBuilderBase
    where T : IFileBuilderBase<T>
{
    T OnBuild(Action<T> configure, int order = 0);
    T AfterBuild(Action<T> configure, int order = 0);
    T WithFileExtension(string fileExtension);
    T WithRelativeLocation(string relativeLocation);
    ITemplateFileConfig GetConfig();
}