# Intent.Modelers.Services.ProxyInteractions

Services modeler extensions for describing interactions between an application's services and service proxies.

## Creating services from a Service Proxy

Right-click a Service Proxy and select **Create CQRS Operations** or **Create Service** to create a Command or Query, or a Service Operation, for each of its Operations. Right-click a single proxy Operation and select **Create CQRS Operation** or **Create Service Operation** to create or update the one for that Operation. Each created element invokes its proxy Operation through a `Perform Invocation` association, with its fields mapped to the Operation's parameters.

The element for an Operation is found again by that `Perform Invocation`, so running an option a second time reuses it, even after it has been renamed. When proxy Operations share a name, each gets its own element, numbered from the second onwards: `GetByIdQuery`, then `GetById2Query`.

Detailed information on `Service Proxies` can be found on the [Modeling Service Proxies page.](https://docs.intentarchitect.com/articles/application-development/modelling/proxy-designer/proxy-designer.html)
