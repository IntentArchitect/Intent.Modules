// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Angular.Templates.Module.AngularRoutingModuleTemplate
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Angular.Api;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Module\AngularRoutingModuleTemplate\AngularRoutingModuleTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class AngularRoutingModuleTemplate : AngularTypescriptProjectItemTemplateBase<IModuleModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("import { NgModule } from \'@angular/core\';\r\nimport { RouterModule, Routes } from \'" +
                    "@angular/router\';\r\n\r\nconst routes: Routes = [\r\n");
            
            #line 12 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Module\AngularRoutingModuleTemplate\AngularRoutingModuleTemplate.tt"
 foreach(var component in Model.Components) { 
            
            #line default
            #line hidden
            this.Write("  {\r\n    path: \'");
            
            #line 14 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Module\AngularRoutingModuleTemplate\AngularRoutingModuleTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetPath(component)));
            
            #line default
            #line hidden
            this.Write("\',\r\n    component: ");
            
            #line 15 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Module\AngularRoutingModuleTemplate\AngularRoutingModuleTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(component)));
            
            #line default
            #line hidden
            this.Write("\r\n  },\r\n");
            
            #line 17 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Module\AngularRoutingModuleTemplate\AngularRoutingModuleTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("];\r\n\r\n@NgModule({\r\n  imports: [RouterModule.forChild(routes)],\r\n  exports: [Route" +
                    "rModule]\r\n})\r\nexport class ");
            
            #line 24 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Module\AngularRoutingModuleTemplate\AngularRoutingModuleTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" { }\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
