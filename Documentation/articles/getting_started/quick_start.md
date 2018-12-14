# Quick Start

This quickstart guide will take you through the steps required to create an ASP.NET Core web application using the open-source modules created by the Intent team. The aim is to show how you can describe your system in high-level models and allow a technology specific set of modules (ASP.NET Core modules in this guide) to realize that into code.

The open-source modules repository can be found [here](https://github.com/IntentSoftware/IntentArchitect).

## 1. Installing and running Intent Architect

Get Intent Architect up and running through the following steps.

- Download the latest version of Intent Architect from the website: [https://intentarchitect.com/#/downloads](https://intentarchitect.com/#/downloads)
- Register an account: [https://intentarchitect.com/#/user-access/register](https://intentarchitect.com/#/user-access/register).
- Double click the downloaded executable to install and run Intent Architect.
- Log in using the credentials created for your account.

## 2. Create a workspace

A `workspace` provides the root location of your solution. This is typically the root folder of you version control repository. It may for example contain a single monolithic application or several microservices.

**From the home screen, click `Create new workspace...` under the 'Create' header.**

Fill in the Name, Description (optional), and Location of your workspace. You can also change the icon by clicking on it.

![New Workspace Dialog](../../images/quick_start/create_workspace.png)

*Create Workspace Dialog*

|Field|Description|
|-|-|
|Icon|This is an icon which will be associated with the *workspace*. By default a random icon is chosen, but it can be changed at any time.|
|Name|The name of your workspace. This name should be alpha-numeric with no special characters or spaces.|
|Description|Any description or comments about this workspace for yourself (or your team).|
|Location|The folder where Intent Architect will save data for the workspace. This should point to a path under the same Source Control Management as your normal source code, so that it can be versioned and shared amongst the development team in the same way. Intent Architect does not append any additional sub directories, the directory selected will be the directory used.|

You will be presented with the `Create Solution` Dialog.

>[!TIP]
>Good names for Solutions would typically be things like the client name, your product name, business area / unit or your organization name.

**Once the workspace details have been filled out, click `Save`.**

The workspace will open automatically.

## 3. Create a new Application

Next we will create an `Application`. An application could represent a standalone monolithic application, a mircroservice, or simply a location for Intent Architect to generate code into.

**To create an application, click the `New Application` tile from within the workspace.**

A `Create application` wizard will be displayed, presenting a set of `application templates`.
- Select the `Web Application ASP.NET Core` application template.
- Capture the Name, Location, Icon and Description (option) of your application.

![Select an Application Template](../../images/quick_start/create_application_page1.png)
*Select an application template and fill out application details.*

|Field|Description|
|-|-|
|Name|The name of the application. This name should be alpha-numeric with no special characters or spaces.|
|Icon|This is an icon which will be associated with the *application*. By default a random icon is chosen, but it can be changed at any time.|
|Location|The folder where your application's data will be persisted. By default this will be in a sub-folder of the solution named after the Application name.|
|Description|A description of the Application.|
>[!TIP]
>`Application templates` like the ones show above are not baked into Intent Architect. They can be created relatively easily and are a great way to bundle modules into a comprehensive technology set. They can also define the default `Project Layout`. See the open-source Intent Architect GitHub repository (https://github.com/IntentSoftware/IntentArchitect) for examples on how to build application templates.

**Click `NEXT`.**

![Capture Application Details](../../images/quick_start/create_application_page2.png)
*Capture Application Details*

**Leave the Installation options as is and click `CREATE`.**

Intent Architect will create the application as defined by the `Web Application ASP.NET Core` application template and begin downloading all the necessary modules. Once downloading is complete the application will be opened automatically. The `Installation Manager` can now be closed.

## 4. Push the `Play` button

Your Application is now created, and pre-configured with various Modules and Metadata from the Application Template. 

![View of an Application](../../images/quick_start/new_application.png)
*Application View*

**Click the `Play` button, in the top right hand corner.**

The `Run Software Factory` dialogue will appear, providing feedback of and interaction with the code generation process. Not worrying too much about the presented `Console` entries, once the Software Factory has run, the `Changes` tab is presented showing a list of all pending code changes. Reviewing the list, we can see it is wanting to create a Visual Studio solution, made up of the `.sln` file as well as `csproj` and various other infrastructura files.

![Execution of the Software Factory](../../images/quick_start/software_factory_execution1.png)
*Software Factory execution 'Changes'*

**Click the `Apply Changes` button.**

At this point the software factory applies all the pending code changes from the list and then downloads and installs any required NuGet packages, depending on your computer and internet speed, this may take a little while. These NuGet packages were installed based on the configured Module dependencies, once downloaded and installed, they will not need to be downloaded or installed again on future presses of the `Play` button.

**Click the `Close` button.**

At this point you can navigate to the folder on your hard drive which you specified for your application and you should see something like this:

![View of Generated Output](../../images/quick_start/generated_application.png)
*Generated outputs*

Open the solution in Visual Studio 2015/2017 (output will be in the location chosen for your application).

Compile and run (hit F5).


## What just happened?

What may not obvious at this point is what caused the code to be generated, and why it was generated like that. 

If you look at the Modules section of you application, on the installed tab you should see something similar to this:

![View of Modules](../../images/quick_start/modules.png)
*Installed Modules*

Here you can see there are a collection of modules which have been installed by the application template. Each of these modules is affecting what code is generated as well as how it is generated. 

You can try uninstall and reinstalling Modules, then pressing the `Play` button to see what affect they have on the code generation.

In a less contrived scenario you would hand pick which modules you wished to use or create your own.

Another aspect which is affecting the code generation is the Application Configuration. If you go the `Configuration` section, you will see the following:

![View of Application Configuration](../../images/quick_start/configuration.png)
*Application project configuration*

This is Metadata describing how you want your actual source code to be structured in addition to where you would like the code generation from installed modules to go. This is done by mapping `Target Roles` from the `Modules` onto your project structure.

>[!NOTE]
>The project types available are again supplied by modules, in this case specifically, from the Intent.VisualStudio Module. New project types can be added through the Modules system.    

## Add additional Metadata to describe your Application
Play around in Intent Architect by adding Services, Domain classes and DTOs, each time pressing `Play` to see how the patterns are realized in your codebase.
