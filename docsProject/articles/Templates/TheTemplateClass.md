# The Template Class

Templates are completely generic and can be implemented how ever you like, at it's core you simply have to implement this interface.

```csharp

    public interface ITemplate
    {
        //Unique Identifier for the template
        string Id { get; }
        //Metadata about the generate file, i.e. filename, output location, etc
        IFileMetaData GetMetaData();
        //Method which generates the template output i.e. the generate file
        string RunTemplate();
    }

```

Practically you don't to implement this interface yourself, but rather you can inherit from one of the base templates supplied in 'Intent.Module.Common' available on Nuget. The supplied base templates are as follows.
- **[IntentRoslynProjectItemTemplateBase](#intentroslynprojectitemtemplatebase)**, base template for C# code generation, supports code weaving scenarios.
- **[IntentTypescriptProjectItemTemplateBase](#intenttypescriptprojectitemtemplatebase)**, base template for TypeScript code generation.
- **[IntentProjectItemTemplateBase](#intentprojectitemtemplatebase)**, base template for any generation where the output belongs to a project.
- **[IntentTemplateBase](#intenttemplatebase)**, base template for any generation where the output does not belong within a project.

## Base Templates

### IntentRoslynProjectItemTemplateBase
This base template is intended for templates generating C# files. There are 2 versions of this template one for template which are bound to data (`IntentRoslynProjectItemTemplateBase<TModel>`) and one for templates which are not bound to data(`IntentRoslynProjectItemTemplateBase`). 

This template has build in support for 'Roslyn Weaver' which allows for advanced code weaving scenarios.
This template supports T4. 

### IntentTypescriptProjectItemTemplateBase
This base template is intended for templates generating TypeScript (ts) files. There are 2 versions of this template one for template which are bound to data (`IntentTypescriptProjectItemTemplateBase<TModel>`) and one for templates which are not bound to data(`IntentTypescriptProjectItemTemplateBase`). 

This template supports T4. 

### IntentProjectItemTemplateBase
This base template is intended for templates generating code into project structures. If one of the previous two templates is not what what you are looking for this is probably what you want to use.There are 2 versions of this template one for template which are bound to data (`IntentProjectItemTemplateBase<TModel>`) and one for templates which are not bound to data(`IntentProjectItemTemplateBase`).

This template supports T4. 

### IntentTemplateBase
This base template is intended for templates generating code that does not fall into project structures. 99% of the time this is not what you want.

This template supports T4.

## Implementing your class

Once you have chosen which type of template you want to create simply create a C# class and inherit from that class. Below is an example of a 'IntentRoslynProjectItemTemplateBase<TModel>' template, `SomeMetaDataModel` is the Metadata model or data model we intend on using.

```csharp

    public class MyFirstTemplate : IntentRoslynProjectItemTemplateBase<SomeMetadataModel>
    {
        public const string Identifier = "MyProjectOrOrganization.MyFirstTemplate";

        public MyFirstTemplate(IProject project, SomeMetadataModel model)
            : base(Identifier, project, model)
        {

        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            //This is covered in the Configuration section
            throw new NotImplementedException();
        }

        public override string TransformText()
        {
            //This is covered in the Code Output section
            throw new NotImplementedException();
        }

    }

```