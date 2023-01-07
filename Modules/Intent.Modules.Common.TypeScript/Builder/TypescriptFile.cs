using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Typescript.Templates;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptFile
{
    private readonly IList<(Action Action, int Order)> _configurations = new List<(Action Action, int Order)>();
    private readonly IList<(Action Action, int Order)> _configurationsAfter = new List<(Action Action, int Order)>();
    public IList<TypescriptUsing> Usings { get; } = new List<TypescriptUsing>();
    public string Namespace { get; }
    public string RelativeLocation { get; }
    public string DefaultIntentManaged { get; private set; } = "Mode.Fully";
    public IList<TypescriptInterface> Interfaces { get; } = new List<TypescriptInterface>();
    public IList<TypescriptClass> Classes { get; } = new List<TypescriptClass>();

    public TypescriptFile(string @namespace, string relativeLocation)
    {
        Namespace = @namespace;
        RelativeLocation = relativeLocation;
    }

    public TypescriptFile AddUsing(string @namespace)
    {
        Usings.Add(new TypescriptUsing(@namespace));
        return this;
    }

    public TypescriptFile AddClass(string name, Action<TypescriptClass> configure = null)
    {
        var @class = new TypescriptClass(name);
        Classes.Add(@class);
        if (_isBuilt)
        {
            configure?.Invoke(@class);
        }
        else if (configure != null)
        {
            _configurations.Add((() => configure(@class), 0));
        }
        return this;
    }

    public TypescriptFile AddInterface(string name, Action<TypescriptInterface> configure = null)
    {
        var @interface = new TypescriptInterface(name);
        Interfaces.Add(@interface);
        if (_isBuilt)
        {
            configure?.Invoke(@interface);
        }
        else if (configure != null)
        {
            _configurations.Add((() => configure(@interface), 0));
        }
        return this;
    }

    public TypescriptFile IntentManagedFully()
    {
        DefaultIntentManaged = "Mode.Fully";
        return this;
    }

    public TypescriptFile IntentManagedMerge()
    {
        DefaultIntentManaged = "Mode.Merge";
        return this;
    }

    public TypescriptFile IntentManagedIgnore()
    {
        DefaultIntentManaged = "Mode.Ignore";
        return this;
    }

    public TypescriptFileConfig GetConfig()
    {
        return new TypescriptFileConfig(
            className: Classes.FirstOrDefault()?.Name ?? throw new Exception("At least one type must be specified for C# file"),
            @namespace: Namespace,
            relativeLocation: RelativeLocation);
    }

    private bool _isBuilt;
    private bool _afterBuildRun;

    public TypescriptFile OnBuild(Action<TypescriptFile> configure, int order = 0)
    {
        if (_isBuilt)
        {
            throw new Exception("This file has already been built. " +
                                "Consider registering your configuration in the AfterBuild(...) method.");
        }
        _configurations.Add((() => configure(this), order));
        return this;
    }

    public TypescriptFile AfterBuild(Action<TypescriptFile> configure, int order = 0)
    {
        if (_afterBuildRun)
        {
            throw new Exception("The AfterBuild step has already been run for this file.");
        }
        _configurationsAfter.Add((() => configure(this), order));
        return this;
    }

    //public TypescriptFile Configure()
    //{
    //    foreach (var configuration in _configurations)
    //    {
    //        configuration.Invoke();
    //    }
    //    _configurations.Clear();

    //    return this;
    //}

    public TypescriptFile StartBuild()
    {
        while (_configurations.Count > 0)
        {
            var toExecute = _configurations.OrderBy(x => x.Order).First();
            toExecute.Action.Invoke();
            _configurations.Remove(toExecute);
        }

        return this;
    }

    public TypescriptFile CompleteBuild()
    {
        while (_configurations.Count > 0)
        {
            var toExecute = _configurations.OrderBy(x => x.Order).First();
            toExecute.Action.Invoke();
            _configurations.Remove(toExecute);
        }
        _isBuilt = true;

        return this;
    }

    public TypescriptFile AfterBuild()
    {
        while (_configurationsAfter.Count > 0)
        {
            var toExecute = _configurationsAfter.OrderBy(x => x.Order).First();
            toExecute.Action.Invoke();
            _configurationsAfter.Remove(toExecute);
        }

        if (_configurations.Any())
        {
            throw new Exception("Pending configurations have not been executed. Please contact support@intentarchitect.com for assistance.");
        }

        _afterBuildRun = true;

        return this;
    }

    public override string ToString()
    {
        if (!_isBuilt)
        {
            throw new Exception("Build() needs to be called before ToString(). Check that your template implements ITypescriptFileBuilderTemplate, or ensure that Build() is called manually.");
        }

        return $@"{string.Join(@"
", Usings)}
[assembly: DefaultIntentManaged({DefaultIntentManaged})]

namespace {Namespace}
{{
{string.Join(@"

", Interfaces.Select(x => x.ToString("    ")).Concat(Classes.Select(x => x.ToString("    "))))}
}}";
    }
}