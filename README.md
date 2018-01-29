# Intent Architect

## Module Examples
This folder working examples of the features available to developers wanting to build their own modules. These demos allow you to interact with the examples learning and exploring the many options available to you.

## Modules Quick Start

## Intent Documentation

## To Clean Up

Intent Architect is a coding automation tool for software developers. It's based on the principles of a software factory, and provides a platform for design an pattern reuse.
  - **Build Faster** - Allow Intent Architect to manage the tedious and repetitive tasks that take up most of your time, freeing you up to focus on meaningful value-adding development.
  - **Quality Assured** - Create your patterns and rest easy knowing that they are consistently rolled out throughout your codebase.
  - **Agile Architecture** - Future-proof your architecture! Effortlessly upgrade or even swap out your technologies and design patterns at the push of a button. Mitigating technical risk has never been easier.

## Getting started
[Register](https://intentarchitect.com/#/user-access/register) on our website and [download](https://intentarchitect.com/#/downloads) the latest release.

### Quickstart
To get a feel for the power of Intent Architect, let's create a "full stack" ASP.NET WebApi 2.2 server with persistence and domain patterns in place. Install the latest release and sign in to application using the credentials used for registration. In the application:
  1. Create a new solution
  2. Create an application in the solution using the "Full Stack ASP.NET WebApi 2.2" template.
  3. Run the software factory (play button in the top right)
  4. Apply the code outputs

Here we've create the scaffolding code for the server, as well as installed all the required NuGet packages.

  5. Open the solution in Visual Studio 2015/17 _(output will be in the location chosen for your application)_
  6. Compile and run (_hit F5_)

Play around by adding Services, Domain classes and DTOs, and see how the patterns are realized in your codebase.

_(Important: You will need to add the "Aggregate Root" / "Entity" (Domain Driven Development concepts) stereotype to you Domain classes for them to generate. This is a requirement of the Intent.RichDomain module, currently being installed for this application template. To do so simply right-click the Domain class and select Add Stereotype)_

## How it works
Intent Architect works by feeding information (Metadata) about the application, and how this information should be realized (Modules), into a Software Factory, which then outputs code into your codebase. For more information, check out our website [https://intentarchitect.com/](https://intentarchitect.com/).

## Create your own Modules
Modules are made up of a cohesive set of outputs which make up a pattern. This repository contains the modules that have been created by the Intent Architect team to deliver complex business applications for existing clients.

While the output of each module is language agnostic, creating the modules must be done using C# and .NET (a modern and powerful strongly typed language).

Tutorials and documentation for creating your own modules is on the way, but in the meantime feel free to check out our existing modules to see how they work.