//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:7.0.11
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModuleBuilders.Templates.File.FileFilePerModelT4 {
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Metadata.Models;
    using Intent.Modelers.Domain.Api;
    using System;
    
    
    public partial class FileFilePerModelT4Template : IntentTemplateBase<Intent.Modelers.Domain.Api.ClassModel> {
        
        public override string TransformText() {
            this.GenerationEnvironment = null;
            
            #line 9 ""
            this.Write("\r\n// Place your file template logic here\r\n");
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
        
        public override void Initialize() {
            base.Initialize();
        }
    }
}