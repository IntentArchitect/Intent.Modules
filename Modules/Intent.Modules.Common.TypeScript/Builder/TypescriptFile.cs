using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Modules.Common.TypeScript.Templates;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptFile
{
    private readonly List<(Action Action, int Order)> _configurations = new();
    private readonly List<(Action Action, int Order)> _configurationsAfter = new();
    public Dictionary<string, TypescriptImport> ImportsBySource { get; } = new();
    public string RelativeLocation { get; }
    public List<TypescriptInterface> Interfaces { get; } = new();
    public List<TypescriptClass> Classes { get; } = new();
    public List<TypescriptStatement> Statements { get; } = new();

    public TypescriptFile(string relativeLocation)
    {
        RelativeLocation = relativeLocation;
    }

    public string Indentation { get; private set; } = "  ";

    public TypescriptFile SetIndentation(int indent)
    {
        Indentation = new string(' ', indent);
        return this;
    }

    public TypescriptFile AddImport(string name, string source)
    {
        return AddImport(name, null, source);
    }

    public TypescriptFile AddImports(IEnumerable<string> names, string source)
    {
        foreach (var name in names)
        {
            AddImport(name, null, source);
        }

        return this;
    }

    public TypescriptFile AddImport(string name, string alias, string source)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var key = name == "*"
            ? $"{source}-*"
            : source;

        if (!ImportsBySource.TryGetValue(key, out var import))
        {
            import = new TypescriptImport(source);
            ImportsBySource.Add(key, import);
        }

        import.HasExport(name, alias);

        return this;
    }

    public TypescriptFile AddDefaultImport(string name, string source)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (!ImportsBySource.TryGetValue(source, out var import))
        {
            import = new TypescriptImport(source);
            ImportsBySource.Add(source, import);
        }

        import.HasDefaultExport(name);

        return this;
    }

    public TypescriptFile AddClass(string name, Action<TypescriptClass> configure = null)
    {
        var @class = new TypescriptClass(name, this);
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
        var @interface = new TypescriptInterface(name, this);
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

    public TypeScriptFileConfig GetConfig(string typeName)
    {
        return new TypeScriptFileConfig(
            className: typeName,
            @namespace: null,
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

    /// <inheritdoc />
    public override string ToString()
    {
        if (!_isBuilt)
        {
            throw new Exception($"Build() needs to be called before ToString(). Check that your template implements {nameof(ITypescriptFileBuilderTemplate)}, or ensure that Build() is called manually.");
        }

        var sb = new StringBuilder();

        foreach (var value in ImportsBySource.Values)
        {
            sb.AppendLine(value.ToString());
        }

        foreach (var @interface in Interfaces)
        {
            sb.AppendLine();
            sb.Append(@interface);
        }

        foreach (var @class in Classes)
        {
            sb.AppendLine();
            sb.Append(@class);
        }

        return sb.ToString();
    }
}