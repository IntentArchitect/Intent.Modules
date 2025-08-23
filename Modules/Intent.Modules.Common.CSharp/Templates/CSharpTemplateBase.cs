#nullable enable
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.FactoryExtensions;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.VisualStudio;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Modules.Common.CSharp.Utils;

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
        protected CSharpTemplateBase(string templateId, IOutputTarget outputTarget, TModel? model) : base(templateId, outputTarget, model)
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
    public abstract class CSharpTemplateBase<TModel> :
        IntentTemplateBase<TModel>,
        ICSharpTemplate,
        IDeclareUsings,
#pragma warning disable CS0618 // Type or member is obsolete
        IRoslynMerge
#pragma warning restore CS0618 // Type or member is obsolete
    {
        private readonly ICollection<IAssemblyReference> _assemblyDependencies = new List<IAssemblyReference>();
        private readonly HashSet<string> _additionalUsingNamespaces = new();
        private IEnumerable<string>? _templateUsings;
        private IEnumerable<string>? _existingContentUsings;

        /// <summary>
        /// Creates a new instance of <see cref="CSharpTemplateBase{TModel}"/>.
        /// </summary>
        protected CSharpTemplateBase(string templateId, IOutputTarget outputTarget, TModel? model)
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

                // For backwards compatibility formatting is opt-out
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
                    return FileMetadata.CustomMetadata["ClassName"].ToCSharpIdentifier(CapitalizationBehaviour.AsIs);
                }
                return FileMetadata.FileName.ToCSharpIdentifier(CapitalizationBehaviour.AsIs);
            }
        }

        /// <summary>
        /// Adds the <paramref name="fullyQualifiedTypeName"/> as a known type for purposes of
        /// being able to disambiguating type references in cases where multiple using directives
        /// have types with the same.
        /// </summary>
        public void AddKnownType(string fullyQualifiedTypeName)
        {
            CSharpTypesCache.AddKnownType(fullyQualifiedTypeName);
        }

        public virtual ICSharpCodeContext? RootCodeContext => this is ICSharpFileBuilderTemplate builder
            ? builder.CSharpFile.TypeDeclarations.FirstOrDefault() ?? builder.CSharpFile.Interfaces.FirstOrDefault() ?? (ICSharpCodeContext)builder.CSharpFile
            : null;

        /// <summary>
        /// Add the using clause with the specified <paramref name="namespace"/> to this template's file.
        /// </summary>
        public void AddUsing(string @namespace)
        {
            _additionalUsingNamespaces.Add(@namespace);
        }

        /// <summary>
        /// Remove the using clause with the specified <paramref name="namespace"/> to this template's file.
        /// </summary>
        public void RemoveUsing(string @namespace)
        {
            _additionalUsingNamespaces.Remove(@namespace);
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
        /// Adds the namespace parts of the <paramref name="fullName"/> as a dependent namespace and returns the normalized name.
        /// </summary>
        public string UseType(string fullName)
        {
            var sb = new StringBuilder(fullName.Length);
            var namespaceLength = -1;

            foreach (var c in fullName)
            {
                if (c is ',' or '<' or '>')
                {
                    if (namespaceLength > 0)
                    {
                        sb.Length = namespaceLength;
                        AddUsing(sb.ToString());
                    }

                    namespaceLength = -1;
                    sb.Length = 0;
                    continue;
                }

                if (c is ' ')
                {
                    continue;
                }

                if (c is '.')
                {
                    namespaceLength = sb.Length;
                }

                sb.Append(c);
            }

            if (namespaceLength > 0)
            {
                sb.Length = namespaceLength;
                AddUsing(sb.ToString());
            }

            return NormalizeNamespace(fullName);
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
        public new virtual ClassTypeSource AddTypeSource(string templateId)
        {
            return base.AddTypeSource(templateId);
        }

        /// <inheritdoc cref="IntentTemplateBase.AddTypeSource(string,string)"/>
        [FixFor_Version4(
            "Change this to an override which returns ClassTypeSource and make collectionFormat " +
            "have a default value of null")]
        public new virtual void AddTypeSource(string templateId, string collectionFormat)
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
            var alteredForeignType = foreignType.Trim();

            var usingPaths = ResolveAllUsings()
                .Concat(_templateUsings ??= GetUsingsFromContent(GenerationEnvironment?.ToString() ?? string.Empty))
                .Concat(_existingContentUsings ??= GetUsingsFromContent(TryGetExistingFileContent(out var existingContent) ? existingContent : string.Empty))
                .Concat(_additionalUsingNamespaces)
                .Concat(CSharpTypesCache.GetGlobalUsings(this))
                // ReSharper disable once SuspiciousTypeConversion.Global
                .Concat((this as ICSharpFileBuilderTemplate)?.CSharpFile.Usings.Select(u => u.Namespace) ?? [])
                .Distinct()
                .ToArray();
            var localNamespace = Namespace;
            var knownOtherPaths = usingPaths
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray();
            
            if (GenericCSharpTypeParser.TryParse(alteredForeignType, name =>
                {
                    if (name is null)
                    {
                        return null;
                    }

                    return InternalNormalizeNamespace(name);
                }, out var parsed))
            {
                alteredForeignType = parsed!;
            }

            return alteredForeignType;

            string InternalNormalizeNamespace(string fullTypeName)
            {
                var normalizedType = NormalizeNamespace(
                    localNamespace: localNamespace,
                    fullyQualifiedType: fullTypeName,
                    knownOtherNamespaceNames: knownOtherPaths,
                    usingPaths: usingPaths,
                    outputTargetNames: CSharpTypesCache.GetOutputTargetNames(),
                    knownTypes: CSharpTypesCache.GetKnownTypes());
                return normalizedType;
            }
        }

        private class CompositeRegistry
        {
            private readonly TypeRegistry[] _registries;
            private readonly string[] _fullyQualifiedTypeParts;

            public CompositeRegistry(
                string[] fullyQualifiedTypeParts,
                IEnumerable<string> namespaces,
                IEnumerable<TypeRegistry> registries)
            {
                var pathsRegistry = new TypeRegistry().WithNamespaces(namespaces);
                _registries = registries.Append(pathsRegistry).ToArray();

                // This is the type that we're going to be normalizing so
                // needs to be excluded as a match always.
                _fullyQualifiedTypeParts = fullyQualifiedTypeParts;
            }

            public bool ContainsAny(Span<string> namespaceParts, string? typeName)
            {
                return Contains(namespaceParts, typeName, out _);
            }

            public bool ContainsType(Span<string> namespaceParts, string? typeName)
            {
                return Contains(namespaceParts, typeName, out var isType) && isType;
            }

            private bool Contains(Span<string> namespaceParts, string? typeName, out bool isType)
            {
                // Check that the type we're comparing against isn't the type we're excluding
                if (typeName == null)
                {
                    var partsToCheck = namespaceParts.Length < _fullyQualifiedTypeParts.Length
                        ? _fullyQualifiedTypeParts.AsSpan(0, namespaceParts.Length)
                        : _fullyQualifiedTypeParts.AsSpan();

                    if (namespaceParts.SequenceEqual(partsToCheck))
                    {
                        isType = default;
                        return false;
                    }
                }
                else
                {
                    var partsToCheck = namespaceParts.Length + 1 < _fullyQualifiedTypeParts.Length
                        ? _fullyQualifiedTypeParts.AsSpan(0, namespaceParts.Length)
                        : _fullyQualifiedTypeParts.AsSpan(0, _fullyQualifiedTypeParts.Length - 1);

                    
                    if (_fullyQualifiedTypeParts.Length > namespaceParts.Length &&
                        typeName == _fullyQualifiedTypeParts[namespaceParts.Length] &&
                        namespaceParts.SequenceEqual(partsToCheck))
                    {
                        isType = default;
                        return false;
                    }
                }

                foreach (var registry in _registries)
                {
                    if (registry.Contains(namespaceParts, typeName, out isType, out _))
                    {
                        return true;
                    }
                }

                isType = default;
                return false;
            }
        }

        internal static string NormalizeNamespace(
            string localNamespace,
            string fullyQualifiedType,
            string[] knownOtherNamespaceNames,
            string[] usingPaths,
            TypeRegistry outputTargetNames,
            TypeRegistry knownTypes)
        {
            ArgumentNullException.ThrowIfNull(fullyQualifiedType);
            
            // NB: If changing this method, please run the unit tests against it

            var fullyQualifiedTypeParts = fullyQualifiedType.Split('.');
            if (fullyQualifiedTypeParts.Length == 1)
            {
                return fullyQualifiedType;
            }

            var typeNamespaceSpan = fullyQualifiedTypeParts.AsSpan(0, fullyQualifiedTypeParts.Length - 1);
            var typePartsToReturn = fullyQualifiedTypeParts.AsSpan(typeNamespaceSpan.Length, 1);

            // Qualify nested types:
            // (This is presently unaware of not needing to qualify
            // nested types from within the same type.)
            while (typeNamespaceSpan.Length > 0 &&
                   knownTypes.IsNestedType(typePartsToReturn, typePartsToReturn[0]))
            {
                typeNamespaceSpan = fullyQualifiedTypeParts.AsSpan(0, typeNamespaceSpan.Length - 1);
                typePartsToReturn = fullyQualifiedTypeParts.AsSpan(typeNamespaceSpan.Length, 1);
            }

            var originalTypeNamespaceParts = typeNamespaceSpan.ToArray();
            var originalTypeParts = typePartsToReturn.ToArray();

            var otherPaths = Enumerable.Empty<string>()
                .Append(localNamespace)
                .Append(string.Join('.', originalTypeNamespaceParts))
                .Concat(knownOtherNamespaceNames)
                .Concat(usingPaths)
                .Where(x => x != fullyQualifiedType)
                .Distinct()
                .ToArray();

            var typeRegistry = new CompositeRegistry(
                fullyQualifiedTypeParts: fullyQualifiedTypeParts,
                namespaces: otherPaths,
                registries:
                [
                    outputTargetNames,
                    knownTypes
                ]);


            var localNamespaceParts = !string.IsNullOrWhiteSpace(localNamespace)
                ? localNamespace.Split('.')
                : [];

            var commonNamespacePartsCount = originalTypeNamespaceParts
                .Zip(localNamespaceParts)
                .TakeWhile(x => x.First == x.Second)
                .Count();

            // C# always tries to resolve first from the namespace parts, moving from the most to
            // the least specific part (or gives precedence to using directives inside the
            // namespace, but Intent at this time isn't aware of them).
            var hasConflict = false;
            var typePartsToSkip = typeNamespaceSpan.Length;
            for (var partIndex = localNamespaceParts.Length - 1; partIndex >= 0; partIndex--)
            {
                // If no conflict and the type we're looking for is within the current part of the
                // namespace then we're golden:
                if (!hasConflict &&
                    localNamespaceParts.AsSpan(0, partIndex + 1).SequenceEqual(originalTypeNamespaceParts))
                {
                    return PrefixWithGlobalMaybe(string.Join('.', typePartsToReturn.ToArray()));
                }

                // We need to also check on each namespace part from most to least significant
                // that it doesn't contain the first part of our typeParts to return. We don't
                // check when we're in common namespace count region as that will always have
                // itself as a conflict
                for (var innerIndex = localNamespaceParts.Length - 1;
                     innerIndex >= 0;
                     innerIndex--)
                {

                    var hasNewConflict = false;

                    var checkTypePart = innerIndex >= commonNamespacePartsCount ||
                                        innerIndex != typePartsToSkip;
                    var checkTypeRegistry = innerIndex >= commonNamespacePartsCount ||
                                            innerIndex != typePartsToSkip - 1;
                    
                    if (checkTypePart &&
                        localNamespaceParts[innerIndex] == typePartsToReturn[0])
                    {
                        hasNewConflict = true;
                    }
                    else if (checkTypeRegistry && typeRegistry.ContainsAny(
                                 localNamespaceParts.AsSpan(0, innerIndex + 1),
                                 typePartsToReturn[0]))
                    {
                        hasNewConflict = true;

                    }

                    if (hasNewConflict)
                    {
                        // If we've found a conflict, we always need to try from the common point
                        typePartsToSkip = new[]
                        {
                            innerIndex < commonNamespacePartsCount ? innerIndex + 1: int.MaxValue,
                            commonNamespacePartsCount,
                            typePartsToSkip - 1
                        }.Min();

                        partIndex = innerIndex - 1;
                        typePartsToReturn = fullyQualifiedTypeParts.AsSpan(typePartsToSkip);
                        hasConflict = true;
                        break; // And by implication continue the outer loop
                    }
                }
            }

            // If there is a namespace conflict then regardless of the usings situation we will
            // need to qualify the type.
            if (hasConflict)
            {
                return PrefixWithGlobalMaybe(string.Join(".", typePartsToReturn.ToArray()));
            }

            // Check if we have a using containing the type:
            var count = 0;
            var typeNamespace = string.Join('.', originalTypeNamespaceParts);
            foreach (var usingPath in usingPaths)
            {
                if (usingPath != typeNamespace &&
                    !typeRegistry.ContainsType(usingPath.Split('.'), typePartsToReturn[0]))
                {
                    continue;
                }

                if (++count <= 1)
                {
                    continue;
                }

                break;
            }

            if (count == 1)
            {
                return PrefixWithGlobalMaybe(string.Join('.', originalTypeParts));
            }

            // Either multiple or no usings with the type, meaning we can't use the usings to help
            // at all, so we work out the shortest required qualifier when considering our namespace
            {
                var skipCount = 0;
                for (; skipCount < fullyQualifiedTypeParts.Length && skipCount < localNamespaceParts.Length; skipCount++)
                {
                    if (localNamespaceParts[skipCount] == fullyQualifiedTypeParts[skipCount])
                    {
                        continue;
                    }

                    break;
                }

                return PrefixWithGlobalMaybe(string.Join('.', fullyQualifiedTypeParts.Skip(skipCount)));
            }

            string PrefixWithGlobalMaybe(string normalizedTypeName)
            {
                // If the current namespace's non-first part is the first part of the type we're resolving
                // then we have to prefix with global::
                if (normalizedTypeName == fullyQualifiedType &&
                    fullyQualifiedType.Contains('.') &&
                    localNamespaceParts.Skip(1).Any(x => x == fullyQualifiedTypeParts[0]))
                {
                    return $"global::{fullyQualifiedType}";
                }

                return normalizedTypeName;
            }
        }

        /// <summary>
        /// Determines usings which are present in provided content.
        /// </summary>
        protected virtual IEnumerable<string> GetUsingsFromContent(string existingContent)
        {
            if (string.IsNullOrWhiteSpace(existingContent))
            {
                return [];
            }

            var lines = existingContent
                .Replace("\r\n", "\n")
                .Split('\n');

            var relevantContent = new List<string>();
            foreach (var line in lines)
            {
                if (line.TrimStart().StartsWith("using ") && !line.Contains('='))
                {
                    relevantContent.Add(line
                        .Replace(";", string.Empty)
                        .Replace("using ", string.Empty)
                        .Trim());
                }

                if (line.Contains("class ") ||
                    line.Contains("interface ") ||
                    line.Contains("record ") ||
                    line.Contains("struct "))
                {
                    break;
                }
            }

            return relevantContent;
        }

        /// <summary>
        /// Use the implementation of <see cref="ISupportsMigrations"/> instead.
        /// </summary>
        [Obsolete("See XML doc comments")]
#pragma warning disable CS0618 // Type or member is obsolete
        public virtual RoslynMergeConfig ConfigureRoslynMerger() => _roslynMergeConfig ??= new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        private RoslynMergeConfig? _roslynMergeConfig;
#pragma warning restore CS0618 // Type or member is obsolete

        /// <inheritdoc />
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return DefineFileConfig();
        }

        /// <summary>
        /// Returns a string representation of the provided <paramref name="resolvedTypeInfo"/>,
        /// adds any required usings, applicable template dependencies and makes the best effort to
        /// avoid conflicts between the type name and known other types and namespaces.
        /// </summary>
        public override string UseType(IResolvedTypeInfo resolvedTypeInfo)
        {
            if (resolvedTypeInfo is not CSharpResolvedTypeInfo cSharpResolvedTypeInfo)
            {
                return base.UseType(resolvedTypeInfo);
            }

            // Adds template usings etc. but we ignore the returned string since we will do different logic
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
        public virtual string DependencyUsings =>
            string.Join(Environment.NewLine, ResolveAllUsings().Select(@using => $"using {@using};"));

        /// <summary>
        /// Resolves all dependency usings for this template.
        /// </summary>
        protected IEnumerable<string> ResolveAllUsings()
        {
            var usingDirectives = this.GetAll<IDeclareUsings, string>(item => item.DeclareUsings())
                .Where(@namespace =>
                    !string.IsNullOrWhiteSpace(@namespace) &&
                    @namespace != Namespace &&
                    !Namespace.StartsWith($"{@namespace}."))
                .Distinct();

            return usingDirectives;
        }

        internal ICollection<NuGetInstall> NugetInstalls { get; } = new List<NuGetInstall>();

        /// <inheritdoc />
        public virtual IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return NugetInstalls.Select(i => i.Package);
        }

        /// <summary>
        /// Registers that the specified NuGet package should be installed in the csproj file where this file resides.
        /// </summary>
        public NugetPackageInfo AddNugetDependency(string packageName, string packageVersion)
        {
            var package = new NugetPackageInfo(packageName, packageVersion);
            NugetInstalls.Add(new NuGetInstall(package));
            return package;
        }

        /// <summary>
        /// Registers that the specified NuGet package should be installed in the .csproj file where this file resides.
        /// </summary>
        public void AddNugetDependency(INugetPackageInfo nugetPackageInfo)
        {
            NugetInstalls.Add(new NuGetInstall( nugetPackageInfo));
        }

        /// <summary>
        /// Registers that the specified NuGet package should be installed in the .csproj file where this file resides.
        /// </summary>
        public void AddNugetDependency(INugetPackageInfo nugetPackageInfo, NuGetInstallOptions options)
        {
            NugetInstalls.Add(new NuGetInstall(nugetPackageInfo, options));
        }

        /// <summary>
        /// Removes a NuGet dependency the specified NuGet package should.
        /// </summary>
        public void RemoveNugetDependency(string packageName)
        {
            var toRemoves = NugetInstalls.Where(n => n.Package.Name == packageName).ToList();
            foreach (var toRemove in toRemoves)
            {
                NugetInstalls.Remove(toRemove);
            }
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

            OutputTarget.GetProject().AddDependency(project.GetProject(), $"Template with id {Id}");
        }

        private readonly ICollection<string> _frameworkDependency = new HashSet<string>();

        /// <summary>
        /// Registers that the specified <FrameworkReference/> element should be added to the .csproj file under which this file resides.
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

        #region ISupportsMigrations

        /// <inheritdoc />
#pragma warning disable CS0618 // Type or member is obsolete
        public virtual TemplateMetadata TemplateMetadata => ConfigureRoslynMerger().TemplateMetadata;

        /// <inheritdoc />
        public virtual ITemplateMigration[] Migrations => ConfigureRoslynMerger().Migrations;
#pragma warning restore CS0618 // Type or member is obsolete

        #endregion

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
        public string GetFullyQualifiedTypeName(IElement element, string? collectionFormat = null)
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
        public string GetFullyQualifiedTypeName(IHasTypeReference hasTypeReference, string? collectionFormat = null)
        {
            var resolvedTypeInfo = GetTypeInfo(hasTypeReference.TypeReference, collectionFormat);

            return GetFullyQualifiedTypeName(resolvedTypeInfo);
        }

        private string GetFullyQualifiedTypeName(IResolvedTypeInfo resolvedTypeInfo)
        {
            base.UseType(resolvedTypeInfo);

            return resolvedTypeInfo is CSharpResolvedTypeInfo cSharpResolvedTypeInfo
                ? cSharpResolvedTypeInfo.GetFullyQualifiedTypeName()
                : resolvedTypeInfo.ToString()!;
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
        public string GetFullyQualifiedTypeName(ITemplate template, TemplateDiscoveryOptions? options = null)
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
        public string GetFullyQualifiedTypeName(ITemplateDependency templateDependency, TemplateDiscoveryOptions? options = null)
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
        public string GetFullyQualifiedTypeName(ITypeReference typeReference, string? collectionFormat = null)
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
        public string GetFullyQualifiedTypeName(string templateId, IMetadataModel model, TemplateDiscoveryOptions? options = null)
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
        public string GetFullyQualifiedTypeName(string templateId, string modelId, TemplateDiscoveryOptions? options = null)
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
        public string GetFullyQualifiedTypeName(string templateId, TemplateDiscoveryOptions? options = null)
        {
            var resolvedTypeInfo = GetTypeInfo(templateId, options);

            return GetFullyQualifiedTypeName(resolvedTypeInfo);
        }

        #endregion

        #region Nullable Reference Type Nullability Checking

        /// <summary>
        /// Determine if this template's <i>Template Output</i> is placed in a C# project with
        /// <i>Nullable</i> enabled and that the provided <paramref name="typeReference"/> is for a
        /// <b>non-</b><see href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/nullable-reference-types">nullable
        /// reference type</see>.
        /// </summary>
        /// <remarks>
        /// The check for this template's <i>Template Output</i> being placed in a C# project with
        /// <i>Nullable</i> enabled can be skipped by setting <paramref name="forceNullableEnabled"/>
        /// to <see langword="true"/>.
        /// </remarks>
        /// <param name="typeReference">
        /// The <see cref="ITypeReference"/> to check.
        /// </param>
        /// <param name="forceNullableEnabled">
        /// Force C# nullable reference type checking to be regarded as enabled regardless of whether this template's
        /// <i>Template Output</i> is in a project with <i>Nullable</i> enabled.
        /// </param>
        // ReSharper disable once UnusedMember.Global
        public bool IsNonNullableReferenceType(
            ITypeReference typeReference,
            bool forceNullableEnabled = false)
            => IsReferenceTypeWithNullability(typeReference, false, forceNullableEnabled);

        /// <summary>
        /// Determine if this template's <i>Template Output</i> is placed in a C# project with
        /// <i>Nullable</i> enabled and that the provided <paramref name="typeReference"/> is for a
        /// <see href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/nullable-reference-types">nullable
        /// reference type</see>.
        /// </summary>
        /// <remarks>
        /// The check for this template's <i>Template Output</i> being placed in a C# project with
        /// <i>Nullable</i> enabled can be skipped by setting <paramref name="forceNullableEnabled"/>
        /// to <see langword="true"/>.
        /// </remarks>
        /// <param name="typeReference">
        /// The <see cref="ITypeReference"/> to check.
        /// </param>
        /// <param name="forceNullableEnabled">
        /// Force C# nullable reference type checking to be regarded as enabled regardless of whether this template's
        /// <i>Template Output</i> is in a project with <i>Nullable</i> enabled.
        /// </param>
        public bool IsNullableReferenceType(
            ITypeReference typeReference,
            bool forceNullableEnabled = false)
            => IsReferenceTypeWithNullability(typeReference, true, forceNullableEnabled);

        /// <summary>
        /// Determine if this template's <i>Template Output</i> is placed in a C# project with
        /// <i>Nullable</i> enabled, that the provided <paramref name="typeReference"/> is a
        /// reference type and based on <paramref name="isNullable"/> whether or not it is
        /// <see href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/nullable-reference-types">nullable</see>.
        /// </summary>
        /// <remarks>
        /// The check for this template's <i>Template Output</i> being placed in a C# project with
        /// <i>Nullable</i> enabled can be skipped by setting <paramref name="forceNullableEnabled"/>
        /// to <see langword="true"/>.
        /// </remarks>
        /// <param name="typeReference">
        /// The <see cref="ITypeReference"/> to check.
        /// </param>
        /// <param name="isNullable">
        /// The nullability value to test against.
        /// </param>
        /// <param name="forceNullableEnabled">
        /// Force C# nullable reference type checking to be regarded as enabled regardless of whether this template's
        /// <i>Template Output</i> is in a project with <i>Nullable</i> enabled.
        /// </param>
        public bool IsReferenceTypeWithNullability(
            ITypeReference typeReference,
            bool isNullable,
            bool forceNullableEnabled = false)
        {
            var typeInfo = GetTypeInfo(typeReference);

            if (typeInfo.IsPrimitive ||
                typeInfo.TypeReference?.Element.SpecializationType.EndsWith("Enum", StringComparison.OrdinalIgnoreCase) == true)
            {
                return false;
            }

            return isNullable == typeInfo.IsNullable &&
                   (forceNullableEnabled || OutputTarget.GetProject().IsNullableAwareContext());
        }

        #endregion
    }
}