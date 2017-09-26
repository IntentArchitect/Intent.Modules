# Intent Architect

Intent Architect is a coding automation tool for software developers. It's based on the principles of a software factory, and provides a platform for design and pattern reuse.
  - **Build Faster** - Allow Intent Architect to manage the tedious and repetitive tasks that take up most of your time, freeing you up to focus on meaningful value-adding development.
  - **Quality Assured** - Create your patterns, and then rest easy, knowing that they are applied consistently and correctly throughout your codebase.
  - **Agile Architecture** - Future-proof your architecture! Effortlessly upgrade (or even swap out) your technologies and design patterns at the push of a button - mitigating technical risk has never been easier.

## Getting started
[Register](https://intentarchitect.com/#/user-access/register) on our website and [download](https://intentarchitect.com/#/downloads) the latest release.

#### Quickstart

It takes just a few minutes to get a feel for the power of Intent Architect. For this quickstart we're going to create a "full stack" ASP.NET WebApi 2.2 server with persistence and domain patterns in place. If you haven't done so already, install the latest release and sign in to application using the credentials used for registration. Once signed in, in the application:
  1. Press 'Create New Solution' directly under the 'Start' heading at the top left of the initial screen.
  2. You are now on the screen for managing your new solution, press the 'New Application' tile.
  3. Select the 'Full Stack ASP.NET WebApi 2.2' tile and press the 'Next' button.
  4. Keep the default application 'Name' or change it, then press 'Create'.
  5. Give the application a short while to download and install the modules.
  6. Press the icon with a play button at the top right of the UI to run the software factory.
  7. After the software factory execution is complete, you are presented with a list of all pending changes, press the 'Apply' button for the outputs to be written to your disk drive.

You've now created the scaffolding code for a Visual Studio server solution, as well as installed all the required NuGet packages.

  8. Open the solution in Visual Studio 2015/2017 _(output will be in the location chosen for your application)_
  9. Compile and run (_hit F5_)

Play around in Intent Architect by adding Services, Domain classes and DTOs, and see how the patterns are realized in your codebase.

_(Important: You will need to add the "Aggregate Root" / "Entity" (Domain Driven Development concepts) stereotype to you Domain classes for them to generate. This is a requirement of the Intent.RichDomain module, currently being installed for this application template. To do so simply right-click the Domain class and select Add Stereotype)_

## How it works
Intent Architect works by feeding information (Metadata) about the application, and how this information should be realized (Modules), into a Software Factory, which then outputs code into your codebase. For more information, check out our website [https://intentarchitect.com/](https://intentarchitect.com/).

## Create your own Modules
Modules are a packaged set of a cohesive outputs which make up a pattern. This repository contains the source code for modules that have been created by the Intent Architect team to deliver complex business applications for existing clients.

Module output is merely text, allowing development of patterns for any existing or future programming language, but creating a module must be done using C# and .NET (a modern, powerful and strongly typed language).

Tutorials and documentation for creating your own modules is on the way, but we have made our existing modules open source so that anyone can see how they work or copy/edit them for their own purposes.
