﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions
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
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataPackageExtensions\ApiMetadataPackageExtensionsTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class ApiMetadataPackageExtensionsTemplate : CSharpTemplateBase<IList<Intent.ModuleBuilder.Api.PackageSettingsModel>>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System.Collections.Generic;\r\nusing System.Linq;\r\nusing Intent.Metadata.Models;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 17 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataPackageExtensions\ApiMetadataPackageExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public static class ");
            
            #line 19 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataPackageExtensions\ApiMetadataPackageExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n");
            
            #line 21 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataPackageExtensions\ApiMetadataPackageExtensionsTemplate.tt"
  foreach(var packageSettings in Model) { 
            
            #line default
            #line hidden
            this.Write("        public static IList<");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataPackageExtensions\ApiMetadataPackageExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(packageSettings)));
            
            #line default
            #line hidden
            this.Write("> Get");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataPackageExtensions\ApiMetadataPackageExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(packageSettings).ToPluralName()));
            
            #line default
            #line hidden
            this.Write("(this IDesigner designer)\r\n        {\r\n            return designer.GetPackagesOfType(");
            
            #line 24 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataPackageExtensions\ApiMetadataPackageExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(packageSettings)));
            
            #line default
            #line hidden
            this.Write(".SpecializationTypeId)\r\n                .Select(x => new ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataPackageExtensions\ApiMetadataPackageExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(packageSettings)));
            
            #line default
            #line hidden
            this.Write("(x))\r\n                .ToList();\r\n        }\r\n\r\n        public static bool Is");
            
            #line 29 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataPackageExtensions\ApiMetadataPackageExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(packageSettings)));
            
            #line default
            #line hidden
            this.Write("(this IPackage package)\r\n        {\r\n            return package?.SpecializationTypeId == ");
            
            #line 31 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataPackageExtensions\ApiMetadataPackageExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetClassName(packageSettings)));
            
            #line default
            #line hidden
            this.Write(".SpecializationTypeId;\r\n        }\r\n\r\n");
            
            #line 34 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiMetadataPackageExtensions\ApiMetadataPackageExtensionsTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
}
