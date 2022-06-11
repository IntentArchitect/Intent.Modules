using System;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.TypeScript.TypeResolvers;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.TypeScript.Templates
{
    /// <summary>
    /// Obsolete. Use <see cref="TypeScriptTemplateBase"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public abstract class IntentTypescriptProjectItemTemplateBase<TModel> : IntentTemplateBase<TModel>, IClassProvider
    {
        /// <summary>
        /// Creates a new instance of <see cref="IntentTypescriptProjectItemTemplateBase{TModel}"/>.
        /// </summary>
        protected IntentTypescriptProjectItemTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }

        /// <inheritdoc />
        public string Namespace
        {
            get
            {
                if (FileMetadata.CustomMetadata.ContainsKey("Namespace"))
                {
                    return FileMetadata.CustomMetadata["Namespace"];
                }
                return null;
            }
        }

        /// <inheritdoc />
        public string ClassName
        {
            get
            {
                if (FileMetadata.CustomMetadata.ContainsKey("ClassName"))
                {
                    return FileMetadata.CustomMetadata["ClassName"];
                }
                return FileMetadata.FileName;
            }
        }

        /// <summary>
        /// The type name for the type defined by this template.
        /// </summary>
        public string TypeName => string.IsNullOrWhiteSpace(Namespace) ? ClassName : $"{Namespace}.{ClassName}";

        /// <summary>
        /// Obsolete. Please notify Intent Architect employees if you still have a need for this.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public string Location => FileMetadata.LocationInProject;

        /// <summary>
        /// Obsolete. Use <see cref="ClassTypeSource.WithCollectionFormat"/> instead. For example:
        /// <code>
        /// AddTypeSource(...).WithCollectionFormat(...);
        /// </code>
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public new void AddTypeSource(string templateId, string collectionFormat)
        {
            AddTypeSource(TypescriptTypeSource.Create(ExecutionContext, templateId, collectionFormat));
        }

        /// <inheritdoc />
        protected override string UseType(IResolvedTypeInfo resolvedTypeInfo)
        {
            var normalizedTypeName = NormalizeTypeName(resolvedTypeInfo.ToString());

            return normalizedTypeName;
        }

        /// <inheritdoc />
        public override string RunTemplate()
        {
            var templateOutput = base.RunTemplate();

            return templateOutput;
        }
    }
}