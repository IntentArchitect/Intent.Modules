// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Angular.Templates.Proxies.AngularDTOTemplate
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Metadata.Models;
    using Intent.Modules.Angular.Api;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Proxies\AngularDTOTemplate\AngularDTOTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class AngularDTOTemplate : AngularTypescriptProjectItemTemplateBase<IModuleDTOModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\nexport interface ");
            
            #line 10 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Proxies\AngularDTOTemplate\AngularDTOTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            
            #line 10 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Proxies\AngularDTOTemplate\AngularDTOTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenericTypes));
            
            #line default
            #line hidden
            this.Write(" {\r\n");
            
            #line 11 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Proxies\AngularDTOTemplate\AngularDTOTemplate.tt"
  foreach (var field in Model.Fields) { 
            
            #line default
            #line hidden
            this.Write("    ");
            
            #line 12 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Proxies\AngularDTOTemplate\AngularDTOTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(field.Name.ToCamelCase()));
            
            #line default
            #line hidden
            
            #line 12 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Proxies\AngularDTOTemplate\AngularDTOTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(field.Type.IsNullable ? "?" : ""));
            
            #line default
            #line hidden
            this.Write(": ");
            
            #line 12 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Proxies\AngularDTOTemplate\AngularDTOTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.Types.Get(field.Type)));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 13 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Proxies\AngularDTOTemplate\AngularDTOTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}