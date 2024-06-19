// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.CSharp.Templates.RazorTemplatePartial
{
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.ModuleBuilder.CSharp.Api;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class RazorTemplatePartialTemplate : CSharpTemplateBase<Intent.ModuleBuilder.CSharp.Api.RazorTemplateModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing Intent.Engine;\r\nusing Intent.Modules.Common;\r\nusing Intent.M" +
                    "odules.Common.Templates;\r\nusing Intent.Modules.Common.CSharp.Templates;\r\n");
            
            #line 10 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetModelType() != null ? $"using {Model.GetModelType().Namespace};" : ""));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 14 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    /// <summary>\r\n    /// A Razor template.\r\n    /// </summary>\r\n    [Inten" +
                    "tManaged(Mode.Fully, Body = Mode.Merge)]\r\n    ");
            
            #line 20 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAccessModifier()));
            
            #line default
            #line hidden
            this.Write("class ");
            
            #line 20 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 20 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(string.Join(", ", GetBaseTypes())));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        /// <inheritdoc cref=\"IntentTemplateBase.Id\"/>\r\n        [IntentM" +
                    "anaged(Mode.Fully)]\r\n        public const string TemplateId = \"");
            
            #line 24 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTemplateId()));
            
            #line default
            #line hidden
            this.Write("\";\r\n\r\n        /// <summary>\r\n        /// Creates a new instance of <see cref=\"");
            
            #line 27 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\"/>.\r\n        /// </summary>\r\n        [IntentManaged(Mode.Fully, Body = Mode.Igno" +
                    "re)]\r\n        public ");
            
            #line 30 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(IOutputTarget outputTarget, ");
            
            #line 30 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetModelType()));
            
            #line default
            #line hidden
            this.Write(" model");
            
            #line 30 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetModelType() == null ? " = null" : ""));
            
            #line default
            #line hidden
            this.Write(") : base(TemplateId, outputTarget, model)\r\n        {\r\n");
            
            #line 32 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
  if (Model.GetRazorTemplateSettings()?.TemplatingMethod().IsRazorFileBuilder() == true) { 
            
            #line default
            #line hidden
            this.Write("            RazorFile = IRazorFile.Create.(this, $\"");
            
            #line 33 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName()));
            
            #line default
            #line hidden
            this.Write("\")\r\n                .Configure(file =>\r\n                {\r\n                    fi" +
                    "le.AddPageDirective($\"");
            
            #line 36 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetPageDirectiveText()));
            
            #line default
            #line hidden
            this.Write("\");\r\n\r\n                    file.AddHtmlElement(\"PageTitle\", element => element.Wi" +
                    "thText($\"");
            
            #line 38 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetPageTitleText()));
            
            #line default
            #line hidden
            this.Write("\"));\r\n                });\r\n");
            
            #line 40 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("        }\r\n");
            
            #line 42 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
  if (Model.GetRazorTemplateSettings()?.TemplatingMethod().IsRazorFileBuilder() == true) { 
            
            #line default
            #line hidden
            this.Write("\r\n        /// <inheritdoc />\r\n        [IntentManaged(Mode.Fully)]\r\n        public" +
                    " ");
            
            #line 46 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(UseType("Intent.Modules.Blazor.Api.IRazorFile")));
            
            #line default
            #line hidden
            this.Write(" RazorFile { get; }\r\n");
            
            #line 47 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("\r\n        /// <inheritdoc />\r\n        [IntentManaged(Mode.Fully)]\r\n        protec" +
                    "ted override RazorFileConfig DefineRazorConfig()\r\n        {\r\n            return " +
                    "RazorFile.GetConfig();\r\n        }\r\n");
            
            #line 55 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
  if (Model.GetRazorTemplateSettings()?.TemplatingMethod().IsRazorFileBuilder() == true) { 
            
            #line default
            #line hidden
            this.Write("\r\n        /// <inheritdoc />\r\n        [IntentManaged(Mode.Fully)]\r\n        public" +
                    " override string TransformText() => RazorFile.ToString();\r\n");
            
            #line 60 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\RazorTemplatePartial\RazorTemplatePartialTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
