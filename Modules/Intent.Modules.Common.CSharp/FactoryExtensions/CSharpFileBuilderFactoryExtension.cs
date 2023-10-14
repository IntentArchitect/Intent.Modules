using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Templates;

namespace Intent.Modules.Common.Plugins
{
    [Description("Intent.Common.TemplateLifeCycleHooks")]
    public class CSharpFileBuilderFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Common.CSharp.CSharpFileBuilderFactoryExtension";

        public override int Order => 100000000; // always execute last

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.OutputTargets.SelectMany(x => x.TemplateInstances)
                .Where(x => x.CanRunTemplate())
                .OfType<ICSharpFileBuilderTemplate>()
                .ToList();

            templates.ForEach(x =>
            {
                try
                {
                    x.CSharpFile.StartBuild();
                }
                catch (ElementException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    if (x.TryGetModel<IElementWrapper>(out var model))
                    {
                        throw new ElementException(model.InternalElement, $@"An unexpected error occurred while building C# file [{x.Namespace}.{x.ClassName}] from template [{x.Id}]. See inner exception for more details:", e);
                    }
                    throw new Exception($@"An unexpected error occurred while building C# file [{x.Namespace}.{x.ClassName}] from template [{x.Id}]. See inner exception for more details:", e);
                }
            });
            templates.ForEach(x =>
            {
                try
                {
                    x.CSharpFile.CompleteBuild();
                }
                catch (ElementException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    if (x.TryGetModel<IElementWrapper>(out var model))
                    {
                        throw new ElementException(model.InternalElement, $@"An unexpected error occurred while building C# file [{x.Namespace}.{x.ClassName}] from template [{x.Id}]. See inner exception for more details:", e);
                    }
                    throw new Exception($@"An unexpected error occurred while building C# file [{x.Namespace}.{x.ClassName}] from template [{x.Id}]. See inner exception for more details:", e);
                }
            });
        }

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var templates = application.OutputTargets.SelectMany(x => x.TemplateInstances)
                .Where(x => x.CanRunTemplate())
                .OfType<ICSharpFileBuilderTemplate>()
                .ToList();

            templates.ForEach(x => x.CSharpFile.AfterBuild());
        }
    }
}