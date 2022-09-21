using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.FactoryExtensions;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.VisualStudio;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates
{
    /// <inheritdoc />
    public abstract class CSharpTemplateBase : CSharpTemplateBase<object>
    {
        /// <summary>
        /// Creates a new instance of <see cref="CSharpTemplateBase"/>.
        /// </summary>
        protected CSharpTemplateBase(string templateId, IOutputTarget outputTarget) : base(templateId, outputTarget, null)
        {
        }
    }

    /// <inheritdoc cref="CSharpTemplateBase{TModel}"/>
    public abstract class CSharpTemplateBase<TModel, TDecorator> : CSharpTemplateBase<TModel>, IHasDecorators<TDecorator>
        where TDecorator : ITemplateDecorator
    {
        private readonly ICollection<TDecorator> _decorators = new List<TDecorator>();

        /// <summary>
        /// Creates a new instance of <see cref="CSharpTemplateBase{TModel,TDecorator}"/>.
        /// </summary>
        protected CSharpTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }

        /// <inheritdoc />
        public IEnumerable<TDecorator> GetDecorators()
        {
            return _decorators.OrderBy(x => x.Priority);
        }

        /// <inheritdoc />
        public void AddDecorator(TDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        /// <summary>
        /// Aggregates the specified <paramref name="propertyFunc"/> property of all decorators.
        /// </summary>
        /// <remarks>
        /// Ignores Decorators where the property returns null.
        /// </remarks>
        protected string GetDecoratorsOutput(Func<TDecorator, string> propertyFunc)
        {
            return GetDecorators().Aggregate(propertyFunc);
        }
    }

    /// <summary>
    /// Template base for C# files, which invokes code-management to make updates to existing files.
    /// <para>
    /// Learn more about templates in
    /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common.csharp&amp;additionalData=templates">
    /// this article</seealso>.
    /// </para>
    /// </summary>
    public abstract class CSharpTemplateBase<TModel> : IntentTemplateBase<TModel>, ICSharpTemplate, IHasNugetDependencies, IHasAssemblyDependencies, IClassProvider, IRoslynMerge, IDeclareUsings, IHasFrameworkDependencies
    {
        private readonly ICollection<IAssemblyReference> _assemblyDependencies = new List<IAssemblyReference>();
        private readonly HashSet<string> _additionalUsingNamespaces = new();
        private IEnumerable<string> _templateUsings;
        private IEnumerable<string> _existingContentUsings;

        /// <summary>
        /// Creates a new instance of <see cref="CSharpTemplateBase{TModel}"/>.
        /// </summary>
        protected CSharpTemplateBase(string templateId, IOutputTarget outputTarget, TModel model)
            : base(templateId, outputTarget, model)
        {
            Types = new CSharpTypeResolver(
                defaultCollectionFormatter: CSharpCollectionFormatter.Create("System.Collections.Generic.IEnumerable<{0}>"),
                defaultNullableFormatter: CSharpNullableFormatter.Create(OutputTarget.GetProject()));
        }

        /// <inheritdoc cref="IntentTemplateBase.GetTypeInfo(IClassProvider)"/>
        protected new CSharpResolvedTypeInfo GetTypeInfo(IClassProvider classProvider)
        {
            return (CSharpResolvedTypeInfo)base.GetTypeInfo(classProvider);
        }

        /// <summary>
        /// Returns the <see cref="IOutputTarget"/> for the .csproj file that contains this file.
        /// </summary>
        public IOutputTarget Project => OutputTarget.GetProject();

        /// <summary>
        /// Returns the class' namespace as specified in the <see cref="CSharpFileConfig"/>.
        /// Escapes any invalid characters and enforces pascal-case. May be overriden.
        /// </summary>
        public virtual string Namespace
        {
            get
            {
                if (!FileMetadata.CustomMetadata.TryGetValue("Namespace", out var @namespace))
                {
                    @namespace = OutputTarget.Name;
                }

                // Deliberately assume formatting wanted if not specified
                if (!FileMetadata.CustomMetadata.TryGetValue(
                        key: nameof(CSharpFileConfig.ApplyNamespaceFormatting),
                        value: out var value) ||
                    bool.TryParse(value, out var parsedBool) &&
                    parsedBool)
                {
                    @namespace = @namespace.ToCSharpNamespace();
                }

                return @namespace;
            }
        }

        /// <summary>
        /// Returns the class name as specified in the <see cref="CSharpFileConfig"/>. Escapes
        /// any invalid characters and enforces pascal-case. May be overriden.
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
        /// Add the using clause with the specified <paramref name="namespace"/> to this template's file.
        /// </summary>
        public void AddUsing(string @namespace)
        {
            _additionalUsingNamespaces.Add(@namespace);
        }

        /// <summary>
        /// Adds the <paramref name="namespace"/> as a dependent using clause and returns the <paramref name="name"/>.
        /// </summary>
        public string UseType(string name, string @namespace)
        {
            AddUsing(@namespace);
            return name;
        }

        /// <summary>
        /// Adds the namespace of the <paramref name="fullName"/> as a dependent namespace and returns the normalized name.
        /// </summary>
        public string UseType(string fullName)
        {
            var elements = new List<string>(fullName.Split(".", StringSplitOptions.RemoveEmptyEntries));
            elements.RemoveAt(elements.Count - 1);
            AddUsing(string.Join(".", elements));
            return NormalizeNamespace(fullName);
        }

        /// <inheritdoc />
        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            (this as ICSharpFileBuilderTemplate)?.CSharpFile.StartBuild();
        }

        /// <inheritdoc />
        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            (this as ICSharpFileBuilderTemplate)?.CSharpFile.FinalizeBuild();
        }

        /// <inheritdoc />
        public override string RunTemplate()
        {
            var templateOutput = base.RunTemplate().TrimStart();
            var dependencyUsings = DependencyUsings;

            return dependencyUsings == string.Empty
                ? templateOutput
                : $"{dependencyUsings}{Environment.NewLine}{templateOutput}";
        }

        /// <inheritdoc cref="IntentTemplateBase.AddTypeSource(string)"/>
        [FixFor_Version4("Remove this method and let the override below do the necessary work")]
        public new ClassTypeSource AddTypeSource(string templateId)
        {
            return base.AddTypeSource(templateId);
        }

        /// <inheritdoc cref="IntentTemplateBase.AddTypeSource(string,string)"/>
        [FixFor_Version4(
            "Change this to an override which returns ClassTypeSource and make collectionFormat " +
            "have a default value of null")]
        public new void AddTypeSource(string templateId, string collectionFormat)
        {
            base.AddTypeSource(templateId, collectionFormat);
        }

        /// <inheritdoc />
        protected override ICollectionFormatter CreateCollectionFormatter(string collectionFormat)
        {
            return CSharpCollectionFormatter.Create(collectionFormat);
        }

        /// <inheritdoc />
        public override string NormalizeTypeName(string name)
        {
            return NormalizeNamespace(name);
        }

        /// <summary>
        /// Converts the namespace of a fully qualified class name to the relative namespace for this class instance.
        /// </summary>
        /// <param name="foreignType">The foreign type which is ideally fully qualified</param>
        public virtual string NormalizeNamespace(string foreignType)
        {
            foreignType = foreignType.Trim();

            var isNullable = false;
            if (foreignType.EndsWith("?", StringComparison.OrdinalIgnoreCase))
            {
                isNullable = true;
                foreignType = foreignType[..^1];
            }

            // Handle Generics recursively:
            string normalizedGenericTypes = null;
            if (foreignType.Contains("<") && foreignType.Contains(">"))
            {
                var genericTypes = foreignType.Substring(foreignType.IndexOf("<", StringComparison.Ordinal) + 1, foreignType.Length - foreignType.IndexOf("<", StringComparison.Ordinal) - 2);

                normalizedGenericTypes = genericTypes
                    .Split(',')
                    .Select(NormalizeNamespace)
                    .Aggregate((x, y) => x + ", " + y);
                foreignType = $"{foreignType[..foreignType.IndexOf("<", StringComparison.Ordinal)]}";
            }

            var usingPaths = DependencyUsings
                .Split(';')
                .Select(x => x.Trim().Replace("using ", ""))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Concat(_templateUsings ??= GetUsingsFromContent(GenerationEnvironment?.ToString() ?? string.Empty))
                .Concat(_existingContentUsings ??= GetUsingsFromContent(TryGetExistingFileContent(out var existingContent) ? existingContent : string.Empty))
                .Distinct()
                .ToArray();
            var localNamespace = Namespace;
            var knownOtherPaths = usingPaths
                .Concat(ExecutionContext.OutputTargets.Select(x => x.Name))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray();

            var nullable = isNullable ? "?" : string.Empty;

            return NormalizeNamespace(
                       localNamespace: localNamespace,
                       fullyQualifiedType: foreignType,
                       knownOtherPaths: knownOtherPaths,
                       usingPaths: usingPaths,
                       knownTypesByNamespace: KnownCSharpTypesCache.GetKnownTypesByNamespace()) +
                   (normalizedGenericTypes != null ? $"<{normalizedGenericTypes}>{nullable}" : nullable);
        }

        internal static string NormalizeNamespace(
            string localNamespace,
            string fullyQualifiedType,
            string[] knownOtherPaths,
            string[] usingPaths,
            IReadOnlyDictionary<string, ISet<string>> knownTypesByNamespace)
        {
            // NB: If changing this method, please run the unit tests against it

            var localNamespaceParts = localNamespace.Split('.').ToArray();

            var typeParts = fullyQualifiedType.Split('.').ToArray();
            if (typeParts.Length == 1)
            {
                return fullyQualifiedType;
            }

            if (localNamespaceParts.SequenceEqual(typeParts.Take(typeParts.Length - 1)))
            {
                return typeParts.Last();
            }

            var otherPathsToCheck = knownOtherPaths
                .Append(localNamespace)
                .Concat(usingPaths)
                .Distinct()
                .ToArray();

            // Is there already a using which matches qualifier:
            {
                var typeQualifier = string.Join('.', typeParts.Take(typeParts.Length - 1));
                var typeName = typeParts.Last();

                // It's not immediately clear what scenario this covers, if you know/find out, please
                // document by adding unit test to cover scenario.
                bool AllUsingPathsAreNotForeignType() => usingPaths.All(x => x != fullyQualifiedType);

                // If name exists in local namespace, can't use name as is.
                bool LocalNamespacePartsDoNotContainType() => !localNamespaceParts.Contains(typeName);

                // Ensure there are no other known types in usings with the same name:
                bool ConflictingTypeExists() => usingPaths
                    .Any(usingPath => usingPath != typeQualifier &&
                                      knownTypesByNamespace.TryGetValue(usingPath, out var types) &&
                                      types.Contains(typeName));

                if (usingPaths.Contains(typeQualifier) &&
                    AllUsingPathsAreNotForeignType() &&
                    LocalNamespacePartsDoNotContainType() &&
                    !ConflictingTypeExists())
                {
                    return typeName;
                }
            }

            // To minimize the chance that simplifying the path of the foreign type causes a compile time error due to a
            // conflicting path, we pre-compute known sub paths for each part of the namespace. To try summarize the logic,
            // for each part of the local namespace, find all their respective immediate sub path parts and select with
            // some other data for easier debugging.
            var namespacePartsSubPaths = localNamespaceParts
                .Select((_, index) =>
                {
                    var namespacePartPath = string.Join('.', localNamespaceParts.Take(index + 1));

                    return new
                    {
                        LocalNamespacePartPath = namespacePartPath,
                        LocalNamespacePartSubPaths = otherPathsToCheck
                            .Where(y => y.StartsWith(namespacePartPath + "."))
                            .Select(otherPath => new
                            {
                                OtherPathSubPart = otherPath[(namespacePartPath + ".").Length..].Split('.').First(),
                                OtherPathFull = otherPath,
                                LocalNamespacePartPath = namespacePartPath,
                            })
                            .GroupBy(x => x.OtherPathSubPart)
                            .ToDictionary(x => x.Key, x => x.ToList())
                    };
                })
                .Where(x => x.LocalNamespacePartSubPaths.Count > 0)
                .ToArray();

            var commonPartsCount = 0;
            for (var i = 0; i < localNamespaceParts.Length && i < typeParts.Length; i++)
            {
                var localPart = localNamespaceParts[i];
                var foreignPart = typeParts[i];
                var proposedFirstForeignPart = typeParts.Skip(i + 1).FirstOrDefault();
                var proposedPathToOmit = string.Join('.', typeParts.Take(i + 1));

                // Simple check first:
                if (localPart != foreignPart)
                {
                    break;
                }

                if (proposedFirstForeignPart == null)
                {
                    commonPartsCount++;
                    continue;
                }

                var conflicts = namespacePartsSubPaths
                    // C# gives precedence to resolving types from the most to the least specific from the namespace
                    .Skip(i + 1)

                    // For namespaces with a matching sub-part:
                    .Where(x => x.LocalNamespacePartSubPaths.ContainsKey(proposedFirstForeignPart))

                    // Select filtered results (for easier debugging):
                    .Select(x => new
                    {
                        x.LocalNamespacePartPath,
                        Conflicts = x.LocalNamespacePartSubPaths[proposedFirstForeignPart]
                            .Where(y => y.LocalNamespacePartPath != proposedPathToOmit)
                            .ToArray()
                    })

                    // Where not empty:
                    .Where(x => x.Conflicts.Length != 0)
                    .ToArray();

                if (conflicts.Length != 0)
                {
                    break;
                }

                commonPartsCount++;
            }

            return string.Join('.', typeParts.Skip(commonPartsCount));
        }

        private static IEnumerable<string> GetUsingsFromContent(string existingContent)
        {
            if (string.IsNullOrWhiteSpace(existingContent))
            {
                return Enumerable.Empty<string>();
            }

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

        /// <inheritdoc />
        public virtual RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, new TemplateVersion(1, 0)));
        }

        /// <inheritdoc />
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return DefineFileConfig();
        }

        /// <summary>
        /// Returns a string representation of the provided <paramref name="resolvedTypeInfo"/>,
        /// adds any required usings, applicable template dependencies and makes a best effort to
        /// avoid conflicts between the type name and known other types and namespaces.
        /// </summary>
        public override string UseType(IResolvedTypeInfo resolvedTypeInfo)
        {
            if (resolvedTypeInfo is not CSharpResolvedTypeInfo cSharpResolvedTypeInfo)
            {
                return base.UseType(resolvedTypeInfo);
            }

            // Adds template usings etc, but we ignore the returned string since we will do different logic
            base.UseType(resolvedTypeInfo);

            foreach (var @namespace in cSharpResolvedTypeInfo.GetNamespaces())
            {
                AddUsing(@namespace);
            }

            var fullyQualifiedTypeName = cSharpResolvedTypeInfo.GetFullyQualifiedTypeName();

            return NormalizeNamespace(fullyQualifiedTypeName);
        }

        /// <summary>
        /// Factory method for creating a <see cref="CSharpFileConfig"/> for a template.
        /// </summary>
        /// <remarks>
        /// <see cref="CSharpFileConfig"/> is used to specify configuration such as its
        /// type name, namespace and relative output location.
        /// </remarks>
        protected abstract CSharpFileConfig DefineFileConfig();

        /// <summary>
        /// Returns all using statements that are introduced through dependencies.
        /// </summary>
        public virtual string DependencyUsings => this.ResolveAllUsings(namespacesToIgnore: Namespace);

        private readonly ICollection<INugetPackageInfo> _nugetDependencies = new List<INugetPackageInfo>();

        /// <inheritdoc />
        public virtual IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return _nugetDependencies;
        }

        /// <summary>
        /// Registers that the specified NuGet package should be installed in the csproj file where this file resides.
        /// </summary>
        public NugetPackageInfo AddNugetDependency(string packageName, string packageVersion)
        {
            var package = new NugetPackageInfo(packageName, packageVersion);
            _nugetDependencies.Add(package);
            return package;
        }

        /// <summary>
        /// Registers that the specified NuGet package should be installed in the .csproj file where this file resides.
        /// </summary>
        public void AddNugetDependency(INugetPackageInfo nugetPackageInfo)
        {
            _nugetDependencies.Add(nugetPackageInfo);
        }

        /// <summary>
        /// Registers that a <c>.csproj</c> containing a Role named <paramref name="roleName"/>
        /// should be a dependency of the <c>.csproj</c> where this file resides.
        /// </summary>
        public void AddProjectDependency(string roleName)
        {
            var project = ExecutionContext.OutputTargets
                .SingleOrDefault(x => x.HasRole(roleName));
            if (project == null)
            {
                throw new Exception($"Could not find project with role {roleName}");
            }

            OutputTarget.GetProject().AddDependency(project.GetProject());
        }

        private readonly ICollection<string> _frameworkDependency = new HashSet<string>();

        /// <summary>
        /// Registers that the specified <FrameworkReference/> element should be add in the .csproj file where this file resides.
        /// </summary>
        public void AddFrameworkDependency(string id)
        {
            _frameworkDependency.Add(id);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFrameworkDependencies()
        {
            return _frameworkDependency;
        }

        /// <inheritdoc />
        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return _assemblyDependencies;
        }

        /// <summary>
        /// Registers that the specified GAC assembly should be installed in the .csproj file where this file resides.
        /// </summary>
        public void AddAssemblyReference(IAssemblyReference assemblyReference)
        {
            _assemblyDependencies.Add(assemblyReference);
        }

        /// <inheritdoc />
        public IEnumerable<string> DeclareUsings()
        {
            return _additionalUsingNamespaces;
        }

        #region GetFullyQualifiedTypeName

        /// <summary>
        /// Resolves the fully qualified type name for the provided <paramref name="classProvider"/>
        /// parameter.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        protected virtual string GetFullyQualifiedTypeName(IClassProvider classProvider)
        {
            var resolvedTypeInfo = GetTypeInfo(classProvider);

            return GetFullyQualifiedTypeName(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the fully qualified type name for the provided <paramref name="element"/>
        /// parameter.
        /// 
        /// Any source added by <see cref="IntentTemplateBase.AddTypeSource(ITypeSource)"/> or <see cref="AddTypeSource(string,string)"/>
        /// will be searched to resolve the type name.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="element">The <see cref="IElement"/> for which to get the type name.</param>
        public string GetFullyQualifiedTypeName(IElement element)
        {
            var resolvedTypeInfo = GetTypeInfo(element);

            return GetFullyQualifiedTypeName(resolvedTypeInfo);
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetFullyQualifiedTypeName(IElement)"/> instead.
        /// </summary>
        /// <remarks>
        /// Even before this method was marked as obsolete, the <paramref name="collectionFormat"/>
        /// value actually had no effect.
        /// </remarks>
        [Obsolete(WillBeRemovedIn.Version4)]
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public string GetFullyQualifiedTypeName(IElement element, string collectionFormat = null)
        {
            return GetFullyQualifiedTypeName(element);
        }

        /// <summary>
        /// Resolves the fully qualified type name for the provided <paramref name="hasTypeReference"/>
        /// parameter.
        /// 
        /// Any source added by <see cref="IntentTemplateBase.AddTypeSource(ITypeSource)"/> or <see cref="AddTypeSource(string,string)"/>
        /// will be searched to resolve the type name.
        /// 
        /// Applies the <paramref name="collectionFormat"/> if the resolved type's <see cref="ITypeReference.IsCollection"/>
        /// is true.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="hasTypeReference">The <see cref="IHasTypeReference"/> for which to get the type name.</param>
        /// <param name="collectionFormat">The collection format to be applied if the resolved type <see cref="ITypeReference.IsCollection"/> is true.</param>
        public string GetFullyQualifiedTypeName(IHasTypeReference hasTypeReference, string collectionFormat = null)
        {
            var resolvedTypeInfo = GetTypeInfo(hasTypeReference.TypeReference, collectionFormat);

            return GetFullyQualifiedTypeName(resolvedTypeInfo);
        }

        private string GetFullyQualifiedTypeName(IResolvedTypeInfo resolvedTypeInfo)
        {
            base.UseType(resolvedTypeInfo);

            return resolvedTypeInfo is CSharpResolvedTypeInfo cSharpResolvedTypeInfo
                ? cSharpResolvedTypeInfo.GetFullyQualifiedTypeName()
                : resolvedTypeInfo.ToString();
        }

        /// <summary>
        /// Resolves the fully qualified type name for the provided <paramref name="template"/>
        /// parameter.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="template">The <see cref="ITemplate"/> for which to get the type name.</param>
        /// <param name="options"><see cref="TemplateDiscoveryOptions"/> to use.</param>
        public string GetFullyQualifiedTypeName(ITemplate template, TemplateDiscoveryOptions options = null)
        {
            var resolvedTypeInfo = GetTypeInfo(template, options);

            return GetFullyQualifiedTypeName(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the fully qualified type name for the provided <paramref name="templateDependency"/>
        /// parameter.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateDependency">The <see cref="ITemplateDependency"/> for which to get the type name.</param>
        /// <param name="options"><see cref="TemplateDiscoveryOptions"/> to use.</param>
        public string GetFullyQualifiedTypeName(ITemplateDependency templateDependency, TemplateDiscoveryOptions options = null)
        {
            var resolvedTypeInfo = GetTypeInfo(templateDependency, options);

            return GetFullyQualifiedTypeName(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the fully qualified type name for the provided <paramref name="typeReference"/> parameter.
        /// Any added <see cref="ITypeSource"/> by <see cref="IntentTemplateBase.AddTypeSource(ITypeSource)"/> will be
        /// searched to resolve the type name.
        /// Applies the <paramref name="collectionFormat"/> if the resolved type's <see cref="ITypeReference.IsCollection"/> is true.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="typeReference">The <see cref="ITypeReference"/> for which to get the type name.</param>
        /// <param name="collectionFormat">The collection format to be applied if the resolved type <see cref="ITypeReference.IsCollection"/> is true</param>
        public string GetFullyQualifiedTypeName(ITypeReference typeReference, string collectionFormat = null)
        {
            var resolvedTypeInfo = GetTypeInfo(typeReference, collectionFormat);

            return GetFullyQualifiedTypeName(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the fully qualified type name for the provided <paramref name="templateId"/>
        /// and <paramref name="model"/> parameters.
        /// 
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the provided <paramref name="model"/>.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="model">The model instance that the Template must be bound to.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        public string GetFullyQualifiedTypeName(string templateId, IMetadataModel model, TemplateDiscoveryOptions options = null)
        {
            var resolvedTypeInfo = GetTypeInfo(templateId, model, options);

            return GetFullyQualifiedTypeName(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the fully qualified type name for the provided <paramref name="templateId"/>
        /// and <paramref name="modelId"/> parameters.
        /// 
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the provided <paramref name="modelId"/>.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="modelId">The identifier of the model that the Template must be bound to.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        public string GetFullyQualifiedTypeName(string templateId, string modelId, TemplateDiscoveryOptions options = null)
        {
            var resolvedTypeInfo = GetTypeInfo(templateId, modelId, options);

            return GetFullyQualifiedTypeName(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the fully qualified type name for the provided <paramref name="templateId"/>
        /// parameter.
        /// 
        /// This overload assumes that the Template only has a single instance and will throw an
        /// exception if more than one is found.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        public string GetFullyQualifiedTypeName(string templateId, TemplateDiscoveryOptions options = null)
        {
            var resolvedTypeInfo = GetTypeInfo(templateId, options);

            return GetFullyQualifiedTypeName(resolvedTypeInfo);
        }

        #endregion
    }
}