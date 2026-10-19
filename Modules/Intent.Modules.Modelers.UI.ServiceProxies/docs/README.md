# Intent.Modelers.UI.ServiceProxies

This module brings the `Service Proxy` element from Intent.Modelers.Types.ServiceProxies into the **User Interface** designer, so a UI application can describe the backend services it calls. Each of a proxy's `Operations` is mapped to an HTTP-exposed endpoint of the target service, such as a Service Operation, Command or Query. Its parameters are kept in sync with that endpoint.

## Creating a Service Proxy

Right-click the **User Interface package** root and select **New Service Proxy**. A mapping dialog opens, where you pick the target service and the endpoints to proxy. Cancelling it before anything is mapped removes the new proxy again.

## Calling a proxy from the UI

Proxy Operations are **Invokable Service Operations**, and they are available as targets of the User Interface designer's **Call Service Operation** mappings. Commands, Queries, DTOs and DTO fields are available as targets there too.

## Parameter synchronisation

Whenever a Service Proxy changes, each Operation's parameters are regenerated from the endpoint it is mapped to.

An Operation that isn't mapped to an endpoint has nothing to sync from. It is skipped, and a warning naming the Operation and its proxy is written to the Task Output. To fix it, delete the Operation, or reopen the proxy's mapping and select the endpoint it should call.
