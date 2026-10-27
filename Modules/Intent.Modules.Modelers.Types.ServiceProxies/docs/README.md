# Intent.Modelers.Types.ServiceProxies

This module adds the `Service Proxy` element to the **Services** designer. A Service Proxy describes a client for another Intent Architect application's services. Each of its `Operations` is mapped to an HTTP-exposed endpoint of the target service, such as a Service Operation, Command or Query. Its parameters are kept in sync with that endpoint.

## Creating a Service Proxy

Right-click one of these and select **New Service Proxy** (`ctrl + shift + p` in the tree):

- The **Services package** root
- A **Folder**
- A **diagram** canvas. The proxy is created in the folder or package that owns the diagram, and placed where you right-clicked.

A mapping dialog then opens, where you pick the target service and the endpoints to proxy. Cancelling it before anything is mapped removes the new proxy again.

Service Proxies can also be created in a dedicated **Service Proxies** package.

## Operation names

Operations in a proxy can share a name, for example two `GetById` queries from different folders of the target service. Generated .NET clients name their methods after the endpoint each Operation is mapped to, not after the Operation.

Each Operation in a proxy must be mapped to a different endpoint. Two Operations mapped to the same one are reported as a validation error on both; delete one of them.

## Parameter synchronisation

When the designer loads, and whenever a Service Proxy changes, each Operation's parameters are regenerated from the endpoint it is mapped to.

An Operation that isn't mapped to an endpoint, such as one added with **New Operation** on a proxy, has nothing to sync from. It is skipped, and a warning naming the Operation and its proxy is written to the Task Output. To fix it, delete the Operation, or reopen the proxy's mapping and select the endpoint it should call.
