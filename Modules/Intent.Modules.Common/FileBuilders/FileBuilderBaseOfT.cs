using System;
using Intent.Templates;

namespace Intent.Modules.Common.FileBuilders;

public abstract class FileBuilderBase<T> : FileBuilderBase, IFileBuilderBase<T>
    where T : class, IFileBuilderBase<T>
{
    protected FileBuilderBase(
        string fileName,
        string relativeLocation,
        string extension,
        OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always)
        : base(fileName, relativeLocation, extension, overwriteBehaviour)
    {
    }

    public T OnBuild(Action<T> configure, int order = 0)
    {
        if (IsBuilt)
        {
            throw new Exception("This file has already been built. " +
                                "Consider registering your configuration in the AfterBuild(...) method.");
        }

        Configurations.Add((() => configure((T)this), order));
        return (T)this;
    }

    public T AfterBuild(Action<T> configure, int order = 0)
    {
        if (AfterBuildRun)
        {
            throw new Exception("The AfterBuild step has already been run for this file.");
        }

        ConfigurationsAfter.Add((() => configure((T)this), order));
        return (T)this;
    }

    public T WithFileExtension(string fileExtension)
    {
        _extension = fileExtension;
        return (T)this;
    }

    public T WithRelativeLocation(string relativeLocation)
    {
        _relativeLocation = relativeLocation;
        return (T)this;
    }

    public static explicit operator T(FileBuilderBase<T> t) => t as T ?? throw new InvalidOperationException();
}