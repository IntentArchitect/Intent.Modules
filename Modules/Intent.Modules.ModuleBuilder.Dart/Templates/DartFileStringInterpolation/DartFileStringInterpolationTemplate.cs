// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Dart.Templates.DartFileStringInterpolation
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
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Dart\Templates\DartFileStringInterpolation\DartFileStringInterpolationTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class DartFileStringInterpolationTemplate : CSharpTemplateBase<Intent.ModuleBuilder.Dart.Api.DartFileTemplateModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.Collections.Generic;\r\n\r\n[assembly: DefaultIntentManag" +
                    "ed(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 15 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Dart\Templates\DartFileStringInterpolation\DartFileStringInterpolationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    [IntentManaged(Mode.Fully, Body = Mode.Merge)]\r\n    public partial class" +
                    " ");
            
            #line 18 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Dart\Templates\DartFileStringInterpolation\DartFileStringInterpolationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(@"
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $@""
class {ClassName} {{{string.Join(Environment.NewLine, GetMembers())}
}}
"";
        }

        [IntentInitialGen] private IEnumerable<string> GetMembers()
        {
            var members = new List<string>();

            // example: adding a constructor
            members.Add($@""
    {ClassName}() {{
    }}"");

            return members;
        }
    }
}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}