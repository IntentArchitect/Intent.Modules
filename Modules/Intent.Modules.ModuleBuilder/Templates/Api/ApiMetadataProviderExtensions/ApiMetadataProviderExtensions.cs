// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions
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
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProviderExtensions\ApiMetadataProviderExtensions.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class ApiMetadataProviderExtensions : IntentRoslynProjectItemTemplateBase<IList<ElementSettingsModel>>
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
            
            #line 17 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProviderExtensions\ApiMetadataProviderExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public static class ");
            
            #line 19 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProviderExtensions\ApiMetadataProviderExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n");
            
            #line 21 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProviderExtensions\ApiMetadataProviderExtensions.tt"
  foreach(var elementSettings in Model) { 
            
            #line default
            #line hidden
            this.Write("        public static IList<");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProviderExtensions\ApiMetadataProviderExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(elementSettings)));
            
            #line default
            #line hidden
            this.Write("> Get");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProviderExtensions\ApiMetadataProviderExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(elementSettings).ToPluralName()));
            
            #line default
            #line hidden
            this.Write("(this IMetadataManager metadataManager, IApplication application)\r\n        {\r\n   " +
                    "         return new ");
            
            #line 24 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProviderExtensions\ApiMetadataProviderExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MetadataProviderClassName));
            
            #line default
            #line hidden
            this.Write("(metadataManager).Get");
            
            #line 24 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProviderExtensions\ApiMetadataProviderExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(elementSettings).ToPluralName()));
            
            #line default
            #line hidden
            this.Write("(application);\r\n        }\r\n\r\n");
            
            #line 27 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataProviderExtensions\ApiMetadataProviderExtensions.tt"
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
