//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:8.0.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModuleBuilders.Templates.Kotlin.KotlinSingleFile {
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.Kotlin.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    
    public partial class KotlinSingleFileTemplate : KotlinTemplateBase<object> {
        
        public override string TransformText() {
            this.GenerationEnvironment = null;
            
            #line 10 ""
            this.Write("package ");
            
            #line default
            #line hidden
            
            #line 10 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( Package ));
            
            #line default
            #line hidden
            
            #line 10 ""
            this.Write("\r\n\r\nclass ");
            
            #line default
            #line hidden
            
            #line 12 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( ClassName ));
            
            #line default
            #line hidden
            
            #line 12 ""
            this.Write("() {\r\n\r\n}");
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
        
        public override void Initialize() {
            base.Initialize();
        }
    }
}
