# Template Configuration

There are two aspects to template configuration
- **Template Class Configuration**
- **IModSpec Configuration**

## Template Class Configuration

There are several flavours of these configurations, but they are all very similar, you are basically describing the nature of the template and it's output. Here is an example.

```csharp

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "MyFirstTemplateFolder",
                className: "${Model.Name}",
                //remove the "_" from "@namespace_" bug in the documentation tool
                @namespace_: " ${Project.Name}.MyFirstTemplateFolder" );
        }

```

>[!NOTE]
>${...} is a binding syntax. See below for more information.

In the above example the configuration settings mean the following.
- **overwriteBehaviour**, this has 2 options, `Always` and `OnceOff`, `Always` will always generate and overwrite the solution file, `OnceOff` will only create the solution file if it does not exist.
- **fileName**, the file name of the generated solution file.
- **fileExtension**, the file extension of the generated solution file.
- **defaultLocationInProject**, the relative path to put the generated solution file with it's project.
- **className**, the class name of the generated class with the generated solution file
- **namespace**, the namespace of the generated class with the generated solution file

There is also a `Dictionary<string, string>` called `CustomMetaData` which allows you to define any additional parameter you wish to define, these can be exposed for end user configuration through the IModSpec file.

>[!NOTE]
>These are default values, i.e. should you choose to expose these variables through the IModSpec files, consumers can change the values of these settings.

>[!NOTE]
>Once a template instance has been created, calling `GetFileMetaData()` will return the actual configured values, the template will execute with.


## IModSpec Configuration

This configuration is similar to 'Template Class Configuration' but is exposed to consumers of your template through Intent Architect, i.e. this configuration can be adjusted "runtime". And allow you to support configurable features on your templates.

Sample IModSpec configuration.

```xml
    <config>
    <add key="fileName" 
        description="The generated output's file name" 
        default="${Model.Name}" />
    </config>
```

In this example a consumer of the template can now change the generated files names. 

>[!NOTE]
>Only variables declared in the IModSpec file can be configured by consumers.

## Binding Syntax

The binding syntax can be used within your configuration. The binding is relative to you template class, i.e. you can bind to any properties or methods within your template. The syntax is `${`expression`}`. The reason to use this syntax is to make it possible for others to see your configuration an rebind it, if you want to support this scenario.