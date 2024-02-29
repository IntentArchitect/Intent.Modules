﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Kotlin.Templates.KotlinFileTemplatePartial
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Kotlin\Templates\KotlinFileTemplatePartial\KotlinFileTemplatePartialTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class KotlinFileTemplatePartialTemplate : CSharpTemplateBase<Intent.ModuleBuilder.Kotlin.Api.KotlinFileTemplateModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System.Collections.Generic;\r\nusing Intent.Engine;\r\nusing Intent.Modules.Common;\r\nusing Intent.Modules.Common.Templates;\r\nusing Intent.Modules.Common.Kotlin;\r\nusing Intent.Modules.Common.Kotlin.Templates;\r\nusing Intent.RoslynWeaver.Attributes;\r\nusing Intent.Templates;\r\n");
            
            #line 18 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Kotlin\Templates\KotlinFileTemplatePartial\KotlinFileTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetModelType() != null ? string.Format("using {0};", Model.GetModelType().ParentModule.ApiNamespace) : ""));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Kotlin\Templates\KotlinFileTemplatePartial\KotlinFileTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n\t[IntentManaged(Mode.Merge, Signature = Mode.Fully)]\r\n    partial class ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Kotlin\Templates\KotlinFileTemplatePartial\KotlinFileTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Kotlin\Templates\KotlinFileTemplatePartial\KotlinFileTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetBaseType()));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        [IntentManaged(Mode.Fully)]\r\n        public const string TemplateId = \"");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Kotlin\Templates\KotlinFileTemplatePartial\KotlinFileTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTemplateId()));
            
            #line default
            #line hidden
            this.Write("\";\r\n\r\n        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]\r\n        public ");
            
            #line 31 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Kotlin\Templates\KotlinFileTemplatePartial\KotlinFileTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(IOutputTarget outputTarget, ");
            
            #line 31 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Kotlin\Templates\KotlinFileTemplatePartial\KotlinFileTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetModelType()));
            
            #line default
            #line hidden
            this.Write(" model");
            
            #line 31 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Kotlin\Templates\KotlinFileTemplatePartial\KotlinFileTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetModelType() == null ? " = null" : ""));
            
            #line default
            #line hidden
            this.Write(") : base(TemplateId, outputTarget, model)\r\n        {\r\n        }\r\n\r\n        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]\r\n        public override ITemplateFileConfig GetTemplateFileConfig()\r\n        {\r\n            return new KotlinFileConfig(\r\n                className: $\"");
            
            #line 39 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Kotlin\Templates\KotlinFileTemplatePartial\KotlinFileTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.IsFilePerModelTemplateRegistration() ? "{Model.Name}" : Model.Name.Replace("Template", "")));
            
            #line default
            #line hidden
            this.Write("\",\r\n                package: this.GetPackageName(),\r\n                relativeLocation: this.GetFolderPath()\r\n            );\r\n        }\r\n\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
}
