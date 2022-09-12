//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:6.0.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Intent.Modules.ModuleBuilder.Kotlin.Templates.KotlinFileTemplatePartial {
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    
    public partial class KotlinFileTemplatePartialTemplate : CSharpTemplateBase<Intent.ModuleBuilder.Kotlin.Api.KotlinFileTemplateModel> {
        
        public override string TransformText() {
            this.GenerationEnvironment = null;
            
            #line 10 ""
            this.Write(@"using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Kotlin;
using Intent.Modules.Common.Kotlin.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
");
            
            #line default
            #line hidden
            
            #line 18 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( Model.GetModelType() != null ? string.Format("using {0};", Model.GetModelType().ParentModule.ApiNamespace) : "" ));
            
            #line default
            #line hidden
            
            #line 18 ""
            this.Write("\r\n\r\n[assembly: DefaultIntentManaged(Mode.Merge)]\r\n\r\nnamespace ");
            
            #line default
            #line hidden
            
            #line 22 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( Namespace ));
            
            #line default
            #line hidden
            
            #line 22 ""
            this.Write("\r\n{\r\n\t[IntentManaged(Mode.Merge, Signature = Mode.Fully)]\r\n    partial class ");
            
            #line default
            #line hidden
            
            #line 25 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( ClassName ));
            
            #line default
            #line hidden
            
            #line 25 ""
            this.Write(" : ");
            
            #line default
            #line hidden
            
            #line 25 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( GetBaseType() ));
            
            #line default
            #line hidden
            
            #line 25 ""
            this.Write("\r\n    {\r\n        [IntentManaged(Mode.Fully)]\r\n        public const string Templat" +
                    "eId = \"");
            
            #line default
            #line hidden
            
            #line 28 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( GetTemplateId() ));
            
            #line default
            #line hidden
            
            #line 28 ""
            this.Write("\";\r\n\r\n        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]\r\n        public" +
                    " ");
            
            #line default
            #line hidden
            
            #line 31 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( ClassName ));
            
            #line default
            #line hidden
            
            #line 31 ""
            this.Write("(IOutputTarget outputTarget, ");
            
            #line default
            #line hidden
            
            #line 31 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( GetModelType() ));
            
            #line default
            #line hidden
            
            #line 31 ""
            this.Write(" model");
            
            #line default
            #line hidden
            
            #line 31 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( Model.GetModelType() == null ? " = null" : ""));
            
            #line default
            #line hidden
            
            #line 31 ""
            this.Write(@") : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new KotlinFileConfig(
                className: $""");
            
            #line default
            #line hidden
            
            #line 39 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( Model.IsFilePerModelTemplateRegistration() ? "{Model.Name}" : Model.Name.Replace("Template", "") ));
            
            #line default
            #line hidden
            
            #line 39 ""
            this.Write("\",\r\n                package: this.GetPackageName(),\r\n                relativeLoca" +
                    "tion: this.GetFolderPath()\r\n            );\r\n        }\r\n\r\n    }\r\n}");
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
        
        public override void Initialize() {
            base.Initialize();
        }
    }
}
