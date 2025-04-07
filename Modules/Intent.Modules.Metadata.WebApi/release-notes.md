### Version 4.7.6

- Fixed: `Http Settings` stereotype could not be applied on `Azure Function` elements.

### Version 4.7.5

- Improvement: Added support for `Domain` and `Services` naming conventions for `Entities`, `Attributes` and `Operations`.
- Fixed: When an empty `Default API Route Prefix` is supplied, it will no longer create a route starting with `/`.

### Version 4.7.4

- Improvement: `Http Settings` can now be applied to `API Gateway Routes`.

### Version 4.7.3

- Improvement: Enhanced the heuristics for auto-generating routes for Commands, Queries, and Operations. 

### Version 4.7.2

- Improvement: Added new documentation topics around securing services.
- Fixed: Route path is now correctly calculated when the "Default API Route Prefix" is set to blank.

### Version 4.7.1

- Improvement: `Services Designer` warning if the HTTP route contains double forward slash (`//`)
- Improvement: New extension methods introduced (`HttpEndpointExecutionContextExtensions`) that would now also attempt to include the API Setting `Secure By Default` to determine whether services are secure or not.

### Version 4.7.0

- Fixed: Exposing Services/Commands/Queries as HTTP Endpoints will now correctly position the route parameters and include the operation names for Domain Entity Operations being exposed through Commands/Queries.
- Improvement: Added `Suggestion` for `Exposing as Http Endpoint` as well as a number of documentation topics
- Improvement: Updated `Expose HTTP` menu item icon to align with module icon

### Version 4.6.4

- Improvement: The `Secured` Stereotype can now be applied multiple times to an element to represent an `AND` security requirement.
- Improvement: An `OpenAPI Settings` Stereotype can now be applied to the types `DTO Field` and `Parameter`, allowing for example values to be set to reflect in the OpenApi spec.

### Version 4.6.3

- Improvement: Services designer will now show a validation error when an HTTP endpoint has more than one parameter's source is set to `From Body`.

### Version 4.6.2

- Improvement: Updated `Services Designer` validation for `routes` to cater for multitenancy route parameter
- Fixed: An incorrect route would be applied elements to when applying the `Api Version Settings` stereotype and the `Intent.AspNetCore.Controllers` module was not installed.

### Version 4.6.1

- Fixed: Default API route path when API Settings are not present.

### Version 4.6.0

- Improvement: Certain element types will now show validation errors if their name is not unique.
- Fixed: Header Names are required fields which will be enforced when Header binding is selected.
- Fixed: Removed Legacy behaviour where `Services` without `Http Service Settings` stereotypes would result in interpreting the Operations with `Http Settings` to have a base route of `api/[controller]`.

  > ⚠️ NOTE
  >
  > If you are using ASP.NET Core Controllers (using the `Intent.AspNetCore.Controllers` module) and you don't want the `[Route("api/[controller]")]` attribute presented on some of your Controllers, ensure you update it to the latest version.  

### Version 4.5.7

- Fixed: Queries and Service Operations exposed as HTTP endpoints will now set `Return Type Mediatype` to `application/json` for primitive return types.

### Version 4.5.6

- Improvement: Added support for customizing Success Response Code.

### Version 4.5.5

- Improvement: Improved service security modeling experience.
- Improvement: Added an _Ignore_ option to the _OpenAPI Settings_ stereotype. Modules (e.g. _Intent.AspNetCore.Controllers_) can use this property to know to apply the relevant annotations to prevent API endpoints from being generated in OpenAPI specifications.

  > ⚠️ NOTE
  >
  > The behaviour of the _OperationId_ property has now changed.
  > 
  > In prior versions of this module, a blank value would be equivalent to having specified the method name of the endpoint it was applied to. From this version onwards it will no longer have any effect without a value specified.
  >
  > To avoid disruptions by this change, a once off automatic migration is run during the upgrade of this module which will automatically change all values of this property to `{MethodName}` which will result in the equivalent generated code as before.

### Version 4.5.4

- Improvement: `Exposing as HTTP Endpoint` now pluralizes the entity e.g. `api/customers` was previously `api/customer`.
- Fixed: Wrongfully validated `Route mismatch: some route parameters do not match element's properties/parameters. Unmatched parameters: {version}` as an error.
- Fixed: Incorrectly didn't identify conflicting route paths that only were distinct on route parameter name.
    - For example the following should be conflicting route paths:
        - root/sub/{param1}
        - root/sub/{param2}

### Version 4.5.3

- Fixed: Route parameter generation for CQRS Services fixed that are generated from Service Proxies.

### Version 4.5.2

- Fixed: Issue where route path validation breaks when tokens are complex.

### Version 4.5.1

- Fixed: Enums weren't accepted as values that can be passed in via route paths.

### Version 4.5.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

### Version 4.4.4

- Improvement: Add support for exposing HTTP endpoints for Commands / Queries mapping to elements that are not Classes (limited currently to Repositories).

### Version 4.4.3

- Fixed: Check for unmatched route parameters.
- Improvement: Added a `FileTransfer` type, for modeling REST file upload and downloads

### Version 4.4.2

- Fixed: Check for undefined route parameters.

### Version 4.4.2

- Fixed: Any [`~/`s at the start of a route for an operation / command / query](https://learn.microsoft.com/aspnet/core/mvc/controllers/routing#attribute-routing-for-rest-apis) were not being respected.

### Version 4.4.1

- Fixed: `Expose as Http Endpoint` didn't resolve ID route parameters for advanced mapping and class inheritance scenarios.  

### Version 4.4.0

- Improvement: _Expose as Http Endpoint_ script updated to work with new Advanced Mappings.

### Version 4.3.3

- Fixed: Duplicate route detection validation to accommodate `[controller]` in rest routes.

### Version 4.3.2

- Improvement: It is now possible to specify a [custom name for query strings](https://github.com/IntentArchitect/Intent.Modules/blob/development/Modules/Intent.Modules.Metadata.WebApi/README.md#parameter-settings-query-string-name-property) on HTTP endpoint parameters.

### Version 4.3.1

- Improvement: Added a new `OpenAPI Settings` stereotype which can be used to control `operationId`s for endpoints.

### Version 4.3.0

- Improvement: Scripts updated to cater for Default Route Prefix for API Services.

### Version 4.2.6

- Fixed: Under certain circumstances the `Expose as Http Endpoint` scripts would generate incorrect routes.

### Version 4.2.5

- Update: `Expose as Http Endpoint` to have a better default behavior for `Operation`s and `Class Constructors`.
- Update: Duplicate validation for `Command`s and `Query`s on Http route / verb combination.

### Version 4.2.4

- Update: `Expose as Http Endpoint` to have a better default behavior for composite keys.
- Update: `Azure Function Parameter` are now valid targets for `Parameter Settings` stereotypes.

### Version 4.2.3
- Fixed: `Expose as Http Endpoint` script didn't work correctly for `Command`s which were not mapped.
- Fixed: Parameters without their source specified on `GET` and `DELETE` operations would sometimes implicitly have the source set to `From Body`.

### Version 4.2.1

- Fixed: Applying the Api Version Settings stereotype on a Service / Command / Query without a route would cause an error to be thrown.

### Version 4.2.0

- Added OpenAPI concept for specifying Versions to Services, Commands and Queries.

### Version 4.1.2
- Fix: Fixed issue around Rest route generation by convention not working for attributes ending in `ID`.

### Version 4.1.1

- Support default base Path HttpEndpointModelFactory, if you don't want the build in default path.
- `Command`s or `Query`s with the element name in the designer having a `Request` suffix will not include the suffix in the generated controller method name.

### Version 4.1.0

- Support default values parameters for Services, Domain and Http Metadata.

### Version 4.0.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- `IHttpEndpoint` abstraction available from `HttpEndpointModelFactory` which applies implicit rules (like parameter sources, etc) so that logic doesn't need duplicated in different techs and modules.

### Version 3.3.11

- Added the `Serialization Settings` stereotype, which can be applied to `DTO`s and `DTO-Field`s. This is presently only supported by the `Intent.Application.Dtos` module.

### Version 3.3.9

- Update: Http Settings is no longer `Always` applied. This is to ensure that Modules installing certain Technology stacks can control whether these Stereotypes are applied or not. 

### Version 3.3.8

- New: Return Type Mediatype added for service operations to indicate how their response payload should be returned.

### Version 3.3.7

- Added validation on Header names since having spaces is considered invalid for user defined headers.