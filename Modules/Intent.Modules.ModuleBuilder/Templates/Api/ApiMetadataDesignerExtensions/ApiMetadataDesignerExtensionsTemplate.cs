// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using Intent.ModuleBuilder.Api;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataDesignerExtensions\ApiMetadataDesignerExtensionsTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class ApiMetadataDesignerExtensionsTemplate : CSharpTemplateBase<IList<Intent.ModuleBuilder.Api.DesignerModel>>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using Intent.Engine;\r\nusing Intent.Metadata.Models;\r\n\r\n[assembly: DefaultIntentMa" +
                    "naged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 16 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataDesignerExtensions\ApiMetadataDesignerExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public static class ");
            
            #line 18 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataDesignerExtensions\ApiMetadataDesignerExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n");
            
            #line 20 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataDesignerExtensions\ApiMetadataDesignerExtensionsTemplate.tt"
  foreach(var designer in Model) { 
            
            #line default
            #line hidden
            this.Write("        public static IDesigner ");
            
            #line 21 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataDesignerExtensions\ApiMetadataDesignerExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(designer.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(this IMetadataManager metadataManager, IApplication application)\r\n        {\r\n   " +
                    "         return metadataManager.");
            
            #line 23 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataDesignerExtensions\ApiMetadataDesignerExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(designer.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(application.Id);\r\n        }\r\n\r\n        public static IDesigner ");
            
            #line 26 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataDesignerExtensions\ApiMetadataDesignerExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(designer.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(this IMetadataManager metadataManager, string applicationId)\r\n        {\r\n       " +
                    "     return metadataManager.GetDesigner(applicationId, \"");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataDesignerExtensions\ApiMetadataDesignerExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(designer.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n        }\r\n\r\n");
            
            #line 31 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataDesignerExtensions\ApiMetadataDesignerExtensionsTemplate.tt"
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
