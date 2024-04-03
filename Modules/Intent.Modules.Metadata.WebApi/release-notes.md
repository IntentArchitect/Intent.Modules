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