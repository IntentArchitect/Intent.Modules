#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Utils;

namespace Intent.Modules.Common.CSharp.FactoryExtensions
{
    /// <summary>
    /// Cache of known C# types as determined by all templates which are
    /// <see cref="IClassProvider"/>.
    /// </summary>
    public class CSharpTypesCache : FactoryExtensionBase
    {
        private readonly IFileSystem _fileSystem;

        /// <inheritdoc />
        public override string Id => "Intent.Modules.Common.CSharp.FactoryExtensions.KnownCSharpTypesCache";

        private static TypeRegistry? _knownTypes;
        private static TypeRegistry? _outputTargetNames;
        private static readonly TypeRegistry Empty = new();
        private static readonly HashSet<string> ManuallyAddedKnownTypes = [];
        private static readonly Dictionary<string, HashSet<string>> GlobalUsingsByProjectId = [];

        public CSharpTypesCache(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public override int Order { get; set; }

        /// <inheritdoc />
        protected override void OnStart(IApplication application)
        {
            ManuallyAddedKnownTypes.Clear();
            GlobalUsingsByProjectId.Clear();
            base.OnStart(application);
        }

        /// <inheritdoc />
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var knownTypesByNamespace = application
                .FindTemplateInstances<IClassProvider>(TemplateDependency.OfType<IClassProvider>())
                .Select(x => string.IsNullOrWhiteSpace(x.Namespace) ? x.ClassName : $"{x.Namespace}.{x.ClassName}");
            var commonKnownTypes = GetCommonKnownTypes();

            var knownTypes = knownTypesByNamespace
                .Union(commonKnownTypes)
                .Union(ManuallyAddedKnownTypes)
                .ToArray();

            _knownTypes = new TypeRegistry().WithTypes(knownTypes);

            var instanceRegistryKnownTypes = TemplateInstanceRegistry.GetRegisteredTypes().OfType<CSharpResolvedTypeInfo>();
            foreach (var otherInstance in instanceRegistryKnownTypes)
            {
                _knownTypes.Add(otherInstance.Namespace, otherInstance.Name);
            }

            var outputTargetNames = application.OutputTargets.Select(x => x.Name).ToArray();
            _outputTargetNames = new TypeRegistry().WithNamespaces(outputTargetNames);

            PopulateGlobalUsings(application);
            AddFromCSharpStereotypes(application);
        }

        private static void AddFromCSharpStereotypes(IApplication application)
        {
            var domainDesigner = application.MetadataManager.GetDesigner(application.Id, "6ab29b31-27af-4f56-a67c-986d82097d63");
            var servicesDesigner = application.MetadataManager.GetDesigner(application.Id, "55d06096-83fe-4acf-86ef-586d254e5170");
            var elements = domainDesigner.Elements.Union(servicesDesigner.Elements);

            foreach (var element in elements)
            {
                var cSharpStereotype = element.GetStereotype("30c5995e-17ab-4cc7-8881-3e9561ab06fe");
                if (cSharpStereotype == null)
                {
                    continue;
                }

                var typeProperty = cSharpStereotype.GetProperty<string>("Type");
                var namespaceProperty = cSharpStereotype.GetProperty<string>("Namespace");
                var typeName = !string.IsNullOrEmpty(typeProperty)
                    ? typeProperty
                    : element.Name;
                if (string.IsNullOrWhiteSpace(typeName))
                {
                    continue;
                }

                var fullyQualifiedType = !string.IsNullOrWhiteSpace(namespaceProperty)
                    ? namespaceProperty + "." + typeName
                    : typeName;
                if (!fullyQualifiedType.Contains('.'))
                {
                    continue;
                }

                AddKnownType(fullyQualifiedType);
            }
        }

        private void PopulateGlobalUsings(IApplication application)
        {
            var csharpTemplates = application.FindTemplateInstances<IIntentTemplate>(TemplateDependency.OfType<IIntentTemplate>());
            var csharpTemplatesByProject = csharpTemplates.GroupBy(GetOutputProjectId);

            foreach (var grouping in csharpTemplatesByProject)
            {
                if (!GlobalUsingsByProjectId.TryGetValue(grouping.Key, out var globalUsings))
                {
                    globalUsings = [];
                    GlobalUsingsByProjectId.Add(grouping.Key, globalUsings);
                }

                var templates = grouping.ToArray();

                var alreadyProcessedFiles = new HashSet<string>(templates.Length);
                var csProjFolder = default(string);

                foreach (var template in templates)
                {
                    var path = template.FileMetadata.GetFilePath().Replace('\\', '/');

                    if (template.TryGetExistingFilePath(out var existingFilePath))
                    {
                        alreadyProcessedFiles.Add(existingFilePath);
                    }

                    if (Path.GetExtension(path).Equals(".csproj", StringComparison.OrdinalIgnoreCase) &&
                        _fileSystem.Exists(path))
                    {
                        csProjFolder = Path.GetDirectoryName(path);
                    }

                    if (template is not IHasGlobalUsings canContainGlobalUsings)
                    {
                        continue;
                    }

                    globalUsings.UnionWith(canContainGlobalUsings.GetGlobalUsings());
                }

                if (csProjFolder == null)
                {
                    continue;
                }

                _fileSystem.EnableCachingFor(csProjFolder);

                var filePaths = Directory.EnumerateFiles(csProjFolder, "*.cs", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = true });
                foreach (var filePath in filePaths)
                {
                    if (!alreadyProcessedFiles.Add(filePath.Replace('\\', '/')))
                    {
                        continue;
                    }

                    globalUsings.UnionWith(_fileSystem.ReadAllLines(filePath)
                        .Select(x => x.Trim())
                        .Where(x => x.StartsWith("global using "))
                        .Select(x => x["global using ".Length..^1]));
                }
            }
        }

        private static string GetOutputProjectId(IIntentTemplate templateInstance) => templateInstance.OutputTarget.GetProject().Id;

        internal static void AddKnownType(string fullyQualifiedTypeName)
        {
            if (_knownTypes == null)
            {
                ManuallyAddedKnownTypes.Add(fullyQualifiedTypeName);
                return;
            }

            _knownTypes.Add(fullyQualifiedTypeName);
        }

        private static string[] GetCommonKnownTypes()
        {
            // This forces these assemblies to be loaded prior to the call to
            // AppDomain.CurrentDomain.GetAssemblies()
            var additionalTypesToLoad = new[]
                {
                    typeof(Transaction)
                }
                .Select(x => x.Assembly)
                .Distinct();
            foreach (var type in additionalTypesToLoad)
            {
                Logging.Log.Debug($"Ensured types {type.GetName().Name} will be scanned for known types.");
            }

            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.GetName().Name?.Split('.')[0] == "System")
                .SelectMany(c => c.GetExportedTypes())
                .Where(x => !x.ContainsGenericParameters)
                .Select(x => x.FullName!)
                .OrderBy(x => x)
                .ToArray();
        }

        internal static TypeRegistry GetKnownTypes() => _knownTypes ?? Empty;

        internal static TypeRegistry GetOutputTargetNames() => _outputTargetNames ?? Empty;

        internal static IReadOnlySet<string> GetGlobalUsings(ICSharpTemplate template) => GlobalUsingsByProjectId.TryGetValue(template.Project.Id, out var globalUsings) ? globalUsings : [];
    }
}
