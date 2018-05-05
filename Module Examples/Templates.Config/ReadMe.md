# Templates : Configuration

This example serves to demonstrate the various configuration options supported by templates.

Template configuration with in Intent Architect allows you templates to be more extensible and reusable.

## Concepts shown
- Make your templates generic and/or reusable through runtime configurable variables.
- How to data bind your variables to the template context
- How to wire up the configuration infrastructure in custom scenarios

## Template Variables Template
This template demonstrates the basic usage scenario of variables for templates. The primary use case is to allow template consumers to configure the behaviour of the template at run time through the Intent Architect UI. In this example there are two variables declared (Variable1, Variable2). These variables are declared in the imodspec file.

```xml
    <template id="Templates.Config.TemplateVariables" enabled="true">
      <role>Default</role>
      <config>
        <!-- Specify the  exposed configurable variables, with optional default values-->
        <add key="Variable1" description="Variable 1 description" default="Variable1Default" />
        <add key="Variable2" description="Variable 2 description" default="Variable2Default" />
      </config>
    </template>
```
These variable are now configurable through the Intent Architect UI.

![Configuring template variables through the UI ](images/UIConfig.png)

These variables can be accessed and coded against in the template. These variable are found in the FileMetaData.CustomMetaData dictionary.

```csharp
        //You can encapsulate the config through properties like this
        public string Variable1
        {
            get
            {
                return FileMetaData.CustomMetaData["Variable1"];
            }
        }
```

> [!WARNING]
> Be careful accessing these variable before the template has been fully configured, e.g. in the template constructor. Accessing them during the template transformation is safe, alternatively you can implement the _'IPostTemplateCreation'_ interface, which provides a hook point for Logic after template configuration has happened.

## Data Bound Config Template
This template builds on the concepts shown _'Template Variables Template', but introduces the concept of binding the variables to values on the template. Looking at the imodspec file you will see the following.
```xml
        <add key="Variable1" description="Project Name" default="${Project.ProjectName}" />
        <add key="Variable2" description="Model Property" default="${CustomModel.Name}" />
        <add key="Variable3" description="Code Behind Variable" default="${Variable3}" />
```

Note the ${_expression_} syntax, this syntax allows you to refer to properties on your template. These variables are then bound or substituted to the actual values at runtime. This binding can also be used within the Intent Architect UI, as well in your code template when defining FileMetaData(see example below). 

```csharp

            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}Partial",
                fileExtension: "cs",
                defaultLocationInProject: "Domain",
                className: "${Model.Name}",
                @namespace: "${Project.ProjectName}",
                dependsUpon: partialClass?.GetMetaData().FileNameWithExtension()
                );
```
