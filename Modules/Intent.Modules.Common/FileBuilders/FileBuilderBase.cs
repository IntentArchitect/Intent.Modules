using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.FileBuilders;

public abstract class FileBuilderBase : IFileBuilderBase
{
    private readonly string _fileName;
    private readonly OverwriteBehaviour _overwriteBehaviour;
    protected string _relativeLocation;
    protected string _extension;
    protected readonly IList<(Action Action, int Order)> Configurations = new List<(Action Action, int Order)>();
    protected readonly IList<(Action Action, int Order)> ConfigurationsAfter = new List<(Action Action, int Order)>();
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

    public void StartBuild()
    {
        while (Configurations.Count > 0)
        {
            var toExecute = Configurations.OrderBy(x => x.Order).First();
            toExecute.Action.Invoke();
            Configurations.Remove(toExecute);
        }
    }

    public void CompleteBuild()
    {
        while (Configurations.Count > 0)
        {
            var toExecute = Configurations.OrderBy(x => x.Order).First();
            toExecute.Action.Invoke();
            Configurations.Remove(toExecute);
        }

        IsBuilt = true;
    }

    public void AfterBuild()
    {
        while (ConfigurationsAfter.Count > 0)
        {
            var toExecute = ConfigurationsAfter.OrderBy(x => x.Order).First();
            toExecute.Action.Invoke();
            ConfigurationsAfter.Remove(toExecute);
        }

        if (Configurations.Any())
        {
            throw new Exception("Pending configurations have not been executed. Please contact support@intentarchitect.com for assistance.");
        }

        AfterBuildRun = true;
    }

    public ITemplateFileConfig GetConfig()
    {
        return new TemplateFileConfig(
            fileName: _fileName,
            fileExtension: _extension,
            relativeLocation: _relativeLocation,
            overwriteBehaviour: _overwriteBehaviour);
    }
}