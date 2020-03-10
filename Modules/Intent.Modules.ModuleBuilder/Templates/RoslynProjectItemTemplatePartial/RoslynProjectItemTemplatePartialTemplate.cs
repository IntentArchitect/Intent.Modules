// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplatePartial
{
    using Intent.Modules.Common.Templates;
    using Intent.Metadata.Models;
    using Intent.Modules.ModuleBuilder.Api;
    using Intent.Modules.ModuleBuilder.Helpers;
    using System.Linq;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class RoslynProjectItemTemplatePartialTemplate : IntentRoslynProjectItemTemplateBase<IFileTemplate>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System.Collections.Generic;\r\nusing Intent.Engine;\r\nusing Intent.Modules.Com" +
                    "mon.Templates;\r\nusing Intent.RoslynWeaver.Attributes;\r\nusing Intent.Templates;\r\n" +
                    "");
            
            #line 13 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture((Model.GetModelType() != null ? string.Format("using {0};", Model.GetModelType().Namespace) : "")));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 14 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DependencyUsings));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n[assembly: DefaultIntentManaged(Mode.Merge)]\r\n\r\nnamespace ");
            
            #line 18 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n\t[IntentManaged(Mode.Merge)]\r\n    partial class ");
            
            #line 21 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : IntentRoslynProjectItemTemplateBase<");
            
            #line 21 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetModelType()));
            
            #line default
            #line hidden
            this.Write(">");
            
            #line 21 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetConfiguredInterfaces()));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        [IntentManaged(Mode.Fully)]\r\n        public const string Templat" +
                    "eId = \"");
            
            #line 24 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTemplateId()));
            
            #line default
            #line hidden
            this.Write("\";\r\n\r\n");
            
            #line 26 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
  if (HasDecorators()) { 
            
            #line default
            #line hidden
            this.Write("        private ICollection<");
            
            #line 27 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetExposedDecoratorContractType()));
            
            #line default
            #line hidden
            this.Write("> _decorators = new List<");
            
            #line 27 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetExposedDecoratorContractType()));
            
            #line default
            #line hidden
            this.Write(">();\r\n\r\n");
            
            #line 29 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("        [IntentInitialGen]\r\n        public ");
            
            #line 31 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(IProject project, ");
            
            #line 31 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetModelType()));
            
            #line default
            #line hidden
            this.Write(" model) : base(TemplateId, project, model)\r\n        {\r\n");
            
            #line 33 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
  if (Model.GetCreationMode() != CreationMode.SingleFile) { 
            
            #line default
            #line hidden
            this.Write("            AddTypeSource(CSharpTypeSource.InProject(Project, ");
            
            #line 34 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(".TemplateId, collectionFormat: \"IEnumerable<{0}>\"));\r\n");
            
            #line 35 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write(@"        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, ""1.0""));
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: """);
            
            #line 48 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetCreationMode() == CreationMode.FilePerModel ? "${Model.Name}" : Model.Name.Replace("Template", "")));
            
            #line default
            #line hidden
            this.Write("\",\r\n                fileExtension: \"cs\",\r\n                defaultLocationInProjec" +
                    "t: \"");
            
            #line 50 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetCreationMode() == CreationMode.FilePerModel ? Model.Name.Replace("Template", "") : ""));
            
            #line default
            #line hidden
            this.Write("\",\r\n                className: \"");
            
            #line 51 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetCreationMode() == CreationMode.FilePerModel ? "${Model.Name}" : Model.Name.Replace("Template", "")));
            
            #line default
            #line hidden
            this.Write("\",\r\n                @namespace: \"${Project.Name}");
            
            #line 52 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetCreationMode() == CreationMode.FilePerModel ? "." + Model.Name.Replace("Template", "") : ""));
            
            #line default
            #line hidden
            this.Write("\"\r\n            );\r\n        }\r\n\r\n");
            
            #line 56 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
  if (HasDecorators()) { 
            
            #line default
            #line hidden
            this.Write("        [IntentManaged(Mode.Fully)]\r\n        public void AddDecorator(");
            
            #line 58 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetExposedDecoratorContractType()));
            
            #line default
            #line hidden
            this.Write(" decorator)\r\n        {\r\n            _decorators.Add(decorator);\r\n        }\r\n\r\n   " +
                    "     [IntentManaged(Mode.Fully)]\r\n        public IEnumerable<");
            
            #line 64 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.GetExposedDecoratorContractType()));
            
            #line default
            #line hidden
            this.Write("> GetDecorators()\r\n        {\r\n            return _decorators;\r\n        }\r\n");
            
            #line 68 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
  } 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 70 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
 /* if (HasTemplateDependencies()) { 
            
            #line default
            #line hidden
            this.Write(@"        [IntentManaged(Mode.Fully, Body = Mode.Fully, Signature = Mode.Fully)]
        IEnumerable<ITemplateDependency> IHasTemplateDependencies.GetTemplateDependencies()
        {
            var templateDependencies = new List<ITemplateDependency>();
");
            
            #line 75 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
      foreach (var templateDependency in GetTemplateDependencyInfos().Where(p => !p.IsCustom)) { 
            
            #line default
            #line hidden
            this.Write("                templateDependencies.Add(TemplateDependency.OnTemplate(");
            
            #line 76 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(templateDependency.TemplateId));
            
            #line default
            #line hidden
            this.Write("));\r\n");
            
            #line 77 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
      } 
            
            #line default
            #line hidden
            
            #line 78 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
      if (GetTemplateDependencyInfos().Any(p => p.IsCustom)) { 
            
            #line default
            #line hidden
            this.Write("                templateDependencies.AddRange(GetCustomTemplateDependencies());\r\n" +
                    "");
            
            #line 80 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
      } 
            
            #line default
            #line hidden
            this.Write("            return templateDependencies;\r\n        }\r\n\r\n");
            
            #line 84 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
      foreach (var templateDependency in GetTemplateDependencyInfos().Where(p => !p.IsCustom)) { 
            
            #line default
            #line hidden
            this.Write("        [IntentManaged(Mode.Fully, Body = Mode.Fully, Signature = Mode.Fully)]\r\n " +
                    "       private ");
            
            #line 86 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(templateDependency.InstanceType));
            
            #line default
            #line hidden
            this.Write(" Get");
            
            #line 86 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(templateDependency.TemplateName.ToPascalCase()));
            
            #line default
            #line hidden
            this.Write("Template(");
            
            #line 86 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(templateDependency.TemplateModel));
            
            #line default
            #line hidden
            this.Write(" model)\r\n        {\r\n            return Project.FindTemplateInstance<");
            
            #line 88 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(templateDependency.InstanceType));
            
            #line default
            #line hidden
            this.Write(">(");
            
            #line 88 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(templateDependency.TemplateId));
            
            #line default
            #line hidden
            this.Write(", model);\r\n        }\r\n");
            
            #line 90 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
      } 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 92 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
      if (GetTemplateDependencyInfos().Any(p => p.IsCustom)) { 
            
            #line default
            #line hidden
            this.Write(@"        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        private IEnumerable<ITemplateDependency> GetCustomTemplateDependencies()
        {
            return new ITemplateDependency[] 
            {
            };
        }
");
            
            #line 100 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
      } 
            
            #line default
            #line hidden
            
            #line 101 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\RoslynProjectItemTemplatePartial\RoslynProjectItemTemplatePartialTemplate.tt"
  } */
            
            #line default
            #line hidden
            this.Write("    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
