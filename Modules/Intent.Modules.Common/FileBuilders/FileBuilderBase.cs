#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.FileBuilders;

public abstract class FileBuilderBase : IFileBuilderBase
{
    private readonly OverwriteBehaviour _overwriteBehaviour;
    protected readonly IList<(Action Action, int Order)> Configurations = new List<(Action Action, int Order)>();
    protected readonly IList<(Action Action, int Order)> ConfigurationsAfter = new List<(Action Action, int Order)>();
    protected readonly Dictionary<string, string> CustomMetadata = new();
    protected string _fileName;
    protected string _relativeLocation;
    protected string _extension;
    protected bool IsBuilt;
    protected bool AfterBuildRun;

    protected FileBuilderBase(
        string fileName,
        string relativeLocation,
        string extension,
        OverwriteBehaviour overwriteBehaviour)
    {
        _fileName = fileName;
        _relativeLocation = relativeLocation ?? "";
        _extension = extension ?? "";
        _overwriteBehaviour = overwriteBehaviour;
    }

    public ITemplateFileConfig GetConfig()
    {
        var templateFileConfig = new TemplateFileConfig(
            fileName: _fileName,
            fileExtension: _extension,
            relativeLocation: _relativeLocation,
            overwriteBehaviour: _overwriteBehaviour,
            codeGenType: CodeGenType ?? Common.CodeGenType.Basic);

        foreach (var (key, value) in CustomMetadata)
        {
            templateFileConfig.CustomMetadata.Add(key, value);
        }

        return templateFileConfig;
    }

    protected string CodeGenType { get; set; }

    IReadOnlyCollection<(Action Invoke, int Order)> IFileBuilderBase.GetConfigurationDelegates()
    {
        if (Configurations.Count == 0)
        {
            return [];
        }

        var toReturn = Configurations.ToArray();
        Configurations.Clear();
        return toReturn;
    }

    void IFileBuilderBase.MarkCompleteBuildAsDone()
    {
        IsBuilt = true;
    }

    IReadOnlyCollection<(Action Invoke, int Order)> IFileBuilderBase.GetConfigurationAfterDelegates()
    {
        if (ConfigurationsAfter.Count == 0)
        {
            return [];
        }

        var toReturn = ConfigurationsAfter.ToList();
        ConfigurationsAfter.Clear();
        return toReturn;
    }

    void IFileBuilderBase.MarkAfterBuildAsDone()
    {
        if (Configurations.Count != 0)
        {
            throw new Exception("Pending configurations have not been executed. Please contact support@intentarchitect.com for assistance.");
        }

        AfterBuildRun = true;
    }
}