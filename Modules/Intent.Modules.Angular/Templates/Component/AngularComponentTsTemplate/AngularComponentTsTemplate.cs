// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Angular.Templates.Component.AngularComponentTsTemplate
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
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Component\AngularComponentTsTemplate\AngularComponentTsTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class AngularComponentTsTemplate : AngularTypescriptProjectItemTemplateBase<IComponentModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("import { Component, OnInit } from \'@angular/core\';\r\n\r\n@Component({\r\n  selector: \'" +
                    "app-");
            
            #line 11 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Component\AngularComponentTsTemplate\AngularComponentTsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ComponentName.ToKebabCase()));
            
            #line default
            #line hidden
            this.Write("\',\r\n  templateUrl: \'./");
            
            #line 12 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Component\AngularComponentTsTemplate\AngularComponentTsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ComponentName.ToKebabCase()));
            
            #line default
            #line hidden
            this.Write(".component.html\',\r\n  styleUrls: [\'./");
            
            #line 13 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Component\AngularComponentTsTemplate\AngularComponentTsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ComponentName.ToKebabCase()));
            
            #line default
            #line hidden
            this.Write(".component.css\']\r\n})\r\nexport class ");
            
            #line 15 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Angular\Templates\Component\AngularComponentTsTemplate\AngularComponentTsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" implements OnInit {\r\n\r\n  constructor() { }\r\n\r\n  ngOnInit() {\r\n  }\r\n\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
