// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileNoModel
{
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.ModuleBuilder.Api;
    using Intent.Metadata.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Registration\SingleFileNoModel\SingleFileNoModelTemplateRegistrationTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class SingleFileNoModelTemplateRegistrationTemplate : CSharpTemplateBase<TemplateRegistrationModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(@"
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Templates;
");
            
            #line 17 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Registration\SingleFileNoModel\SingleFileNoModelTemplateRegistrationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DependencyUsings));
            
            #line default
            #line hidden
            this.Write("\r\n[assembly: DefaultIntentManaged(Mode.Merge)]\r\n\r\nnamespace ");
            
            #line 20 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Registration\SingleFileNoModel\SingleFileNoModelTemplateRegistrationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n\t[IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]\r\n   " +
                    " public class ");
            
            #line 23 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Registration\SingleFileNoModel\SingleFileNoModelTemplateRegistrationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : SingleFileTemplateRegistration\r\n    {\r\n        public override string Template" +
                    "Id =>  ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Registration\SingleFileNoModel\SingleFileNoModelTemplateRegistrationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTemplateNameForTemplateId()));
            
            #line default
            #line hidden
            this.Write(".TemplateId;\r\n\r\n        public override ITemplate CreateTemplateInstance(IOutputT" +
                    "arget outputTarget)\r\n        {\r\n\t\t\treturn new ");
            
            #line 29 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Registration\SingleFileNoModel\SingleFileNoModelTemplateRegistrationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTemplateNameForTemplateId()));
            
            #line default
            #line hidden
            this.Write("(outputTarget, null);\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
