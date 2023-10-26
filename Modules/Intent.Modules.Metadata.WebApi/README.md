# Intent.Metadata.WebApi

This Intent Architect module provides programming language agnostic metadata for use in Intent Architect designers around modelling of "WebApi" concepts, such as verbs, routes and other HTTP related settings for endpoints.

## Parameter Settings stereotype

The Parameter Settings stereotype can be applied to operation `Parameter`s and fields on  `Operation`s, `Query`s, `Command`s and `Azure Function`s to control OpenAPI specific settings of it. It needs to be manually applied when needed.

### Parameter Settings' Source property

Used to specify the source of the paramater from within the HTTP request, possible values are:

- **Default** - A heuristic is used by Intent to make a "best guess" as to what the source should be.
- **From Body**
- **From Form**
- **From Header**
- **From Query**
- **From Route**

### Parameter Settings' Header Name property

(Only visible when [Source](#parameter-settings-source-property) is set to `From Header`.)

Allows you to specify the HTTP request header name as the source for the parameter's value.

### Parameter Settings' Query String Name property

(Only visible when [Source](#parameter-settings-source-property) is set to `From Query`.)

Optional. Allows you to override the name of the HTTP query string name to use as the source for the parameter's value. Defaults to the parameter name when blank.

## OpenAPI Settings' stereotype

![The OpenAPI Settings stereotype](docs/open-api-stereotype.png)

The OpenAPI Settings stereotype can be applied to service `Operation`s, `Query`s, `Command`s and `Azure Function`s to control OpenAPI specific settings of it. It needs to be manually applied when needed.

### OpenAPI Settings' OperationId property

Allows controlling the [`operationId`](https://swagger.io/docs/specification/paths-and-operations/) for the endpoint. When blank it will use the endpoint's "method" name by default.
