using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Builder;

public class JavaFile
{
    private bool _isBuilt;
    private bool _afterBuildRun;
    
    private readonly IList<(Action Action, int Order)> _configurations = new List<(Action Action, int Order)>();
    private readonly IList<(Action Action, int Order)> _configurationsAfter = new List<(Action Action, int Order)>();
    
    public JavaFile(string package, string relativeLocation)
    {
        Package = package;
        RelativeLocation = relativeLocation;
    }
    
    public string Package { get; }
    public string RelativeLocation { get; }
    public IList<JavaInterface> Interfaces { get; } = new List<JavaInterface>();
    public IList<JavaClass> Classes { get; } = new List<JavaClass>();
    public IList<JavaImport> Imports { get; } = new List<JavaImport>();
    
    public JavaFile AddImport(string @namespace)
    {
        Imports.Add(new JavaImport(@namespace));
        return this;
    }

    public JavaFile AddClass(string name, Action<JavaClass> configure = null)
    {
        var @class = new JavaClass(name);
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

    public JavaFile AddInterface(string name, Action<JavaInterface> configure = null)
    {
        var @interface = new JavaInterface(name);
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
    
    public JavaFileConfig GetConfig()
    {
        return new JavaFileConfig(
            className: Interfaces.Select(s => s.Name)
                           .Concat(Classes.Select(s => s.Name))
                           .FirstOrDefault() ??
                       throw new Exception("At least one type must be specified for Java file"),
            package: Package,
            relativeLocation: RelativeLocation);
    }
    
    public JavaFile OnBuild(Action<JavaFile> configure, int order = 0)
    {
        if (_isBuilt)
        {
            throw new Exception("This file has already been built. " +
                                "Consider registering your configuration in the AfterBuild(...) method.");
        }
        _configurations.Add((() => configure(this), order));
        return this;
    }

    public JavaFile AfterBuild(Action<JavaFile> configure, int order = 0)
    {
        if (_afterBuildRun)
        {
            throw new Exception("The AfterBuild step has already been run for this file.");
        }
        _configurationsAfter.Add((() => configure(this), order));
        return this;
    }

    public JavaFile StartBuild()
    {
        while (_configurations.Count > 0)
        {
            var toExecute = _configurations.OrderBy(x => x.Order).First();
            toExecute.Action.Invoke();
            _configurations.Remove(toExecute);
        }

        return this;
    }

    public JavaFile CompleteBuild()
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

    public JavaFile AfterBuild()
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
            throw new Exception("Build() needs to be called before ToString(). Check that your template implements IJavaFileBuilderTemplate, or ensure that Build() is called manually.");
        }

        return $@"package {Package};

{string.Join(@"
", Imports)}

{string.Join(@"

", Interfaces.Select(x => x.ToString("")).Concat(Classes.Select(x => x.ToString(""))))}
";
    }
}