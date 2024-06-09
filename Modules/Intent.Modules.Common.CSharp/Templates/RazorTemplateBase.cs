using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Common.CSharp.Templates
{
    /// <inheritdoc />
    public abstract class RazorTemplateBase : RazorTemplateBase<object>
    {
        /// <summary>
        /// Creates a new instance of <see cref="RazorTemplateBase"/>.
        /// </summary>
        protected RazorTemplateBase(string templateId, IOutputTarget outputTarget, object model) : base(templateId, outputTarget, model)
        {
        }
    }

    /// <inheritdoc cref="CSharpTemplateBase{TModel}"/>
    public abstract class RazorTemplateBase<TModel> : CSharpTemplateBase<TModel>
    {
        /// <summary>
        /// Creates a new instance of <see cref="RazorTemplateBase{TModel}"/>.
        /// </summary>
        protected RazorTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }

        /// <inheritdoc />
        protected sealed override CSharpFileConfig DefineFileConfig() => DefineRazorConfig();

        /// <summary>
        /// Factory method for creating a <see cref="RazorFileConfig"/> for a template.
        /// </summary>
        protected abstract RazorFileConfig DefineRazorConfig();

        /// <inheritdoc />
        protected override IEnumerable<string> GetUsingsFromContent(string existingContent)
        {
            if (string.IsNullOrWhiteSpace(existingContent))
            {
                yield break;
            }

            var lines = existingContent.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var line in lines)
            {
                if (!line.StartsWith("@using", StringComparison.InvariantCulture) ||
                    line.Contains('(', StringComparison.InvariantCulture))
                {
                    continue;
                }

                var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 2)
                {
                    continue;
                }

                var @using = split[1].TrimEnd(';');

                yield return @using;
            }
        }

        /// <inheritdoc />
        public override string DependencyUsings =>
            string.Join(Environment.NewLine, ResolveAllUsings().Select(@using => $"@using {@using}"));

        /// <inheritdoc />
        public override string RunTemplate()
        {
            if (this is IRazorFileTemplate razorFileTemplate)
            {
                // TODO: These "Except(...)" items probably originated due to what was in global usings
                // We should be able to use the same system as we're using in the RoslynWeaver
                foreach (var @using in ResolveAllUsings().Except(["System", "System.Collections.Generic"]))
                {
                    razorFileTemplate.RazorFile.AddUsing(@using);
                }
            }

            return TransformText();
        }
    }
}
