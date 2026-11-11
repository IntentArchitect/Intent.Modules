using System;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using System.Linq;
using Intent.Modules.Common.CSharp.Api;

namespace Intent.Modules.Common.CSharp.Templates
{
    /// <summary>
    /// C# template extensions.
    /// </summary>
    public static class ModelHasFolderTemplateExtensionsV2
    {
        /// <summary>
        /// Creates a fully qualified namespace based on the OutputTarget location and the parent
        /// folders of this template's model as described in the designer. If the model has a "C#" stereotype
        /// with a non-blank "Namespace" property, that value is used instead of the OutputTarget location and
        /// parent folders (and <paramref name="additionalFolders"/> are ignored), consistent with how references
        /// to such an element resolve in the type resolver.
        /// </summary>
        public static string GetNamespace<TModel>(this IIntentTemplate<TModel> template, params string[] additionalFolders)
        {
            if (template.Model is IHasStereotypes hasStereotypes &&
                hasStereotypes.GetStereotypeProperty<string>("C#", "Namespace") is { } stereotypeNamespace &&
                !string.IsNullOrWhiteSpace(stereotypeNamespace))
            {
                return stereotypeNamespace.Trim();
            }

            var parentFolders = template.Model is IHasFolder hasFolder
                ? hasFolder.GetParentFolders()
                : [];

            return string.Join(".", new[] { template.OutputTarget.GetNamespace() }
                .Concat(parentFolders
                    .Where(x => !string.IsNullOrWhiteSpace(x.Name) && (!x.HasFolderOptions() || x.GetFolderOptions().NamespaceProvider()))
                    .Select(x => x.Name))
                .Concat(additionalFolders));
        }

        /// <summary>
        /// Creates a fully qualified namespace based on the OutputTarget location and the <paramref name="additionalFolders"/>.
        /// </summary>
        public static string GetNamespace(IIntentTemplate template, params string[] additionalFolders)
        {
            return string.Join(".", new[] { template.OutputTarget.GetNamespace() }.Concat(additionalFolders));
        }
    }

    /// <summary>
    /// Obsolete. Use <see cref="ModelHasFolderTemplateExtensionsV2"/> instead.
    /// </summary>
    [Obsolete]
    public static class ModelHasFolderTemplateExtensions
    {
        /// <summary>
        /// Obsolete, use <see cref="Common.ModelHasFolderTemplateExtensionsV2.GetFolderPath{TModel}(IIntentTemplate{TModel},string[])"/>
        /// instead.
        /// </summary>
        [Obsolete]
        public static string GetFolderPath<TModel>(IntentTemplateBase<TModel> template, params string[] additionalFolders)
            where TModel : IHasFolder
        {
            return string.Join("/", template.Model.GetParentFolderNames().Concat(additionalFolders));
        }

        /// <summary>
        /// Obsolete, use <see cref="Common.ModelHasFolderTemplateExtensionsV2.GetFolderPath{TModel}(IIntentTemplate{TModel},string[])"/>
        /// instead.
        /// </summary>
        [Obsolete]
        public static string GetFolderPath(IntentTemplateBase<object> template, params string[] additionalFolders)
        {
            return string.Join("/", additionalFolders);
        }

        /// <summary>
        /// Obsolete, use <see cref="Common.ModelHasFolderTemplateExtensionsV2.GetFolderPath{TModel}(IIntentTemplate{TModel},string[])"/>
        /// instead.
        /// </summary>
        [Obsolete]
        public static string GetFolderPath(IntentTemplateBase template, params string[] additionalFolders)
        {
            return string.Join("/", additionalFolders);
        }

        /// <summary>
        /// Obsolete, use <see cref="Common.ModelHasFolderTemplateExtensionsV2.GetFolderPath{TModel}(IIntentTemplate{TModel},string[])"/>
        /// instead.
        /// </summary>
        [Obsolete]
        public static string GetNamespace<TModel>(CSharpTemplateBase<TModel> template, params string[] additionalFolders)
            where TModel : IHasFolder
        {
            return string.Join(".", new[] { template.OutputTarget.GetNamespace() }
                .Concat(template.Model.GetParentFolders()
                    .Where(x => !string.IsNullOrWhiteSpace(x.Name) && (!x.HasFolderOptions() || x.GetFolderOptions().NamespaceProvider()))
                    .Select(x => x.Name))
                .Concat(additionalFolders));
        }

        /// <summary>
        /// Obsolete, use <see cref="Common.ModelHasFolderTemplateExtensionsV2.GetFolderPath{TModel}(IIntentTemplate{TModel},string[])"/>
        /// instead.
        /// </summary>
        [Obsolete]
        public static string GetNamespace(CSharpTemplateBase<object> template, params string[] additionalFolders)
        {
            return string.Join(".", new[] { template.OutputTarget.GetNamespace() }.Concat(additionalFolders));
        }

        /// <summary>
        /// Obsolete, use <see cref="Common.ModelHasFolderTemplateExtensionsV2.GetFolderPath{TModel}(IIntentTemplate{TModel},string[])"/>
        /// instead.
        /// </summary>
        [Obsolete]
        public static string GetNamespace(IntentTemplateBase template, params string[] additionalFolders)
        {
            return string.Join(".", new[] { template.OutputTarget.GetNamespace() }.Concat(additionalFolders));
        }
    }
}