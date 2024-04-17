// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModelExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using Intent.ModuleBuilder.Api;
    using Intent.ModuleBuilder.Helpers;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class ApiElementModelExtensionsTemplate : CSharpTemplateBase<ExtensionModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing Inten" +
                    "t.Metadata.Models;\r\nusing Intent.Modules.Common;\r\n\r\n[assembly: DefaultIntentMana" +
                    "ged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 20 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public static class ");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n");
            
            #line 24 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
  foreach(var stereotypeDefinition in Model.StereotypeDefinitions) { 
            
            #line default
            #line hidden
            this.Write("        public static ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write(" Get");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(this ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelClassName));
            
            #line default
            #line hidden
            this.Write(" model)\r\n        {\r\n            var stereotype = model.GetStereotype(");
            
            #line 27 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write(".DefinitionId);\r\n            return stereotype != null ? new ");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(stereotype) : null;\r\n        }\r\n");
            
            #line 30 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
      if (stereotypeDefinition.AllowMultipleApplies) { 
            
            #line default
            #line hidden
            this.Write("\r\n        public static IReadOnlyCollection<");
            
            #line 32 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("> Get");
            
            #line 32 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ForcePluralize(stereotypeDefinition.Name.ToCSharpIdentifier())));
            
            #line default
            #line hidden
            this.Write("(this ");
            
            #line 32 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelClassName));
            
            #line default
            #line hidden
            this.Write(" model)\r\n        {\r\n            var stereotypes = model\r\n                .GetSter" +
                    "eotypes(");
            
            #line 35 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write(".DefinitionId)\r\n                .Select(stereotype => new ");
            
            #line 36 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(stereotype))\r\n                .ToArray();\r\n\r\n            return stereotypes;\r\n  " +
                    "      }\r\n");
            
            #line 41 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
      } 
            
            #line default
            #line hidden
            this.Write(" \r\n\r\n        public static bool Has");
            
            #line 43 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(this ");
            
            #line 43 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelClassName));
            
            #line default
            #line hidden
            this.Write(" model)\r\n        {\r\n            return model.HasStereotype(");
            
            #line 45 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write(".DefinitionId);\r\n        }\r\n\r\n        public static bool TryGet");
            
            #line 48 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(this ");
            
            #line 48 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelClassName));
            
            #line default
            #line hidden
            this.Write(" model, out ");
            
            #line 48 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write(" stereotype)\r\n        {\r\n            if (!Has");
            
            #line 50 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(model))\r\n            {\r\n                stereotype = null;\r\n                retu" +
                    "rn false;\r\n            }\r\n\r\n            stereotype = new ");
            
            #line 56 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(model.GetStereotype(");
            
            #line 56 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write(".DefinitionId));\r\n            return true;\r\n        }\r\n\r\n");
            
            #line 60 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
  } 
            
            #line default
            #line hidden
            
            #line 61 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
  foreach(var stereotypeDefinition in Model.StereotypeDefinitions) { 
            
            #line default
            #line hidden
            this.Write("        public class ");
            
            #line 62 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("\r\n        {\r\n            private IStereotype _stereotype;\r\n            public con" +
                    "st string DefinitionId = \"");
            
            #line 65 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Id));
            
            #line default
            #line hidden
            this.Write("\";\r\n\r\n            public ");
            
            #line 67 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(IStereotype stereotype)\r\n            {\r\n                _stereotype = stereotype" +
                    ";\r\n            }\r\n\r\n            public string ");
            
            #line 72 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(stereotypeDefinition.Properties.Any(x => x.Name.Equals("Name", StringComparison.InvariantCultureIgnoreCase)) ? "StereotypeName" : "Name"));
            
            #line default
            #line hidden
            this.Write(" => _stereotype.Name;\r\n\r\n");
            
            #line 74 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
      foreach(var property in stereotypeDefinition.Properties) { 
        switch (property.ControlType) {
            case StereotypePropertyControlType.TextBox:
            case StereotypePropertyControlType.TextArea:
            case StereotypePropertyControlType.Function:
            
            
            #line default
            #line hidden
            this.Write("            public string ");
            
            #line 80 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n                return _stereotype.GetProperty<string>(\"");
            
            #line 81 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n            }\r\n\r\n");
            
            #line 84 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
              break;
            case StereotypePropertyControlType.Number:
            
            #line default
            #line hidden
            this.Write("            public int? ");
            
            #line 86 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n                return _stereotype.GetProperty<int?>(\"");
            
            #line 87 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n            }\r\n\r\n");
            
            #line 90 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
              break;
            case StereotypePropertyControlType.Checkbox:
            
            #line default
            #line hidden
            this.Write("            public bool ");
            
            #line 92 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n                return _stereotype.GetProperty<bool>(\"");
            
            #line 93 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n            }\r\n\r\n");
            
            #line 96 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
              break;
            case StereotypePropertyControlType.Select:
            
            #line default
            #line hidden
            
            #line 98 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
              switch (property.OptionsSource) {
                    case StereotypePropertyOptionsSource.LookupElement:
                    case StereotypePropertyOptionsSource.LookupChildren:
                    case StereotypePropertyOptionsSource.LookupStereotype:
                    case StereotypePropertyOptionsSource.NestedLookup:
            
            #line default
            #line hidden
            this.Write("            public IElement ");
            
            #line 103 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n");
            
            #line 104 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
                      if (property.LookupTypes.Count == 1 && false) { /* TODO */ 
            
            #line default
            #line hidden
            this.Write("                return new ");
            
            #line 105 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.LookupTypes.Single().ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("(_stereotype.GetProperty<IElement>(\"");
            
            #line 105 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\"));\r\n");
            
            #line 106 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
                      } else { 
            
            #line default
            #line hidden
            this.Write("                return _stereotype.GetProperty<IElement>(\"");
            
            #line 107 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n");
            
            #line 108 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
                      } 
            
            #line default
            #line hidden
            this.Write("            }\r\n\r\n");
            
            #line 111 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
                      break;
                case StereotypePropertyOptionsSource.Options:
            
            #line default
            #line hidden
            this.Write("            public ");
            
            #line 113 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("Options ");
            
            #line 113 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n                return new ");
            
            #line 114 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("Options(_stereotype.GetProperty<string>(\"");
            
            #line 114 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\"));\r\n            }\r\n\r\n");
            
            #line 117 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
                      break;
                    default:
                        throw new ArgumentOutOfRangeException(property.OptionsSource.ToString());
                } 
            
            #line default
            #line hidden
            
            #line 121 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
              break;
            case StereotypePropertyControlType.MultiSelect:
            
            #line default
            #line hidden
            
            #line 123 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
              switch (property.OptionsSource) {
                    case StereotypePropertyOptionsSource.LookupElement:
                    case StereotypePropertyOptionsSource.LookupChildren:
                    case StereotypePropertyOptionsSource.LookupStereotype:
                    case StereotypePropertyOptionsSource.NestedLookup:
            
            #line default
            #line hidden
            this.Write("            public IElement[] ");
            
            #line 128 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n                return _stereotype.GetProperty<IElement[]>(\"");
            
            #line 129 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\") ?? new IElement[0];\r\n            }\r\n\r\n");
            
            #line 132 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
                      break;
                case StereotypePropertyOptionsSource.Options:
            
            #line default
            #line hidden
            this.Write("            public ");
            
            #line 134 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("Options[] ");
            
            #line 134 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n                return _stereotype.GetProperty<string[]>(\"");
            
            #line 135 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\")?.Select(x => new ");
            
            #line 135 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("Options(x)).ToArray() ?? new ");
            
            #line 135 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("Options[0];\r\n            }\r\n\r\n");
            
            #line 138 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
                      break;
                    default:
                        throw new ArgumentOutOfRangeException(property.OptionsSource.ToString());
                } 
            
            #line default
            #line hidden
            
            #line 142 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
              break;
            case StereotypePropertyControlType.Icon:
            
            #line default
            #line hidden
            this.Write("            public IIconModel ");
            
            #line 144 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("() {\r\n                return _stereotype.GetProperty<IIconModel>(\"");
            
            #line 145 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n            }\r\n\r\n");
            
            #line 148 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
              break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        }
            
            #line default
            #line hidden
            
            #line 153 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
  foreach (var property in stereotypeDefinition.Properties.Where(x => x.ValueOptions.Any())) { 
            
            #line default
            #line hidden
            this.Write("            public class ");
            
            #line 154 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("Options\r\n            {\r\n                public readonly string Value;\r\n\r\n        " +
                    "        public ");
            
            #line 158 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("Options(string value)\r\n                {\r\n                    Value = value;\r\n   " +
                    "             }\r\n\r\n                public ");
            
            #line 163 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("OptionsEnum AsEnum()\r\n                {\r\n                    switch (Value)\r\n    " +
                    "                {\r\n");
            
            #line 167 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
      foreach(var option in property.ValueOptions) { 
            
            #line default
            #line hidden
            this.Write("                        case \"");
            
            #line 168 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(option));
            
            #line default
            #line hidden
            this.Write("\":\r\n                            return ");
            
            #line 169 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("OptionsEnum.");
            
            #line 169 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(option.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 170 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
      } 
            
            #line default
            #line hidden
            this.Write("                        default:\r\n                            throw new ArgumentO" +
                    "utOfRangeException();\r\n                    }\r\n                }\r\n\r\n");
            
            #line 176 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
      foreach(var option in property.ValueOptions) { 
            
            #line default
            #line hidden
            this.Write("                public bool ");
            
            #line 177 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(("Is" + option.ToPascalCase()).ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("()\r\n                {\r\n                    return Value == \"");
            
            #line 179 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(option));
            
            #line default
            #line hidden
            this.Write("\";\r\n                }\r\n");
            
            #line 181 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
      } 
            
            #line default
            #line hidden
            this.Write("            }\r\n\r\n            public enum ");
            
            #line 184 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToCSharpIdentifier()));
            
            #line default
            #line hidden
            this.Write("OptionsEnum\r\n            {\r\n                ");
            
            #line 186 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(string.Join(@",
                ", property.ValueOptions.Select(option => option.ToCSharpIdentifier()))));
            
            #line default
            #line hidden
            this.Write("\r\n            }\r\n");
            
            #line 189 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("        }\r\n\r\n");
            
            #line 192 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Api\ApiElementModelExtensions\ApiElementModelExtensionsTemplate.tt"
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
