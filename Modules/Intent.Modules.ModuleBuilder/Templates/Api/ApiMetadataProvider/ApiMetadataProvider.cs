// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataProvider
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using Intent.Modules.ModuleBuilder.Api;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProvider\ApiMetadataProvider.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class ApiMetadataProvider : IntentRoslynProjectItemTemplateBase<IList<ElementSettingsModel>>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System.Collections.Generic;\r\nusing System.Linq;\r\nusing Intent.Engine;\r\nusin" +
                    "g Intent.Metadata.Models;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnam" +
                    "espace ");
            
            #line 17 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProvider\ApiMetadataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 19 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProvider\ApiMetadataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        private readonly IMetadataManager _metadataManager;\r\n\r\n        p" +
                    "ublic ");
            
            #line 23 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProvider\ApiMetadataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(IMetadataManager metadataManager)\r\n        {\r\n            _metadataManager = met" +
                    "adataManager;\r\n        }\r\n\r\n");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProvider\ApiMetadataProvider.tt"
  foreach(var elementSettings in Model) { 
            
            #line default
            #line hidden
            this.Write("        public IList<");
            
            #line 29 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProvider\ApiMetadataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(elementSettings)));
            
            #line default
            #line hidden
            this.Write("> Get");
            
            #line 29 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProvider\ApiMetadataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(elementSettings).ToPluralName()));
            
            #line default
            #line hidden
            this.Write("(IApplication application)\r\n        {\r\n            var models = _metadataManager." +
                    "GetMetadata<IElement>(\"Module Builder\", application.Id)\r\n                .Where(" +
                    "x => x.SpecializationType == ");
            
            #line 32 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProvider\ApiMetadataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(elementSettings)));
            
            #line default
            #line hidden
            this.Write(".SpecializationType)\r\n                .Select(x => new ");
            
            #line 33 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProvider\ApiMetadataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(elementSettings)));
            
            #line default
            #line hidden
            this.Write("(x))\r\n                .ToList<");
            
            #line 34 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProvider\ApiMetadataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(elementSettings)));
            
            #line default
            #line hidden
            this.Write(">();\r\n            return models;\r\n        }\r\n\r\n");
            
            #line 38 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProvider\ApiMetadataProvider.tt"
  } 
            
            #line default
            #line hidden
            this.Write("    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
