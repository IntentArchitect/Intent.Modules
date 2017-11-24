# Roslyn Weaver 

## How to implement Roslyn Weaver in your templates

Enabling you templates to work with the Roslyn Weaver module is as simple as implementing the *IRoslynMerge* interface. Alternatively you can inherit you template from the *IntentRoslynProjectItemTemplateBase* base class which implements this interface already, as well as several other quality of life features.

```csharp
using Intent.SoftwareFactory.Templates;

//NOTE IRoslynMerge Interface
public class MyTemplate : ITemplate, IRoslynMerge
{
    ...

    public RoslynMergeConfig ConfigureRoslynMerger()
    {    
        return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
    }
 
    ...
}
````

There are typically 3 types of templates, these can be categorized by the nature of their output
- Largely Generated content
- Largely Hand coded content
- Event split of generated and hand coded content

Coming Soon (tm)

Use Partial Classes (With or without code behind), inheritance, composition
Mention Code Migrations
InitialCodeGen