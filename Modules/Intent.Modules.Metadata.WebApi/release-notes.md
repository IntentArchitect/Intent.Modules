### Version 4.2.3
- Fixed: `Expose as Http Endpoint` script didn't work correctly for `Command`s which were not mapped.

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