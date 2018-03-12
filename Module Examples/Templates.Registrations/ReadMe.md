# Templates : Registrations

Template registrations do 2 things:-
- Allow Intent Architect to discover your template
- Bind your template instances to their specific Metadata

## Concepts shown
- Registering your templates with Intent Architect there by allowing you to create and re-use your own patterns.
- Bind your template to Metadata

## OneTemplateToOneModel Template

This template demonstrates the registration of a template not bound to Metadata models in a 1 - 1 fashion, for example if in your Metadata had 10 uml classes and wanted to generate 10 class file outputs, this is the registration for you.

## OneTemplateToManyModels Template

This template demonstrates the registration of a template not bound to Metadata models in a 1 - Many fashion, for example if in your Metadata had 10 uml classes and wanted to generate 1 class file which needs to know about all 10 uml classes, this is the registration for you.

## NoModel Template

This template demonstrates the registration of a template not bound to data. 

## CustomBinding Template

Custom Registrations allow for all the above scenarios and other more specialized scenarios, for example:-
- a non data bound template which output in multiple projects (e.g. any project item infrastructure like web.config or assemblyInfo.cs )
- Many to Many relationships between Metadata and Template instances
- Any other scenario you can think of.