### Version 3.3.11

- Added the `Serialization Settings` stereotype, which can be applied to `DTO`s and `DTO-Field`s. This is presently only supported by the `Intent.Application.Dtos` module.

### Version 3.3.9

- Update: Http Settings is no longer `Always` applied. This is to ensure that Modules installing certain Technology stacks can control whether these Stereotypes are applied or not. 

### Version 3.3.8

- New: Return Type Mediatype added for service operations to indicate how their response payload should be returned.

### Version 3.3.7

- Added validation on Header names since having spaces is considered invalid for user defined headers.