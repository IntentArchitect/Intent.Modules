using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using ModuleBuilders.Templates.CSharp.CSharpCustomBuilder;
using ModuleBuilders.Templates.CSharp.CSharpCustomCustom;
using ModuleBuilders.Templates.CSharp.CSharpCustomStringInter;
using ModuleBuilders.Templates.CSharp.CSharpCustomT4;
using ModuleBuilders.Templates.CSharp.CSharpFilePerModelBuilder;
using ModuleBuilders.Templates.CSharp.CSharpFilePerModelCustom;
using ModuleBuilders.Templates.CSharp.CSharpFilePerModelStringInter;
using ModuleBuilders.Templates.CSharp.CSharpFilePerModelT4;
using ModuleBuilders.Templates.CSharp.CSharpSingleFileBuilder;
using ModuleBuilders.Templates.CSharp.CSharpSingleFileCustom;
using ModuleBuilders.Templates.CSharp.CSharpSingleFileStringInter;
using ModuleBuilders.Templates.CSharp.CSharpSingleFileT4;
using ModuleBuilders.Templates.Dart.DartCustomCustom;
using ModuleBuilders.Templates.Dart.DartCustomStringInter;
using ModuleBuilders.Templates.Dart.DartCustomT4;
using ModuleBuilders.Templates.Dart.DartFilePerModelCustom;
using ModuleBuilders.Templates.Dart.DartFilePerModelStringInter;
using ModuleBuilders.Templates.Dart.DartFilePerModelT4;
using ModuleBuilders.Templates.Dart.DartSingleFileCustom;
using ModuleBuilders.Templates.Dart.DartSingleFileStringInter;
using ModuleBuilders.Templates.Dart.DartSingleFileT4;
using ModuleBuilders.Templates.Java.JavaCustomBuilder;
using ModuleBuilders.Templates.Java.JavaCustomCustom;
using ModuleBuilders.Templates.Java.JavaCustomStringInter;
using ModuleBuilders.Templates.Java.JavaCustomT4;
using ModuleBuilders.Templates.Java.JavaFilePerModelBuilder;
using ModuleBuilders.Templates.Java.JavaFilePerModelCustom;
using ModuleBuilders.Templates.Java.JavaFilePerModelStringInter;
using ModuleBuilders.Templates.Java.JavaFilePerModelT4;
using ModuleBuilders.Templates.Java.JavaSingleFileBuilder;
using ModuleBuilders.Templates.Java.JavaSingleFileCustom;
using ModuleBuilders.Templates.Java.JavaSingleFileStringInter;
using ModuleBuilders.Templates.Java.JavaSingleFileT4;
using ModuleBuilders.Templates.Kotlin.KotlinCustom;
using ModuleBuilders.Templates.Kotlin.KotlinFilePerModel;
using ModuleBuilders.Templates.Kotlin.KotlinSingleFile;
using ModuleBuilders.Templates.TypeScript.TypeScriptCustomBuilder;
using ModuleBuilders.Templates.TypeScript.TypeScriptCustomCustom;
using ModuleBuilders.Templates.TypeScript.TypeScriptCustomStringInter;
using ModuleBuilders.Templates.TypeScript.TypeScriptCustomT4;
using ModuleBuilders.Templates.TypeScript.TypeScriptFilePerModelBuilder;
using ModuleBuilders.Templates.TypeScript.TypeScriptFilePerModelCustom;
using ModuleBuilders.Templates.TypeScript.TypeScriptFilePerModelStringInter;
using ModuleBuilders.Templates.TypeScript.TypeScriptFilePerModelT4;
using ModuleBuilders.Templates.TypeScript.TypeScriptSingleFileBuilder;
using ModuleBuilders.Templates.TypeScript.TypeScriptSingleFileCustom;
using ModuleBuilders.Templates.TypeScript.TypeScriptSingleFileStringInter;
using ModuleBuilders.Templates.TypeScript.TypeScriptSingleFileT4;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace ModuleBuilders.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCSharpCustomBuilderTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(CSharpCustomBuilderTemplate.TemplateId);
        }

        public static string GetCSharpCustomCustomTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(CSharpCustomCustomTemplate.TemplateId);
        }

        public static string GetCSharpCustomStringInterTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(CSharpCustomStringInterTemplate.TemplateId);
        }

        public static string GetCSharpCustomT4TemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(CSharpCustomT4Template.TemplateId);
        }

        public static string GetCSharpFilePerModelBuilderTemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(CSharpFilePerModelBuilderTemplate.TemplateId, template.Model);
        }

        public static string GetCSharpFilePerModelBuilderTemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(CSharpFilePerModelBuilderTemplate.TemplateId, model);
        }

        public static string GetCSharpFilePerModelCustomTemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(CSharpFilePerModelCustomTemplate.TemplateId, template.Model);
        }

        public static string GetCSharpFilePerModelCustomTemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(CSharpFilePerModelCustomTemplate.TemplateId, model);
        }

        public static string GetCSharpFilePerModelStringInterTemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(CSharpFilePerModelStringInterTemplate.TemplateId, template.Model);
        }

        public static string GetCSharpFilePerModelStringInterTemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(CSharpFilePerModelStringInterTemplate.TemplateId, model);
        }

        public static string GetCSharpFilePerModelT4TemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(CSharpFilePerModelT4Template.TemplateId, template.Model);
        }

        public static string GetCSharpFilePerModelT4TemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(CSharpFilePerModelT4Template.TemplateId, model);
        }

        public static string GetCSharpSingleFileBuilderTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(CSharpSingleFileBuilderTemplate.TemplateId);
        }

        public static string GetCSharpSingleFileCustomTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(CSharpSingleFileCustomTemplate.TemplateId);
        }

        public static string GetCSharpSingleFileStringInterTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(CSharpSingleFileStringInterTemplate.TemplateId);
        }

        public static string GetCSharpSingleFileT4TemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(CSharpSingleFileT4Template.TemplateId);
        }

        public static string GetDartCustomCustomTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(DartCustomCustomTemplate.TemplateId);
        }

        public static string GetDartCustomStringInterTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(DartCustomStringInterTemplate.TemplateId);
        }

        public static string GetDartCustomT4TemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(DartCustomT4Template.TemplateId);
        }

        public static string GetDartFilePerModelCustomTemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(DartFilePerModelCustomTemplate.TemplateId, template.Model);
        }

        public static string GetDartFilePerModelCustomTemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(DartFilePerModelCustomTemplate.TemplateId, model);
        }

        public static string GetDartFilePerModelStringInterTemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(DartFilePerModelStringInterTemplate.TemplateId, template.Model);
        }

        public static string GetDartFilePerModelStringInterTemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(DartFilePerModelStringInterTemplate.TemplateId, model);
        }

        public static string GetDartFilePerModelT4TemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(DartFilePerModelT4Template.TemplateId, template.Model);
        }

        public static string GetDartFilePerModelT4TemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(DartFilePerModelT4Template.TemplateId, model);
        }

        public static string GetDartSingleFileCustomTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(DartSingleFileCustomTemplate.TemplateId);
        }

        public static string GetDartSingleFileStringInterTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(DartSingleFileStringInterTemplate.TemplateId);
        }

        public static string GetDartSingleFileT4TemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(DartSingleFileT4Template.TemplateId);
        }

        public static string GetJavaCustomBuilderTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(JavaCustomBuilderTemplate.TemplateId);
        }

        public static string GetJavaCustomCustomTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(JavaCustomCustomTemplate.TemplateId);
        }

        public static string GetJavaCustomStringInterTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(JavaCustomStringInterTemplate.TemplateId);
        }

        public static string GetJavaCustomT4TemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(JavaCustomT4Template.TemplateId);
        }

        public static string GetJavaFilePerModelBuilderTemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(JavaFilePerModelBuilderTemplate.TemplateId, template.Model);
        }

        public static string GetJavaFilePerModelBuilderTemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(JavaFilePerModelBuilderTemplate.TemplateId, model);
        }

        public static string GetJavaFilePerModelCustomTemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(JavaFilePerModelCustomTemplate.TemplateId, template.Model);
        }

        public static string GetJavaFilePerModelCustomTemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(JavaFilePerModelCustomTemplate.TemplateId, model);
        }

        public static string GetJavaFilePerModelStringInterTemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(JavaFilePerModelStringInterTemplate.TemplateId, template.Model);
        }

        public static string GetJavaFilePerModelStringInterTemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(JavaFilePerModelStringInterTemplate.TemplateId, model);
        }

        public static string GetJavaFilePerModelT4TemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(JavaFilePerModelT4Template.TemplateId, template.Model);
        }

        public static string GetJavaFilePerModelT4TemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(JavaFilePerModelT4Template.TemplateId, model);
        }

        public static string GetJavaSingleFileBuilderTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(JavaSingleFileBuilderTemplate.TemplateId);
        }

        public static string GetJavaSingleFileCustomTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(JavaSingleFileCustomTemplate.TemplateId);
        }

        public static string GetJavaSingleFileStringInterTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(JavaSingleFileStringInterTemplate.TemplateId);
        }

        public static string GetJavaSingleFileT4TemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(JavaSingleFileT4Template.TemplateId);
        }

        public static string GetKotlinCustomTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(KotlinCustomTemplate.TemplateId);
        }

        public static string GetKotlinFilePerModelTemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(KotlinFilePerModelTemplate.TemplateId, template.Model);
        }

        public static string GetKotlinFilePerModelTemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(KotlinFilePerModelTemplate.TemplateId, model);
        }

        public static string GetKotlinSingleFileTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(KotlinSingleFileTemplate.TemplateId);
        }

        public static string GetTypeScriptCustomBuilderTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(TypeScriptCustomBuilderTemplate.TemplateId);
        }

        public static string GetTypeScriptCustomCustomTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(TypeScriptCustomCustomTemplate.TemplateId);
        }

        public static string GetTypeScriptCustomStringInterTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(TypeScriptCustomStringInterTemplate.TemplateId);
        }

        public static string GetTypeScriptCustomT4TemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(TypeScriptCustomT4Template.TemplateId);
        }

        public static string GetTypeScriptFilePerModelBuilderTemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(TypeScriptFilePerModelBuilderTemplate.TemplateId, template.Model);
        }

        public static string GetTypeScriptFilePerModelBuilderTemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(TypeScriptFilePerModelBuilderTemplate.TemplateId, model);
        }

        public static string GetTypeScriptFilePerModelCustomTemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(TypeScriptFilePerModelCustomTemplate.TemplateId, template.Model);
        }

        public static string GetTypeScriptFilePerModelCustomTemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(TypeScriptFilePerModelCustomTemplate.TemplateId, model);
        }

        public static string GetTypeScriptFilePerModelStringInterTemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(TypeScriptFilePerModelStringInterTemplate.TemplateId, template.Model);
        }

        public static string GetTypeScriptFilePerModelStringInterTemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(TypeScriptFilePerModelStringInterTemplate.TemplateId, model);
        }

        public static string GetTypeScriptFilePerModelT4TemplateName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(TypeScriptFilePerModelT4Template.TemplateId, template.Model);
        }

        public static string GetTypeScriptFilePerModelT4TemplateName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(TypeScriptFilePerModelT4Template.TemplateId, model);
        }

        public static string GetTypeScriptSingleFileBuilderTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(TypeScriptSingleFileBuilderTemplate.TemplateId);
        }

        public static string GetTypeScriptSingleFileCustomTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(TypeScriptSingleFileCustomTemplate.TemplateId);
        }

        public static string GetTypeScriptSingleFileStringInterTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(TypeScriptSingleFileStringInterTemplate.TemplateId);
        }

        public static string GetTypeScriptSingleFileT4TemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(TypeScriptSingleFileT4Template.TemplateId);
        }

    }
}