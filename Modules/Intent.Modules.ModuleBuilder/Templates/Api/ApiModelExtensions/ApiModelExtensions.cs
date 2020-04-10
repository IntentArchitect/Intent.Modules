// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiModelExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using Intent.Modules.ModuleBuilder.Api;
    using Intent.Modules.ModuleBuilder.Helpers;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class ApiModelExtensions : IntentRoslynProjectItemTemplateBase<ElementSettingsModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing Intent.Metadata.Models;\r\nusing Intent.Modules.Common;\r\n\r\n[as" +
                    "sembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 17 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public static class ");
            
            #line 19 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n");
            
            #line 21 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
  foreach(var stereotypeDefinition in StereotypeDefinitions) { 
            
            #line default
            #line hidden
            this.Write("        public static ");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write(" Get");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(this ");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelClassName));
            
            #line default
            #line hidden
            this.Write(" model)\r\n        {\r\n            var stereotype = model.GetStereotype(\"");
            
            #line 24 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n            return stereotype != null ? new ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(stereotype) : null;\r\n        }\r\n\r\n");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
  } 
            
            #line default
            #line hidden
            this.Write(" \r\n");
            
            #line 29 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
  foreach(var stereotypeDefinition in StereotypeDefinitions) { 
            
            #line default
            #line hidden
            this.Write("        public class ");
            
            #line 30 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("\r\n        {\r\n            private IStereotype _stereotype;\r\n\r\n            public ");
            
            #line 34 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(IStereotype stereotype)\r\n            {\r\n                _stereotype = stereotype" +
                    ";\r\n            }\r\n\r\n");
            
            #line 39 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
      foreach(var property in stereotypeDefinition.Properties) { 
        switch (property.ControlType) {
            case StereotypePropertyControlType.TextBox:
            case StereotypePropertyControlType.TextArea:
            
            #line default
            #line hidden
            this.Write("            public string ");
            
            #line 43 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n                return _stereotype.GetProperty<string>(\"");
            
            #line 44 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n            }\r\n\r\n");
            
            #line 47 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
              break;
            case StereotypePropertyControlType.Number:
            
            #line default
            #line hidden
            this.Write("            public int? ");
            
            #line 49 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n                return _stereotype.GetProperty<int?>(\"");
            
            #line 50 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n            }\r\n\r\n");
            
            #line 53 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
              break;
            case StereotypePropertyControlType.Checkbox:
            
            #line default
            #line hidden
            this.Write("            public bool ");
            
            #line 55 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n                return _stereotype.GetProperty<bool>(\"");
            
            #line 56 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n            }\r\n\r\n");
            
            #line 59 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
              break;
            case StereotypePropertyControlType.Select:
            
            #line default
            #line hidden
            
            #line 61 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
              switch (property.OptionsSource) {
                    case StereotypePropertyOptionsSource.LookupElement:
                    case StereotypePropertyOptionsSource.NestedLookup:
            
            #line default
            #line hidden
            this.Write("            public IElement ");
            
            #line 64 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n");
            
            #line 65 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
                      if (property.LookupTypes.Count == 1 && false) { /* TODO */ 
            
            #line default
            #line hidden
            this.Write("                return new ");
            
            #line 66 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.LookupTypes.Single().ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(_stereotype.GetProperty<IElement>(\"");
            
            #line 66 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\"));\r\n");
            
            #line 67 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
                      } else { 
            
            #line default
            #line hidden
            this.Write("                return _stereotype.GetProperty<IElement>(\"");
            
            #line 68 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n");
            
            #line 69 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
                      } 
            
            #line default
            #line hidden
            this.Write("            }\r\n\r\n");
            
            #line 72 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
                      break;
                case StereotypePropertyOptionsSource.Options:
            
            #line default
            #line hidden
            this.Write("            public ");
            
            #line 74 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("Options ");
            
            #line 74 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n                return new ");
            
            #line 75 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("Options(_stereotype.GetProperty<string>(\"");
            
            #line 75 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\"));\r\n            }\r\n\r\n");
            
            #line 78 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
                      break;
                    default:
                        throw new ArgumentOutOfRangeException(property.OptionsSource.ToString());
                } 
            
            #line default
            #line hidden
            
            #line 82 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
              break;
            case StereotypePropertyControlType.MultiSelect:
            
            #line default
            #line hidden
            this.Write("            public IElement[] ");
            
            #line 84 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n                return _stereotype.GetProperty<IElement[]>(\"");
            
            #line 85 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n            }\r\n\r\n");
            
            #line 88 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
              break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        }
            
            #line default
            #line hidden
            
            #line 93 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
  foreach (var property in stereotypeDefinition.Properties.Where(x => x.ValueOptions.Any())) { 
            
            #line default
            #line hidden
            this.Write("            public class ");
            
            #line 94 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("Options\r\n            {\r\n                public readonly string Value;\r\n\r\n        " +
                    "        public ");
            
            #line 98 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("Options(string value)\r\n                {\r\n                    Value = value;\r\n   " +
                    "             }\r\n\r\n");
            
            #line 103 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
      foreach(var option in property.ValueOptions) { 
            
            #line default
            #line hidden
            this.Write("                public bool Is");
            
            #line 104 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(option.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("()\r\n                {\r\n                    return Value == \"");
            
            #line 106 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(option));
            
            #line default
            #line hidden
            this.Write("\";\r\n                }\r\n");
            
            #line 108 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
      } 
            
            #line default
            #line hidden
            this.Write("            }\r\n\r\n");
            
            #line 111 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
  } 
            
            #line default
            #line hidden
            this.Write("        }\r\n\r\n");
            
            #line 114 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiModelExtensions\ApiModelExtensions.tt"
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
