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