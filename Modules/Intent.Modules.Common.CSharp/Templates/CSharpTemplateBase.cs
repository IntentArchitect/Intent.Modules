using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates
{
    public abstract class CSharpTemplateBase : CSharpTemplateBase<object>
    {
        protected CSharpTemplateBase(string templateId, IOutputTarget outputTarget) : base(templateId, outputTarget, null)
        {
        }
    }

    public abstract class CSharpTemplateBase<TModel, TDecorator> : CSharpTemplateBase<TModel>, IHasDecorators<TDecorator> 
        where TDecorator : ITemplateDecorator
    {
        private readonly ICollection<TDecorator> _decorators = new List<TDecorator>();

        protected CSharpTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }

        public IEnumerable<TDecorator> GetDecorators()
        {
            return _decorators.OrderBy(x => x.Priority);
        }

        public void AddDecorator(TDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        /// <summary>
        /// Aggregates the specified <see cref="propertyFunc"/> property of all Decorators. Ignores Decorators where the property returns null.
        /// </summary>
        /// <param name="propertyFunc"></param>
        /// <returns></returns>
        protected string GetDecoratorsOutput(Func<TDecorator, string> propertyFunc)
        {
            return GetDecorators().Aggregate(propertyFunc);
        }
    }

    public abstract class CSharpTemplateBase<TModel> : IntentTemplateBase<TModel>, IHasNugetDependencies, IHasAssemblyDependencies, IClassProvider, IRoslynMerge, IDeclareUsings 
    {
        private readonly ICollection<ITemplateDependency> _detectedDependencies = new List<ITemplateDependency>();
        private readonly ICollection<IAssemblyReference> _assemblyDependencies = new List<IAssemblyReference>();
        private List<string> _additionalUsingNamespaces = new List<string>();

        protected CSharpTemplateBase(string templateId, IOutputTarget outputTarget, TModel model)
            : base(templateId, outputTarget, model)
        {
            AddNugetDependency("Intent.RoslynWeaver.Attributes", "1.0.0");
            Types = new CSharpTypeResolver(OutputTarget.GetProject(), new CollectionFormatter(x => $"{UseType("System.Collections.Generic.IEnumerable")}<{x.Name}>"));
        }

        /// <summary>
        /// Returns the csproj file <see cref="IOutputTarget"/> that contains this file.
        /// </summary>
        public IOutputTarget Project => OutputTarget.GetProject();

        /// <summary>
        /// Returns the class' namespace as specified in the <see cref="CSharpFileConfig"/>. Escapes any invalid characters and enforces pascal-case.
        /// May be overriden.
        /// </summary>
        public virtual string Namespace
        {
            get
            {
                if (FileMetadata.CustomMetadata.ContainsKey("Namespace"))
                {
                    return FileMetadata.CustomMetadata["Namespace"].ToCSharpNamespace();
                }
                return this.OutputTarget.Name.ToCSharpNamespace();
            }
        }

        /// <summary>
        /// Returns the class name as specified in the <see cref="CSharpFileConfig"/>. Escapes any invalid characters and enforces pascal-case.
        /// May be overriden.
        /// </summary>
        public virtual string ClassName
        {
            get
            {
                if (FileMetadata.CustomMetadata.ContainsKey("ClassName"))
                {
                    return FileMetadata.CustomMetadata["ClassName"].ToCSharpIdentifier();
                }
                return FileMetadata.FileName.ToCSharpIdentifier();
            }
        }

        /// <summary>
        /// Adds the @namespace as a dependent <see cref="@namespace"/> and returns the <see cref="name"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public string UseType(string name, string @namespace)
        {
            _additionalUsingNamespaces.Add(@namespace);
            return name;
        }

        /// <summary>
        /// Adds the namespace of the <see cref="fullName"/> as a dependent namespace and returns the normalized name />
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public string UseType(string fullName)
        {
            var elements = new List<string>(fullName.Split(".", StringSplitOptions.RemoveEmptyEntries));
            var name = elements.Last();
            elements.RemoveAt(elements.Count - 1);
            _additionalUsingNamespaces.Add(string.Join(".", elements));
            return NormalizeNamespace(fullName);
        }

        public override string RunTemplate()
        {
            var templateOutput = base.RunTemplate();
            return $@"{DependencyUsings}
{templateOutput}".TrimStart();
        }

        /// <summary>
        /// Adds a Template source that will be search when resolving <see cref="ITypeReference"/> types through the <see cref="IntentTemplateBase.GetTypeName(ITypeReference)"/>
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="collectionFormat">Sets the collection type to be used if a type is found.</param>
        [Obsolete("Specify using fluent api (e.g. AddTypeSource(...).WithCollectionFormat(...);")]
        public new void AddTypeSource(string templateId, string collectionFormat = "IEnumerable<{0}>")
        {
            AddTypeSource(ClassTypeSource.Create(ExecutionContext, templateId).WithCollectionFormatter(new CollectionFormatter(collectionFormat)));
        }

        /// <summary>
        /// Called once a type has been resolved in the <see cref="IntentTemplateBase.GetTypeName(Intent.Metadata.Models.ITypeReference)"/>.
        /// Override to alter the resulting string.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string NormalizeTypeName(string name)
        {
            return NormalizeNamespace(name);
        }

        /// <summary>
        /// Converts the namespace of a fully qualified class name to the relative namespace for this class instance
        /// </summary>
        /// <param name="foreignType">The foreign type which is ideally fully qualified</param>
        /// <returns></returns>
        public virtual string NormalizeNamespace(string foreignType)
        {
            // Handle Generics recursively:
            string normalizedGenericTypes = null;
            if (foreignType.Contains("<") && foreignType.Contains(">"))
            {
                var genericTypes = foreignType.Substring(foreignType.IndexOf("<", StringComparison.Ordinal) + 1, foreignType.Length - foreignType.IndexOf("<", StringComparison.Ordinal) - 2);
                normalizedGenericTypes = genericTypes
                    .Split(',')
                    .Select(NormalizeNamespace)
                    .Aggregate((x, y) => x + ", " + y);
                foreignType = $"{foreignType.Substring(0, foreignType.IndexOf("<", StringComparison.Ordinal))}";
            }

            var usingPaths = DependencyUsings
                .Split(';')
                .Select(x => x.Trim().Replace("using ", ""))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Union(TemplateUsings)
                .Union(ExistingContentUsings)
                .Distinct()
                .ToArray();
            var localNamespace = Namespace;
            var knownOtherPaths = usingPaths
                .Concat(ExecutionContext.OutputTargets.Select(x => x.Name))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray();

            return NormalizeNamespace(localNamespace, foreignType, knownOtherPaths, usingPaths) + (normalizedGenericTypes != null ? $"<{normalizedGenericTypes}>" : "");
        }

        private IEnumerable<string> _templateUsings;
        private IEnumerable<string> TemplateUsings => _templateUsings ?? (!string.IsNullOrWhiteSpace(GenerationEnvironment.ToString()) ? (_templateUsings = GetUsingsFromContent(GenerationEnvironment.ToString())) : new string[0]);

        private IEnumerable<string> _existingContentUsings;
        private IEnumerable<string> ExistingContentUsings
        {
            get
            {
                if (_existingContentUsings != null)
                {
                    return _existingContentUsings;
                }

                var filepath = FileMetadata.GetFullLocationPathWithFileName();
                if (!File.Exists(filepath))
                {
                    return _existingContentUsings = new string[0];
                }

                return _existingContentUsings = GetUsingsFromContent(File.ReadAllText(filepath));
            }
        }

        internal static string NormalizeNamespace(
            string localNamespace,
            string foreignType,
            string[] knownOtherPaths,
            string[] usingPaths)
        {
            // NB: If changing this method, please run the unit tests against it

            var localNamespaceParts = localNamespace.Split('.').ToArray();

            var foreignTypeParts = foreignType.Split('.').ToArray();
            if (foreignTypeParts.Length == 1)
            {
                return foreignType;
            }

            if (localNamespaceParts.SequenceEqual(foreignTypeParts.Take(foreignTypeParts.Length - 1)))
            {
                return foreignTypeParts.Last();
            }

            // Is there already a using which matches qualifier:
            // (It's not immediately clear what scenario "usings.All(x => x != foreignType)" covers, if you know, please document)
            // localNamespaceParts.Contains(foreignTypeParts.Last()) - if name exists in local namespace, can't use name as is.
            var foreignTypeQualifier = foreignTypeParts.Take(foreignTypeParts.Length - 1).DefaultIfEmpty().Aggregate((x, y) => x + "." + y);
            if (usingPaths.Contains(foreignTypeQualifier) && usingPaths.All(x => x != foreignType) && !localNamespaceParts.Contains(foreignTypeParts.Last()))
            {
                return foreignTypeParts.Last();
            }

            var otherPathsToCheck = knownOtherPaths
                .Concat(new[] { localNamespace })
                .Concat(usingPaths)
                .Distinct()
                .ToArray();


            // To minimize the chance that simplifying the path of the foreign type causes a compile time error due to a
            // conflicting path, we pre-compute known sub paths for each part of the namespace.To try summarize the logic,
            // for each part of the local namespace, find all their respective immediate sub path parts and select with
            // some other data for easier debugging.
            var namespacePartsSubPaths = localNamespaceParts
                .Select((localNamespacePart, index) =>
                {
                    var namespacePartPath = localNamespaceParts
                        .Take(index + 1)
                        .Aggregate((x, y) => x + "." + y);

                    return new
                    {
                        LocalNamespacePartPath = namespacePartPath,
                        LocalNamespacePartSubPaths = otherPathsToCheck
                            .Where(y => y.StartsWith(namespacePartPath + "."))
                            .Select(otherPath => new
                            {
                                OtherPathSubPart = otherPath.Substring((namespacePartPath + ".").Length).Split('.').First(),
                                OtherPathFull = otherPath,
                                LocalNamespacePartPath = namespacePartPath,
                            })
                            .GroupBy(x => x.OtherPathSubPart)
                            .ToDictionary(x => x.Key, x => x.ToList())
                    };
                })
                .Where(x => x.LocalNamespacePartSubPaths.Any())
                .ToArray();

            var commonPartsCount = 0;
            for (var i = 0; i < localNamespaceParts.Length && i < foreignTypeParts.Length; i++)
            {
                var localPart = localNamespaceParts[i];
                var foreignPart = foreignTypeParts[i];
                var proposedFirstForeignPart = foreignTypeParts.Skip(i + 1).FirstOrDefault();
                var proposedPathToOmit = foreignTypeParts.Take(i + 1).Aggregate((x, y) => x + "." + y);

                // Simple check first:
                if (localPart != foreignPart)
                {
                    break;
                }

                var conflicts = namespacePartsSubPaths
                    // C# gives precendence to resolving types from the most to the least specific from the namespace
                    .Skip(i + 1)

                    // For namespaces with a matching sub-part:
                    .Where(x => proposedFirstForeignPart != null && x.LocalNamespacePartSubPaths.ContainsKey(proposedFirstForeignPart))

                    // Select filtered results (for easier debugging):
                    .Select(x => new
                    {
                        x.LocalNamespacePartPath,
                        Conflicts = x.LocalNamespacePartSubPaths[proposedFirstForeignPart]
                            .Where(y => y.LocalNamespacePartPath != proposedPathToOmit)
                            .ToArray()
                    })

                    // Where not empty:
                    .Where(x => x.Conflicts.Any())
                    .ToArray();

                if (conflicts.Any())
                {
                    break;
                }

                commonPartsCount++;
            }

            return foreignTypeParts.Skip(commonPartsCount).Aggregate((x, y) => x + "." + y);
        }

        private static IEnumerable<string> GetUsingsFromContent(string existingContent)
        {
            var lines = existingContent
                .Replace("\r\n", "\n")
                .Split('\n');

            var relevantContent = new List<string>();
            foreach (var line in lines)
            {
                if (line.TrimStart().StartsWith("using ") && !line.Contains("="))
                {
                    relevantContent.Add(line
                        .Replace(";", string.Empty)
                        .Replace("using ", string.Empty)
                        .Trim());
                }

                // GCB - using clauses can exist after the namespace is declared. Rather check until class is declared.
                if (line.Contains("class "))
                {
                    break;
                }
            }

            return relevantContent;
        }

        public virtual RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, new TemplateVersion(1,0)));
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return DefineFileConfig();
        }

        protected abstract CSharpFileConfig DefineFileConfig();

        /// <summary>
        /// Returns all using statements that are introduced through dependencies.
        /// </summary>
        public virtual string DependencyUsings => this.ResolveAllUsings(ExecutionContext, namespacesToIgnore: Namespace);

        private readonly ICollection<INugetPackageInfo> _nugetDependencies = new List<INugetPackageInfo>();
        public virtual IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return _nugetDependencies;
        }

        /// <summary>
        /// Registers that the specified NuGet package should be installed in the csproj file where this file resides.
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="packageVersion"></param>
        /// <returns></returns>
        public NugetPackageInfo AddNugetDependency(string packageName, string packageVersion)
        {
            var package = new NugetPackageInfo(packageName, packageVersion);
            _nugetDependencies.Add(package);
            return package;
        }

        /// <summary>
        /// Registers that the specified NuGet package should be installed in the csproj file where this file resides.
        /// </summary>
        /// <param name="nugetPackageInfo"></param>
        public void AddNugetDependency(INugetPackageInfo nugetPackageInfo)
        {
            _nugetDependencies.Add(nugetPackageInfo);
        }



        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return _assemblyDependencies;
        }

        /// <summary>
        /// Registers that the specified GAC assembly should be installed in the csproj file where this file resides.
        /// </summary>
        /// <param name="assemblyReference"></param>
        public void AddAssemblyReference(IAssemblyReference assemblyReference)
        {
            _assemblyDependencies.Add(assemblyReference);
        }

        public IEnumerable<string> DeclareUsings()
        {
            return _additionalUsingNamespaces;
        }
    }
}