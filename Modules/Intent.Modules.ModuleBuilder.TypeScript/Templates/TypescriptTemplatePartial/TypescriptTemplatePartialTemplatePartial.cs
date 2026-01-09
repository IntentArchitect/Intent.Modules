using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.TypeScript.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorContract;
using Intent.Modules.ModuleBuilder.Templates.TemplateExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial
{
    [IntentManaged(Mode.Fully, Signature = Mode.Merge, Body = Mode.Merge)]
    public partial class TypescriptTemplatePartialTemplate : CSharpTemplateBase<TypescriptFileTemplateModel>, ICSharpFileBuilderTemplate, IModuleBuilderTemplate
    {
        public const string TemplateId = "Intent.ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TypescriptTemplatePartialTemplate(IOutputTarget outputTarget, TypescriptFileTemplateModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentModulesCommonTypeScript);

            CSharpFile = new CSharpFile($"{OutputTarget.GetNamespace()}.{FolderNamespace}", this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("Intent.Engine")
                .AddUsing("Intent.Modules.Common")
                .AddUsing("Intent.Modules.Common.Templates")
                .AddUsing("Intent.Modules.Common.TypeScript.Templates")
                .AddUsing("Intent.RoslynWeaver.Attributes")
                .AddUsing("Intent.Templates")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.AddAttribute("[IntentManaged(Mode.Merge, Signature = Mode.Fully)]");

                    if (Model.GetModelType() != null)
                    {
                        CSharpFile.AddUsing(Model.GetModelType().ParentModule.ApiNamespace);
                    }

                    @class.Partial();
                    @class.WithBaseType(Model.DecoratorContract != null
                        ? $"TypeScriptTemplateBase<{GetModelType()}, {GetTypeName(TemplateDecoratorContractTemplate.TemplateId, Model.DecoratorContract)}>"
                        : $"TypeScriptTemplateBase<{GetModelType()}>");

                    if (Model.GetTypeScriptTemplateSettings().TemplatingMethod().IsTypeScriptFileBuilder())
                    {
                        @class.ImplementsInterface(nameof(ITypescriptFileBuilderTemplate));
                    }

                    @class.AddField("string", "TemplateId", field => field.Constant($"\"{GetTemplateId()}\"").AddAttribute("[IntentManaged(Mode.Fully)]"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute("[IntentManaged(Mode.Merge, Signature = Mode.Fully)]");
                        ctor.AddParameter("IOutputTarget", "outputTarget");
                        ctor.AddParameter(GetModelType(), "model", p =>
                        {
                            if (Model.GetModelType() == null)
                            {
                                p.WithDefaultValue("null");
                            }
                        });
                        ctor.CallsBase(@base => @base.AddArgument("TemplateId").AddArgument("outputTarget").AddArgument("model"));

                        if (Model.GetTypeScriptTemplateSettings()?.TemplatingMethod().IsTypeScriptFileBuilder() == true)
                        {
                            ctor.AddStatement(new CSharpAssignmentStatement(
                                lhs: "TypescriptFile",
                                rhs: new CSharpInvocationStatement("new TypescriptFile")
                                    .AddArgument("this.GetFolderPath()")
                                    .AddArgument("this")
                                    .AddInvocation("AddClass", addClass =>
                                    {
                                        addClass.OnNewLine();
                                        addClass.AddArgument($"$\"{GetClassName()}\"");
                                        addClass.AddArgument(new CSharpLambdaBlock("@class"), configureClass =>
                                        {
                                            configureClass.AddStatement(new CSharpInvocationStatement("@class.AddConstructor"), addConstructor =>
                                            {
                                                addConstructor.AddArgument(new CSharpLambdaBlock("ctor"), configureCtor =>
                                                {
                                                    configureCtor.AddStatement(new CSharpInvocationStatement("ctor.AddParameter"), addParameter =>
                                                    {
                                                        addParameter.AddArgument("\"string\"");
                                                        addParameter.AddArgument("\"exampleParam\"");
                                                        addParameter.AddArgument(new CSharpLambdaBlock("param"), configureParameter =>
                                                        {
                                                            configureParameter.AddStatement("param.WithPrivateReadonlyFieldAssignment();");
                                                        });
                                                    });
                                                });
                                            });
                                        });
                                    })));
                        }
                    });

                    if (Model.GetTypeScriptTemplateSettings()?.TemplatingMethod().IsTypeScriptFileBuilder() == true)
                    {
                        @class.AddProperty(UseType("Intent.Modules.Common.TypeScript.Builder.TypescriptFile"), "TypescriptFile", property =>
                        {
                            property.AddAttribute("[IntentManaged(Mode.Fully)]");
                            property.WithoutSetter();
                        });

                        @class.AddMethod("ITemplateFileConfig", "GetTemplateFileConfig", method =>
                        {
                            method.AddAttribute("[IntentManaged(Mode.Fully)]");
                            method.Override();
                            method.AddStatement($"return TypescriptFile.GetConfig($\"{GetClassName()}\");");
                        });

                        @class.AddMethod("string", "TransformText", method =>
                        {
                            method.AddAttribute("[IntentManaged(Mode.Fully)]");
                            method.Override();
                            method.AddStatement("return TypescriptFile.ToString();");
                        });
                    }
                    else
                    {
                        @class.AddMethod("ITemplateFileConfig", "GetTemplateFileConfig", method =>
                        {
                            method.AddAttribute("[IntentManaged(Mode.Fully, Body = Mode.Ignore)]");
                            method.Override();
                            method.AddInvocationStatement("return new TypeScriptFileConfig", s =>
                            {
                                s.WithArgumentsOnNewLines();
                                s.AddArgument("className", $"$\"{(Model.IsFilePerModelTemplateRegistration() ? "{Model.Name}" : Model.Name.Replace("Template", ""))}\"");
                                s.AddArgument("fileName", $"$\"{(Model.IsFilePerModelTemplateRegistration() ? "{Model.Name.ToKebabCase()}" : Model.Name.Replace("Template", "").ToKebabCase())}\"");
                            });
                        });
                    }

                    if (Model.GetTypeScriptTemplateSettings()?.TemplatingMethod().IsCustom() == true)
                    {
                        @class.AddMethod("string", "TransformText", method =>
                        {
                            method.AddAttribute("[IntentManaged(Mode.Fully, Body = Mode.Ignore)]");
                            method.Override();
                            method.AddStatement("throw new NotImplementedException(\"Implement custom template here\");");
                        });
                    }
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(new TemplateRegistrationRequiredEvent(this));

            ExecutionContext.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                moduleId: IntentModule.IntentCommonTypeScript.Name,
                moduleVersion: IntentModule.IntentCommonTypeScript.Version));
            ExecutionContext.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                moduleId: IntentModule.IntentCodeWeavingTypeScript.Name,
                moduleVersion: IntentModule.IntentCodeWeavingTypeScript.Version));

            if (Model.GetModelType() != null)
            {
                ExecutionContext.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                    moduleId: Model.GetModelType().ParentModule.Name,
                    moduleVersion: Model.GetModelType().ParentModule.Version));
            }
        }

        private string GetAccessModifier()
        {
            if (Model.GetTypeScriptTemplateSettings()?.TemplatingMethod()?.IsTypeScriptFileBuilder() == true
                || Model.GetTypeScriptTemplateSettings()?.TemplatingMethod()?.IsCustom() == true)
            {
                return "public partial ";
            }
            return "partial ";
        }

        private string GetClassName()
        {
            return $"{(Model.IsFilePerModelTemplateRegistration() ? $"{{Model.Name}}" : Model.Name.RemoveSuffix("Template"))}";
        }

        private IEnumerable<string> GetBaseTypes()
        {
            if (Model.DecoratorContract != null)
            {
                yield return $"TypeScriptTemplateBase<{GetModelType()}, {GetTypeName(TemplateDecoratorContractTemplate.TemplateId, Model.DecoratorContract)}>";
            }
            else
            {
                yield return $"TypeScriptTemplateBase<{GetModelType()}>";
            }

            if (Model.GetTypeScriptTemplateSettings().TemplatingMethod().IsTypeScriptFileBuilder())
            {
                yield return nameof(ITypescriptFileBuilderTemplate);
            }
        }

        public string GetRole() => Model.GetRole();

        public string TemplateType() => "Typescript Template";

        public string GetTemplateId() => $"{Model.GetModule().Name}.{FolderNamespace}";

        public string GetDefaultLocation() => Model.GetLocation();

        public string GetModelType() => Model.GetModelName();

        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";
        public IList<string> OutputFolder => Model.GetParentFolders().Select(x => x.Name).Concat(new[] { Model.Name.ToCSharpIdentifier().RemoveSuffix("Template") }).ToList();
        public string FolderPath => string.Join("/", OutputFolder);
        public string FolderNamespace => string.Join(".", OutputFolder);

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{TemplateName}",
                @namespace: $"{OutputTarget.GetNamespace()}.{FolderNamespace}",
                fileName: $"{TemplateName}Partial",
                relativeLocation: $"{FolderPath}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}