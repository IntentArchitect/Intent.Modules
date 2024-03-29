// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Templates.TemplateDecorator
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\TemplateDecorator\TemplateDecoratorTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class TemplateDecoratorTemplate : CSharpTemplateBase<Intent.ModuleBuilder.Api.TemplateDecoratorModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using Intent.Engine;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace" +
                    " ");
            
            #line 14 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\TemplateDecorator\TemplateDecoratorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    [IntentManaged(Mode.Merge)]\r\n    public class ");
            
            #line 17 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\TemplateDecorator\TemplateDecoratorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 17 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\TemplateDecorator\TemplateDecoratorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTypeName(Model)));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        [IntentManaged(Mode.Fully)]\r\n        public const string Decorat" +
                    "orId = \"");
            
            #line 20 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\TemplateDecorator\TemplateDecoratorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDecoratorId()));
            
            #line default
            #line hidden
            this.Write("\";\r\n\r\n        [IntentManaged(Mode.Fully)]\r\n        private readonly ");
            
            #line 23 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\TemplateDecorator\TemplateDecoratorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTemplateTypeName()));
            
            #line default
            #line hidden
            this.Write(" _template;\r\n        [IntentManaged(Mode.Fully)]\r\n        private readonly IAppli" +
                    "cation _application;\r\n\r\n        [IntentManaged(Mode.Fully, Body = Mode.Fully)]\r\n" +
                    "        public ");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\TemplateDecorator\TemplateDecoratorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\TemplateDecorator\TemplateDecoratorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTemplateTypeName()));
            
            #line default
            #line hidden
            this.Write(" template, IApplication application)\r\n        {\r\n            _template = template" +
                    ";\r\n            _application = application;\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
