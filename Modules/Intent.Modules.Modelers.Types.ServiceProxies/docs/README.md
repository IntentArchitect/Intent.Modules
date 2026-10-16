# Intent.Modelers.Types.ServiceProxies

This module adds the `Service Proxy` element to the **Services** designer. A Service Proxy describes a client for another Intent Architect application's services. Each of its `Operations` is mapped to an HTTP-exposed endpoint of the target service, such as a Service Operation, Command or Query. Its parameters are kept in sync with that endpoint.

## Creating a Service Proxy

Right-click one of these and select **New Service Proxy** (`ctrl + shift + p` in the tree):

- The **Services package** root
- A **Folder**
- A **diagram** canvas. The proxy is created in the folder or package that owns the diagram, and placed where you right-clicked.

A mapping dialog then opens, where you pick the target service and the endpoints to proxy. Cancelling it before anything is mapped removes the new proxy again.

Service Proxies can also be created in a dedicated **Service Proxies** package.
