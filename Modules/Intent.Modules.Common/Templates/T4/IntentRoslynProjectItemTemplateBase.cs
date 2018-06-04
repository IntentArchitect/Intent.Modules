using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Templates
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
                if (FileMetaData.CustomMetaData.ContainsKey("Namespace"))
                {
                    return FileMetaData.CustomMetaData["Namespace"];
                }
                return this.Project.Name;
            }
        }

        public string ClassName
        {
            get
            {
                if (FileMetaData.CustomMetaData.ContainsKey("ClassName"))
                {
                    return FileMetaData.CustomMetaData["ClassName"];
                }
                return FileMetaData.FileName;
            }
        }

        /// <summary>
        /// Converts the namespae of a fully qualified class name to the relative namespace fo this class instance
        /// </summary>
        /// <param name="qualifiedClassName">Namespace to convert</param>
        /// <returns></returns>
        public virtual string NormalizeNamespace(string qualifiedClassName)
        {
            if (qualifiedClassName.Contains("<") && qualifiedClassName.Contains(">"))
            {
                var genericTypes = qualifiedClassName.Substring(qualifiedClassName.IndexOf("<", StringComparison.Ordinal) + 1, qualifiedClassName.Length - qualifiedClassName.IndexOf("<", StringComparison.Ordinal) - 2);
                var normalizedGenericTypes = genericTypes
                    .Split(',')
                    .Select(NormalizeNamespace)
                    .Aggregate((x, y) => x + ", " + y);
                qualifiedClassName = $"{qualifiedClassName.Substring(0, qualifiedClassName.IndexOf("<", StringComparison.Ordinal))}<{normalizedGenericTypes}>";
            }
            var foreignParts = qualifiedClassName.Split('.').ToList();

            if (foreignParts.Count == 1)
            {
                return qualifiedClassName;
            }

            var localParts = this.Namespace.Split('.');

            foreach (var localPart in localParts)
            {
                if (localPart == foreignParts[0])
                {
                    foreignParts.RemoveAt(0);
                }
                else
                {
                    break;
                }
                if (foreignParts.Count == 1)
                {
                    return foreignParts[0];
                }
            }
            return foreignParts.Aggregate((x, y) => x + "." + y);
        }

        public abstract RoslynMergeConfig ConfigureRoslynMerger();

        public sealed override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return DefineRoslynDefaultFileMetaData();
        }

        protected abstract RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData();

        public override string DependencyUsings => this.ResolveAllUsings(Project, namespacesToIgnore: Namespace);

        public virtual IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
#warning This OK ?
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
