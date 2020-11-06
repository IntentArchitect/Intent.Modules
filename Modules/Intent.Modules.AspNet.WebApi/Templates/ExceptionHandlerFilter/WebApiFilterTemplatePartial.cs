using Intent.Engine;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNet.WebApi.Templates.ExceptionHandlerFilter
{
    partial class WebApiFilterTemplate : CSharpTemplateBase<object>, IDeclareUsings, IHasAssemblyDependencies
    {
        public const string TemplateId = "Intent.AspNet.WebApi.ExceptionHandlerFilter";

        public WebApiFilterTemplate(string templateId, IProject project) 
            : base(templateId, project, null)
        {
            AddAssemblyReference(new GacAssemblyReference("System.Net.Http"));
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ExceptionHandlerFilter",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[] 
            {
                "System.Net",
                "System.Net.Http",
                "System.Web.Http.Filters"
            };
        }
    }
}
