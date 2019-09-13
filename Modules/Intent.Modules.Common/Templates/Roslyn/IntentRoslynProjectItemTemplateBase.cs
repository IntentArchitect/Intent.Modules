using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

[assembly:InternalsVisibleTo("Intent.Modules.Common.Tests")]

namespace Intent.Modules.Common.Templates
{
    public abstract class IntentRoslynProjectItemTemplateBase<TModel> : IntentProjectItemTemplateBase<TModel>, IHasNugetDependencies, IHasClassDetails, IRoslynMerge
    {

        public IntentRoslynProjectItemTemplateBase(string templateId, IProject project, TModel model)
            : base(templateId, project, model)
        {

        }

        public string Namespace
        {
            get
            {
                if (FileMetadata.CustomMetadata.ContainsKey("Namespace"))
                {
                    return FileMetadata.CustomMetadata["Namespace"];
                }
                return this.Project.Name;
            }
        }

        public string ClassName
        {
            get
            {
                if (FileMetadata.CustomMetadata.ContainsKey("ClassName"))
                {
                    return FileMetadata.CustomMetadata["ClassName"].AsClassName();
                }
                return FileMetadata.FileName.AsClassName();
            }
        }

        /// <summary>
        /// Converts the namespace of a fully qualified class name to the relative namespace for this class instance
        /// </summary>
        /// <param name="foreignType">The foreign type which is ideally fully qualified</param>
        /// <returns></returns>
        public virtual string NormalizeNamespace(string foreignType)
        {
            // Handle Generics recursively:
            if (foreignType.Contains("<") && foreignType.Contains(">"))
            {
                var genericTypes = foreignType.Substring(foreignType.IndexOf("<", StringComparison.Ordinal) + 1, foreignType.Length - foreignType.IndexOf("<", StringComparison.Ordinal) - 2);
                var normalizedGenericTypes = genericTypes
                    .Split(',')
                    .Select(NormalizeNamespace)
                    .Aggregate((x, y) => x + ", " + y);
                foreignType = $"{foreignType.Substring(0, foreignType.IndexOf("<", StringComparison.Ordinal))}<{normalizedGenericTypes}>";
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
                .Concat(Project.Application.Projects.Select(x => x.Name))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray();

            return NormalizeNamespace(localNamespace, foreignType, knownOtherPaths, usingPaths);
        }

        private IEnumerable<string> _templateUsings;
        private IEnumerable<string> TemplateUsings => _templateUsings ?? (_templateUsings = GetUsingsFromContent(GenerationEnvironment.ToString()));

        private IEnumerable<string> _existingContentUsings;
        private IEnumerable<string> ExistingContentUsings
        {
            get
            {
                if (_existingContentUsings != null)
                {
                    return _existingContentUsings;
                }

                var filepath = FileMetadata.GetRelativeFilePathWithFileName();
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
                if (line.StartsWith("using ") && !line.Contains("="))
                {
                    relevantContent.Add(line
                        .Replace(";", string.Empty)
                        .Replace("using ", string.Empty)
                        .Trim());
                }

                if (line.StartsWith("namespace"))
                {
                    break;
                }
            }

            return relevantContent;
        }

        public abstract RoslynMergeConfig ConfigureRoslynMerger();

        public sealed override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return DefineRoslynDefaultFileMetadata();
        }

        protected abstract RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata();

        public virtual string DependencyUsings => this.ResolveAllUsings(Project, namespacesToIgnore: Namespace);

        public virtual IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                new NugetPackageInfo(name : "Intent.RoslynWeaver.Attributes", version : "1.0.0")
            };
        }
    }

    public abstract class IntentRoslynProjectItemTemplateBase : IntentRoslynProjectItemTemplateBase<object>
    {
        public IntentRoslynProjectItemTemplateBase(string templateId, IProject project) : base(templateId, project, null)
        {

        }
    }
}
