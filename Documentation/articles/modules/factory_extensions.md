#Factory Extensions

## Overview

Factory Extensions are just that, a way for you to extend the code generation process. These extensions are typically for things orthogonal or supporting to the code generation process.

Some scenarios we have used factory extensions for:-
- Code Weaving
- Updating VS Project files based on generated output
- Installing Nuget dependencies
- Technology agnostic to technology specific type conversion system
- Appending File Headers to output

## Implementing a Factory Extension

Implementing a Factory Extension is as simple as implementing the IFactoryExtension interface. This class will be instantiated at the start of the process an live as a singleton throughout the generation process. For the extension to interact with the Factory in can implement one or more of the following interfaces.

## IDiscoverTypes
This interface allows you to discover Types registered, typically from other modules. 

This interface is typically used if you are supporting your own plugins, i.e. you have supplied an interface which other modules can supply implementations for.

## IExecutionLifeCycle
This interface provides you with hook points for all the steps within the code generation process, allowing you to execute code at specific points.

These steps are currently:-
- **Start** , the start of the process
- **BeforeMetaDataLoad** , before MetaData is loaded
- **BeforeTemplateRegistrations** , before templates are registered
- **BeforeTemplateExecution** , before any templates execute
- **AfterTemplateExecution** , after templates execute, before the changes have been presented to the user
- **AfterCommitChanges** , after template changes have been accepted by the user

The is constants class 'ExecutionLifeCycleSteps' which defines constants for the steps.

## ITemplateLifeCycle
This interface provides you with a hook point for interacting with each specific template instance execution. 

This template provides 2 hook points:-
- **PostConfiguration**, during the template configuration process, after configured values are applied, but before anyone is notified the template is ready to execute. Note the template instance is not yet registered with the project.
- **PostCreation**, before template execution but after configuration and any logic which occurs in template's IPostTemplateCreation implementations.

## ITransformOutput

This interface allows you to intercept the raw output from a template instance and manipulate it.