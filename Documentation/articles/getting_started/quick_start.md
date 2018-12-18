# Getting Started - create an ASP.NET Core web app.

This guide will take you through the steps required to create an ASP.NET Core web application using the open-source modules created by the Intent team. The aim is to show how you can describe your system in high-level models and allow a technology specific set of modules (ASP.NET Core modules in this guide) to realize that into code.

The open-source modules repository can be found [here](https://github.com/IntentSoftware/IntentArchitect). The modules are designed to support a hexagonal architecture (a.k.a. ports and adapters architecture), and the patterns are enterprise grade and production ready.

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

The `Web Application ASP.NET Core` application template provides a set of components which can be turned on or off. Each component represents one or more modules that will be installed. By default the main components are selected, with others presented as options.

We will want to test our services later, and for that we can use the [Swagger UI](https://swagger.io/tools/swagger-ui/).

**Select the `Swagger (Swashbuckle)` component for installation.**

![Capture Application Details](../../images/quick_start/create_application_page2.png)
*Select application components to install*

>[!TIP]
>You will be able to add or remove modules later, so don't worry about ensuring you get everything you need installed upfront.

**Click `CREATE`.**

Intent Architect will now create the application as defined by the `Web Application ASP.NET Core` application template and begin downloading all the necessary modules. Once downloading is complete the application will be opened automatically.

Applications open by default on the `Project Layout` section. This application template lays out the projects in the following way:

![Project Layout](../../images/quick_start/configuration.png)

*Project Layout*

>[!TIP]
>The `Project Layout` is completely configurable. Add, remove or rename projects in accordance with your desired naming conventions. In order for Intent Architect to know in which project to place code generated by the modules, it uses `Roles` which are assigned to a project. Expand a project to see it's assigned `Roles`. By moving these `Roles` between projects, we can effectively decide where code is to be outputted.


## 4. Run the `Software Factory`

Your application is now created, and pre-configured with a `Project Layout` and modules for realising an ASP.NET Core web application. Now to generate the code, we must run the `Software Factory`. 

**To run the `Software Factory`, click the _'play'_ button in the top right hand corner (or press F5):**

![Run Software Factory](../../images/quick_start/play-button.png)

>[!TIP]
>The _'bug'_ button to the left of the _'play'_ button allows you to attach a debugger to the `Software Factory` process. This is useful for debugging custom modules that you have built.

The `Run Software Execution` dialogue will appear, providing an output log in the `Console` tab, followed by staging files it intends on creating, updating or deleting in the `Changes` tab:

![Execution of the Software Factory](../../images/quick_start/software_factory_execute.gif)
*Software Factory execution*

The `Software Factory` hasn’t created or altered any files at this point. The files are staged so that it is clear to the developer what the software factory is intending on doing. By clicking on one of the files, Intent Architect will open a diff-tool (by default Visual Studio) to compare changes. 

>[!TIP]
>Try clicking the `Startup.cs` file to see what Intent Architect intends to output. Note that the default Visual Studio installation locations are inspected to automatically wire up the diff-tool. If not found, an error will be shown. Any diff-tool can be used, and can be changed under your profile settings located in the top right hand corner, to the right of the Software Factory _"play"_ button..

**Click the `APPLY CHANGES` button.**

The `Software Factory` will apply the staged code changes from the list. We can now close the `Software Factory Execution` window.

**Click the `CLOSE` button.**

## 4. Run the application using Visual Studio

At this point, we've told Intent Architect that we want an ASP.NET Core web application, and allowed it to generate the required files for just that. To open the solution in Visual Studio, we need to navigate to the application's folder. An easy shortcut to the code is to navigate to the `Project Layout` section in the aside menu, and click on the `Open in Folder` button.

The folder should look something like this:

![View of Generated Output](../../images/quick_start/generated_application.png)
*Generated outputs*

**Open the solution (e.g. `MyMovies.sln` file) in Visual Studio 2017** or later (since this is a .NET Core application). The solution will look as follows:

![Visual Studio Solution](../../images/quick_start/visual_studio_solution.png)

*Visual Studio solution layout*

Allow Visual Studio to restore NuGet dependencies then **compile and run the application by clicking `Debug` -> `Start Debugging` menu items** (or press F5).

The server will be launched locally. **Navigate to `/swagger` relative url to open the Swagger UI.**

![Empty Swagger](../../images/quick_start/swagger_empty.png)
*Swagger UI - No operations defined in spec!*

Since we haven't described any services, the Swagger UI will be empty. Let's now begin describing what we want our web server to do (_describing our "intent"_).

## 5. Describing a Domain
A good place to start designing a system is with the Domain. The Domain should represent the business entities and their relationships to one another. 

>[!TIP]
>Modeling domains in this way brings _visibility_ and _transparency_ to your business layer. Developers can discuss design decision amongst each other (and even with business) with the knowledge that the diagram they are looking at represents the truth of the underlying code. In his book, _Domain-Driven Design: Tackling Complexity in the Heart of Software_, Eric Evans describes the value of having a model to align developers and domain experts.

To model our Movies domain, **navigate to the `Domain` modeler.**

Let's now a add a class called `Movie` to our Domain. To add a class, **right-click the diagram view and click `Add Class`.**

The newly added class automatically allows you to rename it in the model window on the right. **Rename it to `Movie`.** Alternatively, right-click the class and click `Rename` (or press F2).

Next let's add attributes to our `Movie` class. To add an attribute, **right-click the class and click `Add Attribute` (or press ctrl + shift + a).**

**Add the following attributes:**

![Movie class](../../images/quick_start/domain_movie_class.png)

*Domain Modeler - adding classes and attributes.*

To change the `Type` of the attribute, select the desired type from the dropdown inside the `Properties` window that is displayed on selecting an element in the diagram.

The following `gif` illustrates this process:

![Adding a Domain Class](../../images/quick_start/domain_create_movies.gif)
*Domain Modeler - adding classes and attributes.*

**Press the _'save'_ icon to save your `Domain` model, and re-run the `Software Factory`.**

The changes should look as follows:

![Adding a Domain Class](../../images/quick_start/software_factory_domain_changes.png)
*Software Factory Execution - Domain changes.*

>[!NOTE]
>Looking through the changes above we can see the `Movie.cs` entity, it's state `MovieState.cs` and interface `IMovie.cs` being created, with a [repository](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design) interface `IMovieRepository.cs` and [specification](https://deviq.com/specification-pattern/) object `MovieSpecification.cs`. These reside in the domain project (`MyCompany.MyMovies.Domain.csproj`), and as part of the architecture, none of these classes or interfaces has any _technology_ dependencies. 

>In the infrastructure project (`MyCompany.MyMovies.Infrastructure.csproj`) we see the [Entity Framework](https://docs.microsoft.com/en-gb/ef/core/) mapping `MovieMapping.cs` and the repository implementation `MovieRepository.cs`. These patterns are determined by the modules that were installed on application creation.

**Click `APPLY CHANGES` and close the `Software Factory Execution` window.**

## 6. Describing Services
Next, we want to create services to create and access our Movies. To do this, we must **navigate to the `Services` modeler.**



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
