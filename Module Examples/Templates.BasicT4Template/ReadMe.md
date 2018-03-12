# Templates : Basic T4 Template

This example services to demonstrate setting up a basic T4 Template.

Templates with in Intent Architect are the building blocks used to describe code, more specifically the code patterns you would generated.

## Concepts shown
- T4 templating basics, basic mechanics of T4 you will require to make your own templates e.g. loops, conditionals, binding to the code behind etc.
- Template File Metadata, template provide File Metadata describing the nature of the content they are creating, for example its file name, project location and file extension.
- Template base classes

## VanillaT4 Template
T4 is a templating language, not unlike Razor or the ASP / JSP Engines. This example illustrates the basics of T4 templating including
- Coding in the template
- Loops
- Conditional logic
- Declaring variables 
- Binding data from the code behind
- Invoking methods on the code behind
- Methods in templates

## RoslynWeaver Template
This examples is identical to the one above with one caveat, it allows support for the Roslyn Weaver Factory Extension. Roslyn weaver introduces code weaving to generated c# files, allows consumers of your generated code to manipulate the generated code files.  

This examples serves to show how easy it is to support code weaving in C# files.

To test it out add the following code into the generated file _'MyApp.Library\Templates\BasicT4Template\RoslynWeaverT4\RoslynWeaverT4.cs'_.

```
    [IntentManaged(Mode.Ignore)]
    public void ImHereToStay()
    {
        //Logic
    }
```