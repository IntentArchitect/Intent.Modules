// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace ModuleTests.ModuleBuilderTests.Templates.Other.CustomCreationMode
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Metadata.Models;
    using Intent.Modelers.Domain.Api;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleTests\ModuleBuilderTests\ModuleTests.ModuleBuilderTests\Templates\Other\CustomCreationMode\CustomCreationMode.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class CustomCreationMode : IntentRoslynProjectItemTemplateBase<IClass>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\nusing System;\r\n");
            
            #line 11 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleTests\ModuleBuilderTests\ModuleTests.ModuleBuilderTests\Templates\Other\CustomCreationMode\CustomCreationMode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DependencyUsings));
            
            #line default
            #line hidden
            this.Write(@"
// Mode.Fully will overwrite file on each run. 
// Add in explicit [IntentManaged.Ignore] attributes to class or methods. Alternatively change to Mode.Merge (additive) or Mode.Ignore (once-off)
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace ");
            
            #line 16 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleTests\ModuleBuilderTests\ModuleTests.ModuleBuilderTests\Templates\Other\CustomCreationMode\CustomCreationMode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 18 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleTests\ModuleBuilderTests\ModuleTests.ModuleBuilderTests\Templates\Other\CustomCreationMode\CustomCreationMode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
