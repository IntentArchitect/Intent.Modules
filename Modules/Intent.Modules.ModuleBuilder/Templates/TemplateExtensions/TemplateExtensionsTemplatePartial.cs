using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.TemplateExtensions
{
    public interface IModuleBuilderTemplate : ITemplateWithModel
    {
        string Id { get; }
        string GetTemplateId();
        string GetModelType();
        string GetRole();
        string TemplateType();
    }

    partial class TemplateExtensionsTemplate : CSharpTemplateBase<object>, IDeclareUsings
    {
        //protected List<IModuleBuilderTemplate> Templates = new List<IModuleBuilderTemplate>();
        protected List<IModuleBuilderTemplate> Templates = new List<IModuleBuilderTemplate>();

        public const string TemplateId = "Intent.ModuleBuilder.Templates.TemplateExtensions";

        public TemplateExtensionsTemplate(IOutputTarget outputTarget) : base(TemplateId, outputTarget, null)
        {
            ExecutionContext.EventDispatcher.Subscribe<TemplateRegistrationRequiredEvent>(@event =>
            {
                //var template = @event.ModelId != null
                //    ? GetTemplate<IModuleBuilderTemplate>(@event.TemplateId, @event.ModelId, new TemplateDiscoveryOptions() { ThrowIfNotFound = false })
                //    : GetTemplate<IModuleBuilderTemplate>(@event.TemplateId, new TemplateDiscoveryOptions() { ThrowIfNotFound = false });
                //if (template != null)
                //{
                //    Templates.Add(template);
                //}
                if (@event.SourceTemplateId != null)
                {
                    Templates.Add(this.GetTemplate<IModuleBuilderTemplate>(@event.SourceTemplateId, @event.ModelId));
                }
            });
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"TemplateExtensions",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}