# Intent.Modelers.Services

The Services Designer in Intent Architect is a powerful tool that allows developers to model "Application Services" for their applications. This module's primary focus is to define how your application can be interacted with at the service level, effectively allowing for the creation of internal services and publicly exposed endpoints.

## Purpose of Application Services

Application Services act as the contract for how client applications interact with the business logic encapsulated in your application. They manage communication between the client and the internal system, ensuring that the appropriate commands are executed and queries are returned. [🤔 Should we mention CRUD and CQRS?]

## Creating a Service with Operations

To create a service with operations:

- **Create a New Service**: Right-click on [🤔 Diagram/Package/Folder] and select `New Service` then provide it a unique Name.
- **Define Operations**: Right click on the Service and select `Add Operation` and then provide it a with a Name.
- **Add Parameters**: Right click on the Operation and select `Add Parameter`. Provide it with a Name and a Type. If the type is meant to represent an inbound payload, select the corresponding DTO.
- **Return Type**: If the Operation is not meant to return anything, leave the Type as `void`. Alternatively, select the appropriate Type from the Type dropdown to represent the return type.

## Creating a DTO

To create a DTO:

- **Create a DTO**: Right click on the Service Package or a containing Folder within and select `New DTO` then provide it a unique Name.
- **Create Field**: Right click on the DTO and select `Add Field`. Provide it with a Name and Type.

## Inheriting from a DTO for a DTO

Given that you have a DTO and a specific DTO you want to inherit from:

- **Derived DTO**: Right click on the DTO that will inherit from another DTO and select `New Inheritance`.
- **Base DTO**: Select the DTO from which you would like to inherit from.

## Mapping an Outbound DTO

Mapping outbound DTOs allows you to transform data from Domain entities to Data Transfer Objects.

- **Outbound DTO**: Right click on the DTO that will receive mapped information from a Domain Entity and select `Map From Domain`.
- **Domain Entity**: A dialog will open allowing you to specify the Domain entity and select the attributes you wish to include in the outbound DTO.
- **Complete the Mapping**: Check the boxes next to the attributes you would like to map and click `Done`. This establishes a clear link between your Domain data and the external interface represented by the DTO.
