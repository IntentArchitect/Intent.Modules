# Metadata Navigation : Projects and Applications Model

This example serves to demonstrate the basics of code generation using the Project and Application Metadata models. These models can be seen in the _'Configuration'_ menu option in Intent Architect.

## Concepts shown
- Basic properties for Application and Project Metadata models
- Relationships between Applications, Projects and Template Instances

## AsXml Template

This template builds and XML file illustrating the Metadata available in these Metadata models. Looking at the data you can see if contains runtime information too, not just the Metadata you see in the configuration screen. This data includes things like all the template instances which are being executed during the software factory run.

This Metadata is not only available for templating, like normal Metadata, but also available to project or application based projects. Try access the Project property on any ProjectBased template and you will find this Metadata available.