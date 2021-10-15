// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions
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
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class ModuleSettingsExtensionsTemplate : CSharpTemplateBase<IList<Intent.ModuleBuilder.Api.ModuleSettingsConfigurationModel>>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using Intent.Configuration;\r\nusing Intent.Engine;\r\n\r\n[assembly: DefaultIntentMana" +
                    "ged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 16 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public static class ");
            
            #line 18 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {");
            
            #line 19 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
  
    foreach(var settingsGroup in Model) { 
            
            #line default
            #line hidden
            this.Write("\r\n        public static ");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(settingsGroup.Name.ToCSharpIdentifier().ToPascalCase()));
            
            #line default
            #line hidden
            this.Write(" Get");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(settingsGroup.Name.ToCSharpIdentifier().ToPascalCase()));
            
            #line default
            #line hidden
            this.Write("(this IApplicationSettingsProvider settings)\r\n        {\r\n            return new ");
            
            #line 24 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(settingsGroup.Name.ToCSharpIdentifier().ToPascalCase()));
            
            #line default
            #line hidden
            this.Write("(settings.GetGroup(\"");
            
            #line 24 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(settingsGroup.Id));
            
            #line default
            #line hidden
            this.Write("\"));\r\n        }\r\n");
            
            #line 26 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("    }\r\n");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
  foreach(var settingsGroup in Model) { 
            
            #line default
            #line hidden
            this.Write("\r\n    public class ");
            
            #line 30 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(settingsGroup.Name.ToCSharpIdentifier().ToPascalCase()));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        private readonly IGroupSettings _groupSettings;\r\n\r\n        publi" +
                    "c ");
            
            #line 34 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(settingsGroup.Name.ToCSharpIdentifier().ToPascalCase()));
            
            #line default
            #line hidden
            this.Write("(IGroupSettings groupSettings)\r\n        {\r\n            _groupSettings = groupSett" +
                    "ings;\r\n        }");
            
            #line 37 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
      
        foreach(var settingsField in settingsGroup.Fields) { 
            
            #line default
            #line hidden
            
            #line 39 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
          switch (settingsField.GetFieldConfiguration().ControlType().AsEnum())
            {
                case ModuleSettingsFieldConfigurationModelStereotypeExtensions.FieldConfiguration.ControlTypeOptionsEnum.TextBox:
                case ModuleSettingsFieldConfigurationModelStereotypeExtensions.FieldConfiguration.ControlTypeOptionsEnum.TextArea:
                case ModuleSettingsFieldConfigurationModelStereotypeExtensions.FieldConfiguration.ControlTypeOptionsEnum.Select:
                case ModuleSettingsFieldConfigurationModelStereotypeExtensions.FieldConfiguration.ControlTypeOptionsEnum.Number:
            
            #line default
            #line hidden
            this.Write("\r\n        public string ");
            
            #line 46 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(settingsField.Name.ToCSharpIdentifier().ToPascalCase()));
            
            #line default
            #line hidden
            this.Write(" => _groupSettings.GetSetting(\"");
            
            #line 46 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(settingsField.Id));
            
            #line default
            #line hidden
            this.Write("\")?.Value;\r\n");
            
            #line 47 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
                    break;
                case ModuleSettingsFieldConfigurationModelStereotypeExtensions.FieldConfiguration.ControlTypeOptionsEnum.Checkbox:
                case ModuleSettingsFieldConfigurationModelStereotypeExtensions.FieldConfiguration.ControlTypeOptionsEnum.Switch:
            
            #line default
            #line hidden
            this.Write("\r\n        public bool ");
            
            #line 51 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(settingsField.Name.ToCSharpIdentifier().ToPascalCase()));
            
            #line default
            #line hidden
            this.Write(" => bool.TryParse(_groupSettings.GetSetting(\"");
            
            #line 51 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(settingsField.Id));
            
            #line default
            #line hidden
            this.Write("\")?.Value.ToPascalCase(), out var result) && result;\r\n");
            
            #line 52 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
                    break;
                case ModuleSettingsFieldConfigurationModelStereotypeExtensions.FieldConfiguration.ControlTypeOptionsEnum.MultiSelect:
            
            #line default
            #line hidden
            
            #line 54 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            } 
            
            #line default
            #line hidden
            
            #line 58 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
      } 
            
            #line default
            #line hidden
            this.Write("    }\r\n");
            
            #line 60 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\Settings\ModuleSettingsExtensions\ModuleSettingsExtensionsTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}