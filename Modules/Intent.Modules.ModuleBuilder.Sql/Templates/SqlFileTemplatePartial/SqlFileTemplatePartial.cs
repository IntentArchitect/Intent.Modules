// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Sql.Templates.SqlFileTemplatePartial
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using Intent.Modules.ModuleBuilder.Sql.Api;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Sql\Templates\SqlFileTemplatePartial\SqlFileTemplatePartial.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class SqlFileTemplatePartial : IntentRoslynProjectItemTemplateBase<SqlTemplateModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System.Collections.Generic;\r\nusing Intent.Engine;\r\nusing Intent.Modules.Com" +
                    "mon.Sql.Templates;\r\nusing Intent.RoslynWeaver.Attributes;\r\nusing Intent.Template" +
                    "s;\r\n");
            
            #line 15 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Sql\Templates\SqlFileTemplatePartial\SqlFileTemplatePartial.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture((Model.GetDesignerSettings() != null ? string.Format("using {0};", Model.GetDesignerSettings().ApiNamespace) : "")));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n[assembly: DefaultIntentManaged(Mode.Merge)]\r\n\r\nnamespace ");
            
            #line 19 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Sql\Templates\SqlFileTemplatePartial\SqlFileTemplatePartial.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n\t[IntentManaged(Mode.Merge, Signature = Mode.Fully)]\r\n    partial class ");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Sql\Templates\SqlFileTemplatePartial\SqlFileTemplatePartial.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : SqlTemplateBase<");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Sql\Templates\SqlFileTemplatePartial\SqlFileTemplatePartial.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetModelType()));
            
            #line default
            #line hidden
            this.Write(">\r\n    {\r\n        [IntentManaged(Mode.Fully)]\r\n        public const string Templa" +
                    "teId = \"");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Sql\Templates\SqlFileTemplatePartial\SqlFileTemplatePartial.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTemplateId()));
            
            #line default
            #line hidden
            this.Write("\";\r\n\r\n        [IntentInitialGen]\r\n        public ");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Sql\Templates\SqlFileTemplatePartial\SqlFileTemplatePartial.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(IProject project, ");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Sql\Templates\SqlFileTemplatePartial\SqlFileTemplatePartial.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetModelType()));
            
            #line default
            #line hidden
            this.Write(@" model) : base(TemplateId, project, model)
        {
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new SqlFileConfiguration(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: """);
            
            #line 37 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Sql\Templates\SqlFileTemplatePartial\SqlFileTemplatePartial.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.IsFilePerModelTemplateRegistration() ? "${Model.Name}" : Model.Name.Replace("Template", "")));
            
            #line default
            #line hidden
            this.Write("\",\r\n                defaultLocationInProject: \"");
            
            #line 38 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder.Sql\Templates\SqlFileTemplatePartial\SqlFileTemplatePartial.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.IsFilePerModelTemplateRegistration() ? Model.Name.Replace("Template", "") : ""));
            
            #line default
            #line hidden
            this.Write("\"\r\n            );\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
