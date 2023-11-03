using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpFile : CSharpMetadataBase<CSharpFile>
{
    private readonly IList<(Action Action, int Order)> _configurations = new List<(Action Action, int Order)>();
    private readonly IList<(Action Action, int Order)> _configurationsAfter = new List<(Action Action, int Order)>();
    private readonly Dictionary<string, IHasCSharpName> _modelReferenceRegistry = new();

    public IList<CSharpUsing> Usings { get; } = new List<CSharpUsing>();
    public string Namespace { get; }
    public string RelativeLocation { get; }
    public ICSharpFileBuilderTemplate Template { get; }
    public string DefaultIntentManaged { get; private set; } = "Mode.Fully";
    public IList<CSharpInterface> Interfaces { get; } = new List<CSharpInterface>();
    public IList<CSharpClass> TypeDeclarations { get; } = new List<CSharpClass>();

    public IList<CSharpClass> Classes => TypeDeclarations
        .Where(td => td.TypeDefinitionType == CSharpClass.Type.Class)
        .ToList();

    public IList<CSharpRecord> Records => TypeDeclarations
        .Where(td => td.TypeDefinitionType == CSharpClass.Type.Record)
        .Cast<CSharpRecord>()
        .ToList();

    public IList<CSharpEnum> Enums { get; } = new List<CSharpEnum>();
    public IList<CSharpAssemblyAttribute> AssemblyAttributes { get; } = new List<CSharpAssemblyAttribute>();

    public CSharpFile(string @namespace, string relativeLocation)
    {
        Namespace = @namespace.ToCSharpNamespace();
        RelativeLocation = relativeLocation;
    }

    public CSharpFile(string @namespace, string relativeLocation, ICSharpFileBuilderTemplate template) : this(@namespace, relativeLocation)
    {
        Template = template;
    }

    public CSharpFile AddUsing(string @namespace)
    {
        Usings.Add(new CSharpUsing(@namespace));
        return this;
    }

    public CSharpFile AddClass(string name, Action<CSharpClass> configure = null)
    {
        var @class = new CSharpClass(name, CSharpClass.Type.Class, this);
        TypeDeclarations.Add(@class);
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

    public CSharpFile AddRecord(string name, Action<CSharpRecord> configure = null)
    {
        var record = new CSharpRecord(name);
        TypeDeclarations.Add(record);
        if (_isBuilt)
        {
            configure?.Invoke(record);
        }
        else if (configure != null)
        {
            _configurations.Add((() => configure(record), 0));
        }

        return this;
    }

    public CSharpFile AddInterface(string name, Action<CSharpInterface> configure = null)
    {
        var @interface = new CSharpInterface(name, this);
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

    public CSharpFile AddEnum(string name, Action<CSharpEnum> configure = null)
    {
        var @enum = new CSharpEnum(name);
        Enums.Add(@enum);
        if (_isBuilt)
        {
            configure?.Invoke(@enum);
        }
        else if (configure != null)
        {
            _configurations.Add((() => configure(@enum), 0));
        }

        return this;
    }

    internal CSharpFile RegisterReference(string modelId, IHasCSharpName reference)
    {
        if (_modelReferenceRegistry.ContainsKey(modelId))
        {
            throw new InvalidOperationException($"Cannot add CSharp reference '{reference.Name}' for the model with id {modelId}: A reference with this key already exists '{GetReferenceForModel(modelId).Name}'.");
        }
        _modelReferenceRegistry.Add(modelId, reference);
        return this;
    }

    public IHasCSharpName GetReferenceForModel(string modelId)
    {
        return _modelReferenceRegistry.ContainsKey(modelId) ? _modelReferenceRegistry[modelId] : null;
    }

    public IHasCSharpName GetReferenceForModel(IMetadataModel model)
    {
        return _modelReferenceRegistry.ContainsKey(model.Id) ? _modelReferenceRegistry[model.Id] : null;
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
            className: Interfaces
                           .Select(s => s.Name)
                           .Concat(Classes.Select(s => s.Name))
                           .Concat(Records.Select(s => s.Name))
                           .Concat(Enums.Select(s => s.Name))
                           .FirstOrDefault()
                       ?? throw new Exception("At least one type must be specified for C# file"),
            @namespace: Namespace,
            relativeLocation: RelativeLocation);
    }

    private bool _isBuilt;
    private bool _afterBuildRun;

    public CSharpFile AddAssemblyAttribute(string name, Action<CSharpAssemblyAttribute> configure = null)
    {
        return AddAssemblyAttribute(new CSharpAssemblyAttribute(name), configure);
    }

    public CSharpFile AddAssemblyAttribute(CSharpAssemblyAttribute attribute, Action<CSharpAssemblyAttribute> configure = null)
    {
        AssemblyAttributes.Add(attribute);
        configure?.Invoke(attribute);
        return this;
    }

    public CSharpFile OnBuild(Action<CSharpFile> configure, int order = 0)
    {
        if (_isBuilt)
        {
            throw new Exception("This file has already been built. " +
                                "Consider registering your configuration in the AfterBuild(...) method.");
        }

        _configurations.Add((() => configure(this), order));
        return this;
    }

    public CSharpFile AfterBuild(Action<CSharpFile> configure, int order = 0)
    {
        if (_afterBuildRun)
        {
            throw new Exception("The AfterBuild step has already been run for this file.");
        }

        _configurationsAfter.Add((() => configure(this), order));
        return this;
    }

    public CSharpFile StartBuild()
    {
        while (_configurations.Count > 0)
        {
            var toExecute = _configurations.OrderBy(x => x.Order).First();
            toExecute.Action.Invoke();
            _configurations.Remove(toExecute);
        }

        return this;
    }

    public CSharpFile CompleteBuild()
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

    public CSharpFile AfterBuild()
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
            throw new Exception(
                "Build() needs to be called before ToString(). Check that your template implements ICSharpFileBuilderTemplate, or ensure that Build() is called manually.");
        }

        return $@"{string.Join(@"
", Usings)}
[assembly: DefaultIntentManaged({DefaultIntentManaged})]{string.Concat(AssemblyAttributes.Select(x => $"{Environment.NewLine}{x}"))}

namespace {Namespace}
{{
{string.Join(@"

", Interfaces
    .Select(x => x.ToString("    "))
    .Concat(Classes.Select(x => x.ToString("    ")))
    .Concat(Records.Select(x => x.ToString("    ")))
    .Concat(Enums.Select(x => x.ToString("    "))))}
}}";
    }
}