<!--
Important:

This is largely replicated at https://github.com/IntentSoftware/IntentArchitect/blob/master/docsProject/articles/make_your_own_module.md

Be sure to update both if editing.
-->

# Downloadable Example Module

The https://github.com/IntentSoftware/Modules.Examples repository contains an already put together simple example [Intent Architect](https://intentarchitect.com/) module.

## Pre-requisites

- Latest version of [Intent Architect](https://intentarchitect.com/#/downloads) installed.
- [Visual Studio](https://www.visualstudio.com/vs/) 2015 or later installed.
- (Optional) We highly recommend installation of a Visual Studio extension, such as [this free one](http://t4-editor.tangible-engineering.com/Download_T4Editor_Plus_ModelingTools.html), which adds syntax highlighting support for `.tt` files.

## Getting started

- Clone the [Modules.Examples](https://github.com/IntentSoftware/Modules.Examples) repository by running the following command: `git clone https://github.com/IntentSoftware/Modules.Examples`
- Using Visual Studio:
    - Open the `.\MyModules\MyModules.sln` *solution*.
    - Build the *solution* by using `Build Solution` (*Ctrl-Shift-B*) from the `Build` menu.
    - Leave Visual Studio open so you can come back to it later.
- Using Intent Architect:
    - Open the `.\MySolution\MySolution.isln` *solution* and its `MyApplication` *application*.
    - Run the Software Factory and observe that the output would like to create a `MathServiceController` and update the `.csproj` file. This is based on the metadata in the `Services` modeler.
    - Edit and save `Services` of the *application*, then run the Software Factory again to see how output is affected.
    - Leave Intent Architect open so you can come back to it later.

>[!Note]
>It's worth noting that you can change the implementation of each operation in the `MathServiceController` *without* your code being overwritten. This is due to the [Roslyn Weaving](https://intentarchitect.com/docs/modules/roslyn_weaver/overview.html) output transformer module `Intent.OutputManager.RoslynWeaver`.

## Editing the example module and seeing how it affects the generated code

- In Visual Studio:
    - Edit `.\MyModules\Module.Example.WebApi\Templates\WebApiControllerTemplate\WebApiControllerTemplate.tt` as desired.
    - Build the solution by using `Build Solution` (*Ctrl-Shift-B*) from the `Build` menu.
- In Intent Architect:
    - Run the Software Factory and see how the output changes.

## More information:

For a more detail on the parts and pieces of this example, we recommend that you read the [Creating a Module From Scratch](https://intentarchitect.com/docs/articles/getting_started/creating_a_module_from_scratch.html) article.