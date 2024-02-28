﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using Intent.ModuleBuilder.TypeScript.Api;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class TypescriptTemplatePartialTemplate : CSharpTemplateBase<Intent.ModuleBuilder.TypeScript.Api.TypescriptFileTemplateModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.Collections.Generic;\r\nusing Intent.Engine;\r\nusing Intent.Modules.Common;\r\nusing Intent.Modules.Common.Templates;\r\nusing Intent.Modules.Common.TypeScript.Templates;\r\nusing Intent.RoslynWeaver.Attributes;\r\nusing Intent.Templates;\r\n");
            
            #line 19 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetModelType() != null ? string.Format("using {0};", Model.GetModelType().ParentModule.ApiNamespace) : ""));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 23 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n\t[IntentManaged(Mode.Merge, Signature = Mode.Fully)]\r\n    ");
            
            #line 26 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAccessModifier()));
            
            #line default
            #line hidden
            this.Write("class ");
            
            #line 26 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 26 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(string.Join(", ", GetBaseTypes())));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        [IntentManaged(Mode.Fully)]\r\n        public const string TemplateId = \"");
            
            #line 29 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTemplateId()));
            
            #line default
            #line hidden
            this.Write("\";\r\n\r\n        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]\r\n        public ");
            
            #line 32 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(IOutputTarget outputTarget, ");
            
            #line 32 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetModelType()));
            
            #line default
            #line hidden
            this.Write(" model");
            
            #line 32 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetModelType() == null ? " = null" : ""));
            
            #line default
            #line hidden
            this.Write(") : base(TemplateId, outputTarget, model)\r\n        {\r\n");
            
            #line 34 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
  if (Model.GetTypeScriptTemplateSettings()?.TemplatingMethod().IsTypeScriptFileBuilder() == true) { 
            
            #line default
            #line hidden
            this.Write("            TypescriptFile = new TypescriptFile(this.GetFolderPath())\r\n                .AddClass($\"");
            
            #line 36 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName()));
            
            #line default
            #line hidden
            this.Write("\", @class =>\r\n                {\r\n                    @class.AddConstructor(ctor =>\r\n                    {\r\n                        ctor.AddParameter(\"string\", \"exampleParam\", param =>\r\n                        {\r\n                            param.WithPrivateReadonlyFieldAssignment();\r\n                        });\r\n                    });\r\n                });\r\n");
            
            #line 46 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("        }\r\n");
            
            #line 48 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
  if (Model.GetTypeScriptTemplateSettings()?.TemplatingMethod().IsTypeScriptFileBuilder() == true) { 
            
            #line default
            #line hidden
            this.Write("\r\n        [IntentManaged(Mode.Fully)]\r\n        public ");
            
            #line 51 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(UseType("Intent.Modules.Common.TypeScript.Builder.TypescriptFile")));
            
            #line default
            #line hidden
            this.Write(" TypescriptFile { get; }\r\n\r\n        [IntentManaged(Mode.Fully)]\r\n        public override ITemplateFileConfig GetTemplateFileConfig()\r\n        {\r\n            return TypescriptFile.GetConfig($\"");
            
            #line 56 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName()));
            
            #line default
            #line hidden
            this.Write("\");\r\n        }\r\n\r\n        [IntentManaged(Mode.Fully)]\r\n        public override string TransformText()\r\n        {\r\n            return TypescriptFile.ToString();\r\n        }\r\n");
            
            #line 64 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
  } else { 
            
            #line default
            #line hidden
            this.Write("\r\n        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]\r\n        public override ITemplateFileConfig GetTemplateFileConfig()\r\n        {\r\n            return new TypeScriptFileConfig(\r\n                className: $\"");
            
            #line 70 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.IsFilePerModelTemplateRegistration() ? "{Model.Name}" : Model.Name.Replace("Template", "")));
            
            #line default
            #line hidden
            this.Write("\",\r\n                fileName: $\"");
            
            #line 71 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.IsFilePerModelTemplateRegistration() ? "{Model.Name.ToKebabCase()}" : Model.Name.Replace("Template", "").ToKebabCase()));
            
            #line default
            #line hidden
            this.Write("\"\r\n            );\r\n        }\r\n");
            
            #line 74 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
  } 
            
            #line default
            #line hidden
            
            #line 75 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
  if (Model.GetTypeScriptTemplateSettings()?.TemplatingMethod().IsCustom() == true) { 
            
            #line default
            #line hidden
            this.Write("\r\n        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]\r\n        public override string TransformText()\r\n        {\r\n            throw new NotImplementedException(\"Implement custom template here\");\r\n        }\r\n");
            
            #line 82 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.TypeScript\Templates\TypescriptTemplatePartial\TypescriptTemplatePartialTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
}
