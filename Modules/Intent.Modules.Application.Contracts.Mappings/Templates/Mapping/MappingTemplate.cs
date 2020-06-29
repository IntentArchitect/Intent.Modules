﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Application.Contracts.Mappings.Templates.Mapping
{
    using System.Collections.Generic;
    using Intent.Modelers.Services.Api;
    using Intent.Modules.Common.Templates;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class MappingTemplate : Intent.Modules.Common.Templates.IntentRoslynProjectItemTemplateBase<DTOModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(" \r\n");
            
            #line 7 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DependencyUsings));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 8 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DecoratorUsings));
            
            #line default
            #line hidden
            this.Write("\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Runtime.Ser" +
                    "ialization;\r\nusing AutoMapper;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n" +
                    "\r\nnamespace ");
            
            #line 16 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public static class ");
            
            #line 18 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" \r\n    {\r\n        public static ");
            
            #line 20 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(NormalizeNamespace(ContractTypeName)));
            
            #line default
            #line hidden
            this.Write(" MapTo");
            
            #line 20 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(NormalizeNamespace(ContractTypeName)));
            
            #line default
            #line hidden
            this.Write(" (this ");
            
            #line 20 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(NormalizeNamespace(SourceTypeName)));
            
            #line default
            #line hidden
            this.Write(" projectFrom)\r\n        {\r\n            return Mapper.Map<");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(NormalizeNamespace(ContractTypeName)));
            
            #line default
            #line hidden
            this.Write(">(projectFrom);\r\n        }\r\n\r\n        public static List<");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(NormalizeNamespace(ContractTypeName)));
            
            #line default
            #line hidden
            this.Write("> MapTo");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(NormalizeNamespace(ContractTypeName)));
            
            #line default
            #line hidden
            this.Write("s (this IEnumerable<");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(NormalizeNamespace(SourceTypeName)));
            
            #line default
            #line hidden
            this.Write("> projectFrom)\r\n        {\r\n            return projectFrom.Select(x => x.MapTo");
            
            #line 27 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(NormalizeNamespace(ContractTypeName)));
            
            #line default
            #line hidden
            this.Write("()).ToList();\r\n        }");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.Application.Contracts.Mappings\Templates\Mapping\MappingTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDecoratorMembers(ContractTypeName, SourceTypeName)));
            
            #line default
            #line hidden
            this.Write("\r\n     }\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
