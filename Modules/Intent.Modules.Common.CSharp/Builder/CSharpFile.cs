using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpFile
{
    private readonly IList<(Action Action, int Order)> _configurations = new List<(Action Action, int Order)>();
    public IList<CSharpUsing> Usings { get; } = new List<CSharpUsing>();
    public string Namespace { get; }
    public string RelativeLocation { get; }
    public string DefaultIntentManaged { get; private set; } = "Mode.Fully";
    public IList<CSharpInterface> Interfaces { get; } = new List<CSharpInterface>();
    public IList<CSharpClass> Classes { get; } = new List<CSharpClass>();

    public CSharpFile(string @namespace, string relativeLocation)
    {
        Namespace = @namespace;
        RelativeLocation = relativeLocation;
    }

    public CSharpFile AddUsing(string @namespace)
    {
        Usings.Add(new CSharpUsing(@namespace));
        return this;
    }

    public CSharpFile AddClass(string name, Action<CSharpClass> configure)
    {
        var @class = new CSharpClass(name);
        Classes.Add(@class);
        _configurations.Add((() => configure(@class), 0));
        return this;
    }

    public CSharpFile AddInterface(string name, Action<CSharpInterface> configure)
    {
        var @interface = new CSharpInterface(name);
        Interfaces.Add(@interface);
        _configurations.Add((() => configure(@interface), 0));
        return this;
    }

    public CSharpFile IntentManagedFully()
    {
        DefaultIntentManaged = "Mode.Fully";
        return this;
    }

    public CSharpFile IntentManagedMerge()
    {
        DefaultIntentManaged = "Mode.Merge";
        return this;
    }

    public CSharpFile IntentManagedIgnore()
    {
        DefaultIntentManaged = "Mode.Ignore";
        return this;
    }

    public CSharpFileConfig GetConfig()
    {
        return new CSharpFileConfig(
            className: Classes.FirstOrDefault()?.Name ?? throw new Exception("At least one type must be specified for C# file"),
            @namespace: Namespace,
            relativeLocation: RelativeLocation);
    }

    private bool _isBuilt = false;

    public CSharpFile OnBuild(Action<CSharpFile> configure, int order = 0)
    {
        _configurations.Add((() => configure(this), order));
        return this;
    }

    //public CSharpFile Configure()
    //{
    //    foreach (var configuration in _configurations)
    //    {
    //        configuration.Invoke();
    //    }
    //    _configurations.Clear();

    //    return this;
    //}

    public CSharpFile StartBuild()
    {
        foreach (var configuration in _configurations.OrderBy(x => x.Order))
        {
            configuration.Action.Invoke();
        }
        _configurations.Clear();

        return this;
    }

    public CSharpFile FinalizeBuild()
    {
        foreach (var configuration in _configurations.OrderBy(x => x.Order))
        {
            configuration.Action.Invoke();
        }
        _configurations.Clear();

        _isBuilt = true;
        return this;
    }

    public override string ToString()
    {
        if (!_isBuilt)
        {
            throw new Exception("Build() needs to be called before ToString(). Check that your template implements ICSharpFileBuilderTemplate, or ensure that Build() is called manually.");
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