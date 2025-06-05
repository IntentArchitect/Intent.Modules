#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.FileBuilders;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpFile : CSharpMetadataBase<CSharpFile>, ICSharpFile
{
    private readonly IList<(Action Invoke, int Order)> _configurations = new List<(Action Invoke, int Order)>();
    private readonly IList<(Action Invoke, int Order)> _configurationsAfter = new List<(Action Invoke, int Order)>();
    private readonly List<Action<CSharpFileConfig>> _cSharpFileConfigActions = new();
    private OverwriteBehaviour _overwriteBehaviour = OverwriteBehaviour.Always;
    private string? _fileName;
    private string? _fileExtension;

    public IList<CSharpUsing> Usings { get; } = new List<CSharpUsing>();
    public string Namespace { get; private set; }
    public string RelativeLocation { get; }
    public ICSharpTemplate Template { get; internal set; }
    public string DefaultIntentManaged { get; private set; } = "Mode.Fully";
    public IList<CSharpInterface> Interfaces { get; } = new List<CSharpInterface>();
    public IList<CSharpClass> TypeDeclarations { get; } = new List<CSharpClass>();
    public CSharpTopLevelStatements TopLevelStatements { get; private set; }
    public ICSharpStyleSettings StyleSettings { get; }
    public IList<string> LeadingTrivia { get; } = new List<string>();

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
        StyleSettings = CSharpStyleSettings.Settings;
    }

    public CSharpFile(string @namespace, string relativeLocation, ICSharpFileBuilderTemplate template) : this(
        @namespace, relativeLocation)
    {
        Template = template;
    }

    internal CSharpFile(string @namespace, string relativeLocation, ICSharpStyleSettings settings)
    {
        Namespace = @namespace.ToCSharpNamespace();
        RelativeLocation = relativeLocation;
        StyleSettings = settings;
    }

    public CSharpFile AddUsing(string @namespace)
    {
        Usings.Add(new CSharpUsing(@namespace));
        return this;
    }

    public CSharpFile AddGlobalUsing(string @namespace)
    {
        Usings.Add(new CSharpUsing(@namespace, isGlobal: true));
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

    public CSharpFile WithOverwriteBehaviour(OverwriteBehaviour overwriteBehaviour)
    {
        _overwriteBehaviour = overwriteBehaviour;
        return this;
    }

    public CSharpFile WithFileName(string fileName)
    {
        _fileName = fileName;
        return this;
    }

    public CSharpFile WithFileExtension(string fileExtension)
    {
        _fileExtension = fileExtension;
        return this;
    }

    public CSharpFile WithNamespace(string @namespace)
    {
        Namespace = @namespace.ToCSharpNamespace();
        return this;
    }

    /// <summary>
    /// For adding trivia (such as a comment or pragma) to the very start of the file.
    /// </summary>
    public CSharpFile WithLeadingTrivia(string trivia)
    {
        LeadingTrivia.Add(trivia);
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
            relativeLocation: RelativeLocation,
            overwriteBehaviour: _overwriteBehaviour,
            fileName: _fileName ?? className,
            fileExtension: _fileExtension ?? "cs",
            dependsUpon: default);

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

    IReadOnlyCollection<(Action Invoke, int Order)> IFileBuilderBase.GetConfigurationDelegates() => GetConfigurationDelegates();
    /// <summary>
    /// Internal for unit testing reasons.
    /// </summary>
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

    void IFileBuilderBase.MarkCompleteBuildAsDone() => MarkCompleteBuildAsDone();
    /// <summary>
    /// Internal for unit testing reasons.
    /// </summary>
    internal void MarkCompleteBuildAsDone()
    {
        _isBuilt = true;
    }

    IReadOnlyCollection<(Action Invoke, int Order)> IFileBuilderBase.GetConfigurationAfterDelegates()
    {
        if (_configurationsAfter.Count == 0)
        {
            return Array.Empty<(Action Invoke, int Order)>();
        }

        var toReturn = _configurationsAfter.ToList();
        _configurationsAfter.Clear();
        return toReturn;
    }

    void IFileBuilderBase.MarkAfterBuildAsDone()
    {
        if (_configurations.Any())
        {
            throw new Exception("Pending configurations have not been executed. Please contact support@intentarchitect.com for assistance.");
        }

        _afterBuildRun = true;
    }

    public override string ToString()
    {
        if (!_isBuilt)
        {
            throw new Exception(
                "Build() needs to be called before ToString(). Check that your template implements ICSharpFileBuilderTemplate, or ensure that Build() is called manually.");
        }

        var sb = new StringBuilder();

        foreach (var trivia in LeadingTrivia)
        {
            sb.AppendLine(trivia);
        }

        var usings = Usings.OrderBy(x => x.IsGlobal ? 0 : 1).ToArray();
        for (var index = 0; index < usings.Length; index++)
        {
            var @using = usings[index];
            sb.AppendLine(@using.ToString());

            if (index == usings.Length - 1)
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

        var elementOrder = StyleSettings?.ElementOrder.ToArray() ?? [];

        var typeDeclarations = Enumerable.Empty<string>()
            .Concat(Interfaces.OrderBy(i => Array.IndexOf(elementOrder, i.AccessModifier.Trim())).GroupBy(i => i.Name).SelectMany(g => g).Select(x => x.ToString("    ")))
            .Concat(Classes.OrderBy(c => Array.IndexOf(elementOrder, c.AccessModifier.Trim())).GroupBy(c => c.Name).SelectMany(g => g).Select(x => x.ToString("    ")))
            .Concat(Records.OrderBy(r => Array.IndexOf(elementOrder, r.AccessModifier.Trim())).GroupBy(r => r.Name).SelectMany(g => g).Select(x => x.ToString("    ")))
            .Concat(Enums.OrderBy(e => Array.IndexOf(elementOrder, e.AccessModifier.Trim())).Select(e => e.ToString("    ")))
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

            sb.Append('}');
        }

        return sb.ToString();
    }
}