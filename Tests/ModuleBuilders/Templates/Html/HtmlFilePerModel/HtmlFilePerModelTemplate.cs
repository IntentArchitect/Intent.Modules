//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:8.0.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModuleBuilders.Templates.Html.HtmlFilePerModel {
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.Html.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using Intent.Modelers.Domain.Api;
    using System;
    
    
    public partial class HtmlFilePerModelTemplate : HtmlTemplateBase<Intent.Modelers.Domain.Api.ClassModel> {
        
        public override string TransformText() {
            this.GenerationEnvironment = null;
            
            #line 11 ""
            this.Write("\r\n<!-- Replace this with your HTML template -->");
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
        
        public override void Initialize() {
            base.Initialize();
        }
    }
}
