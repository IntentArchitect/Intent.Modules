using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpFile
{
    private readonly IList<Action> _configurations = new List<Action>();
    public IList<CSharpUsing> Usings { get; } = new List<CSharpUsing>();
    public string Namespace { get; }
    public string RelativeLocation { get; }
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
        _configurations.Add(() => configure(@class));
        return this;
    }

    public CSharpFileConfig GetConfig()
    {
        return new CSharpFileConfig(
            className: Classes.FirstOrDefault()?.Name ?? throw new Exception("At least one type must be specified for C# file"),
            @namespace: Namespace,
            relativeLocation: RelativeLocation);
    }

    public CSharpFile OnBuild(Action<CSharpFile> configure)
    {
        _configurations.Add(() => configure(this));
        return this;
    }

    private bool _isBuilt = false;
    public CSharpFile Build()
    {
        foreach (var configuration in _configurations)
        {
            configuration.Invoke();
        }

        _isBuilt = true;
        return this;
    }

    public override string ToString()
    {
        if (!_isBuilt)
        {
            Build();
        }

        return $@"{string.Join(@"
", Usings)}
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace {Namespace}
{{
{string.Join(@"

", Classes.Select(x => x.ToString("    ")))}
}}";
    }
}