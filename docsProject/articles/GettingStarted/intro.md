# Welcome to Intent Architect

## What is Intent Architect ?

Intent Architect is a coding automation tool for software developers. It's based on the principles of a software factory, and provides a platform for design and pattern reuse.

- Build Faster - Allow Intent Architect to manage the tedious and repetitive tasks that take up most of your time, freeing you up to focus on meaningful value-adding development.
- Quality Assured - Create your patterns, and then rest easy, knowing that they are applied consistently and correctly throughout your codebase.
- Agile Architecture - Future-proof your architecture! Effortlessly upgrade (or even swap out) your technologies and design patterns at the push of a button - mitigating technical risk has never been easier.

## How does it work ?

Conceptually Intent Architect works in 3 steps:- 
- Describe your application through Meta data
- Choose your patterns and their backing technologies through Modules
- Click the play button 

![Image of IA Overview](../../images/HowItWorks.png)
*Overview of how Intent Architect work*

In practice you would apply these steps organically and iteratively as you build out your application, describing your application and refining your design as you model your business domain, patternizing your application architecture either explicitly or in an emergent fashion depending on your experience. 

So what is involved in these 3 steps ?

### Describe your application through Meta data
In this step you can describe your application or aspects of your application in a technology agnostic way. Some of the typical things you might wish to describe are
- Domain model
- Service Contracts
- Services
- Inbound and outbound events
- Source code structure
- ANY other aspect of your application you feel is appropriate

Am I not just documenting my design ? Is principle yes, but unlike traditional documentation this is a living document which *integrates* into your actual source code. As you change your design (Meta data) your application source code changes. Through this process you capture the design intent of your application rather than hard coding it into your application source code as it is traditionally done. This decoupling of the design intent from the actual source code makes refactoring or changing your designs easier as you are literally changing the design rather have to reverse engineer it out of the source code and refactor the code.

Intent Architect has serval Domain Specific Languages (DSLs) which you can use to model the application concepts described above. An example of such a DSL would be our UML modelling 



All the DSLs are fully extensible though a custom meta data extension system simply referred to as *stereotypes*. Stereotypes allow you to extend existing models and add your own custom data to the existing model. This would typically be data that is specific to you application or design. Should you wish to describe other aspects of your application that Intent Architect does not have a DSL for  you can provide your own Meta Data models. These can be in what ever format you like, some examples may include JSON, XML or .Net source code. This can be done through creating your own MetaDataLoader plugin.

It is important to note you do not have to describe your entire application upfront, you can describe your application iteratively as you uncover the domain you are building for. 

### Choose your patterns and their backing technologies through Modules

### Click the play button

## Why is this a better way to build software ?

Truly patternized solution

Encapsulated Technology Stacks

Avoid the "It's too much work to do it right, lets do it good enough"

Refactor Architecture

Softcoded architecture 

Industrialized architecture vs Artisan architecture

Your design / intent is no longer hard coded into your application, it can be revisited 

Visualization of architecture which easily consumable by the development team, and more consumable by system / business analysts