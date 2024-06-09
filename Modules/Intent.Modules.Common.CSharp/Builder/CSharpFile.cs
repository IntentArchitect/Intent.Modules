using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpFile : CSharpMetadataBase<CSharpFile>, ICSharpFile
{
    private readonly IList<(Action Invoke, int Order)> _configurations = new List<(Action Invoke, int Order)>();
    private readonly IList<(Action Invoke, int Order)> _configurationsAfter = new List<(Action Invoke, int Order)>();
    private readonly List<Action<CSharpFileConfig>> _cSharpFileConfigActions = new();

    public IList<CSharpUsing> Usings { get; } = new List<CSharpUsing>();
    public string Namespace { get; }
    public string RelativeLocation { get; }
    public ICSharpTemplate Template { get; internal set; }
    public string DefaultIntentManaged { get; private set; } = "Mode.Fully";
    public IList<CSharpInterface> Interfaces { get; } = new List<CSharpInterface>();
    public IList<CSharpClass> TypeDeclarations { get; } = new List<CSharpClass>();
    public CSharpTopLevelStatements TopLevelStatements { get; private set; }

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

    public CSharpFile(string @namespace, string relativeLocation, ICSharpFileBuilderTemplate template) : this(
        @namespace, relativeLocation)
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
        return AddClass(name, configure, 0);
    }

    public CSharpFile AddClass(string name, Action<CSharpClass> configure, int priority)
    {
        var @class = new CSharpClass(name, CSharpClass.Type.Class, this);
        TypeDeclarations.Add(@class);
        if (_isBuilt)
        {
            configure?.Invoke(@class);
        }
        else if (configure != null)
        {
            _configurations.Add((() => configure(@class), priority));
        }

        return this;
    }

    public CSharpFile AddTopLevelStatements(Action<CSharpTopLevelStatements> configure = null, int priority = 0)
    {
        TopLevelStatements ??= new CSharpTopLevelStatements();

        if (_isBuilt)
        {
            configure?.Invoke(TopLevelStatements);
        }
        else if (configure != null)
        {
            _configurations.Add((() => configure(TopLevelStatements), priority));
        }

        return this;
    }

    public CSharpFile AddRecord(string name, Action<CSharpRecord> configure = null)
    {
        var record = new CSharpRecord(name, this);
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
        var @interface = new CSharpInterface(
            name: name,
            file: this,
            parent: this);

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
        var className = TopLevelStatements != null
            ? "Program"
            : Enumerable.Empty<string>()
                  .Concat(Interfaces.Select(s => s.Name))
                  .Concat(Classes.Select(s => s.Name))
                  .Concat(Records.Select(s => s.Name))
                  .Concat(Enums.Select(s => s.Name))
                  .FirstOrDefault();

        if (className == null)
        {
            throw new Exception("Either a file must use top level statements or at least one type must be specified for C# file");
        }

        var configuration = new CSharpFileConfig(
            className: className,
            @namespace: Namespace,
            relativeLocation: RelativeLocation);

        foreach (var action in _cSharpFileConfigActions)
        {
            action(configuration);
        }

        return configuration;
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

    /// <inheritdoc cref="CSharpFileConfig.IntentTagModeExplicit"/>
    public CSharpFile IntentTagModeExplicit()
    {
        _cSharpFileConfigActions.Add(config => config.IntentTagModeExplicit());
        return this;
    }

    /// <inheritdoc cref="CSharpFileConfig.IntentTagModeImplicit"/>
    public CSharpFile IntentTagModeImplicit()
    {
        _cSharpFileConfigActions.Add(config => config.IntentTagModeImplicit());
        return this;
    }

    public string GetModelType<T>(T model) where T : IMetadataModel, IHasName
    {
        if (Template == null)
        {
            throw new InvalidOperationException("Cannot add property with model. Please add the template as an argument to this CSharpFile's constructor.");
        }

        var type = model switch
        {
            IHasTypeReference hasType => Template.GetTypeName(hasType.TypeReference),
            ITypeReference typeRef => Template.GetTypeName(typeRef),
            _ => throw new ArgumentException($"model {model.Name} must implement either IHasTypeReference or ITypeReference", nameof(model))
        };
        return type;
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

    internal IReadOnlyCollection<(Action Invoke, int Order)> GetConfigurationDelegates()
    {
        if (_configurations.Count == 0)
        {
            return [];
        }

        var toReturn = _configurations.ToArray();
        _configurations.Clear();
        return toReturn;
    }

    internal CSharpFile MarkCompleteBuildAsDone()
    {
        _isBuilt = true;
        return this;
    }

    internal IReadOnlyCollection<(Action Invoke, int Order)> GetConfigurationAfterDelegates()
    {
        if (_configurationsAfter.Count == 0)
        {
            return Array.Empty<(Action Invoke, int Order)>();
        }

        var toReturn = _configurationsAfter.ToList();
        _configurationsAfter.Clear();
        return toReturn;
    }

    internal CSharpFile MarkAfterBuildAsDone()
    {
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

        var sb = new StringBuilder();

        for (var index = 0; index < Usings.Count; index++)
        {
            var @using = Usings[index];
            sb.AppendLine(@using.ToString());

            if (index == Usings.Count - 1)
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine($"[assembly: DefaultIntentManaged({DefaultIntentManaged})]");

        foreach (var assemblyAttribute in AssemblyAttributes)
        {
            sb.AppendLine(assemblyAttribute.ToString());
        }

        if (TopLevelStatements != null)
        {
            sb.AppendLine();
            sb.AppendLine(TopLevelStatements.ToString());
        }

        var typeDeclarations = Enumerable.Empty<string>()
            .Concat(Interfaces.Select(x => x.ToString("    ")))
            .Concat(Classes.Select(x => x.ToString("    ")))
            .Concat(Records.Select(x => x.ToString("    ")))
            .Concat(Enums.Select(x => x.ToString("    ")))
            .ToArray();

        if (typeDeclarations.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"namespace {Namespace}");
            sb.AppendLine("{");

            for (var index = 0; index < typeDeclarations.Length; index++)
            {
                var typeDeclaration = typeDeclarations[index];
                if (index != 0)
                {
                    sb.AppendLine();
                }

                sb.AppendLine(typeDeclaration);
            }

            sb.Append("}");
        }

        return sb.ToString();
    }
}