// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpStringInterpolation
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
    
    #line 1 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\CSharpStringInterpolation\CSharpStringInterpolationTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class CSharpStringInterpolationTemplate : CSharpTemplateBase<Intent.ModuleBuilder.CSharp.Api.CSharpTemplateModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 12 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\CSharpStringInterpolation\CSharpStringInterpolationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    [IntentManaged(Mode.Fully, Body = Mode.Merge)]\r\n    public partial class" +
                    " ");
            
            #line 15 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\CSharpStringInterpolation\CSharpStringInterpolationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(@"
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$""""""
                     using System;

                     [assembly: DefaultIntentManaged(Mode.Fully)]

                     namespace {{Namespace}}
                     {
                         public ");
            
            #line 27 "D:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.CSharp\Templates\CSharpStringInterpolation\CSharpStringInterpolationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(IsForInterface() ? "interface" : "class"));
            
            #line default
            #line hidden
            this.Write(" {{ClassName}}\r\n                         {\r\n                         }\r\n         " +
                    "            }\r\n                     \"\"\";\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
