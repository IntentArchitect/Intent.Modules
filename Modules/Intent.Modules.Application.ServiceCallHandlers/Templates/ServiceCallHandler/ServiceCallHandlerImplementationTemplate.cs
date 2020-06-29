﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceCallHandler
{
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Application.Contracts;
    using Intent.Modelers.Services.Api;
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceCallHandlers\Templates\ServiceCallHandler\ServiceCallHandlerImplementationTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class ServiceCallHandlerImplementationTemplate : Intent.Modules.Common.Templates.IntentRoslynProjectItemTemplateBase<OperationModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(" \r\n");
            this.Write(" \r\n");
            
            #line 14 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceCallHandlers\Templates\ServiceCallHandler\ServiceCallHandlerImplementationTemplate.tt"


            
            #line default
            #line hidden
            this.Write("using System;\r\nusing System.Linq;\r\nusing System.Collections.Generic;\r\nusing Syste" +
                    "m.Threading.Tasks;\r\nusing Intent.RoslynWeaver.Attributes;\r\n");
            
            #line 21 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceCallHandlers\Templates\ServiceCallHandler\ServiceCallHandlerImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DependencyUsings));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n[assembly: DefaultIntentManaged(Mode.Ignore)]\r\n\r\nnamespace ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceCallHandlers\Templates\ServiceCallHandler\ServiceCallHandlerImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    [IntentManaged(Mode.Merge)]\r\n    public class ");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceCallHandlers\Templates\ServiceCallHandler\ServiceCallHandlerImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" \r\n    {\r\n\t\t[IntentInitialGen]\r\n        public ");
            
            #line 31 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceCallHandlers\Templates\ServiceCallHandler\ServiceCallHandlerImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("()\r\n        {\r\n        }\r\n\r\n        [IntentManaged(Mode.Merge, Body = Mode.Ignore" +
                    ", Signature = Mode.Fully)]\r\n        public ");
            
            #line 36 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceCallHandlers\Templates\ServiceCallHandler\ServiceCallHandlerImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationReturnType(Model)));
            
            #line default
            #line hidden
            this.Write(" Handle(");
            
            #line 36 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceCallHandlers\Templates\ServiceCallHandler\ServiceCallHandlerImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationDefinitionParameters(Model)));
            
            #line default
            #line hidden
            this.Write(")\r\n        {\r\n            throw new NotImplementedException(\"Implement your busin" +
                    "ess logic for this service call in the ");
            
            #line 38 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.ServiceCallHandlers\Templates\ServiceCallHandler\ServiceCallHandlerImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" (SCH = Service Call Handler) class.\");\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
