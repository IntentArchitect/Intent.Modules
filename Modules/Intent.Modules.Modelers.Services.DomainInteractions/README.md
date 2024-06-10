# Intent.Modelers.Services.DomainInteractions

Services modeler extensions for describing interactions between an application's services and its domain.

## Mapping from CQRS Operations and Services to Service Proxies

> [!NOTE]
> You will also need version `5.2.0-pre.0` or higher of the `Intent.Modelers.ServiceProxies` module installed.

As usual for _Service Proxies_, you will need to have a reference from the Service Proxies package to the Services package to which you want service proxies created:

![Add Package Reference context menu option in the Service Proxies designer](docs/images/01-service-proxies-add-package-reference.png)

![Selecting the package to reference in the Service Proxies designer](docs/images/02-service-proxies-select-reference.png)

You can then create the Service Proxy and press DONE:

![New Service Proxy context menu option in the Service Proxies designer](docs/images/03-create-service-proxy.png)

![Selecting the Service package reference in the Service Proxies designer](docs/images/04-select-services-to-map.png)

Then from Services designer, add two references, one to the Service Proxies' package, and the other to the domain of the ultimate source application's Domain package:

![Add Package Reference context menu option in the Services designer](docs/images/05-services-add-package-reference.png)

![Selecting the Service Proxies package reference in the Services designer](docs/images/06-services-service-proxy-package-reference.png)

![Selecting the Domain package reference in the Services designer](docs/images/07-services-source-app-domain-package-reference.png)

> [!NOTE]
> It is due a limitation with the current version of Intent Architect that a reference is needed to the ultimate source application's Domain package, a future version of Intent will fix this.

Load the ServiceProxies package into the designer:

![The Load Into Designer context menu option](docs/images/08-services-load-reference-into-designer.png)

If you'd like to visually see the mappings on a diagram, you can create Diagram in the Services designer if you don't have one already open it, and drag the Service Proxy onto it:

![Dragging the Service Proxy onto a diagram in the Services designer](docs/images/09-drag-service-proxy-onto-designer.png)

To create CQRS operations mapped to the service proxy operations, right-click the service proxy and select the _Create CQRS Operations_ option:

![The Create CQRS Operations context menu option on a Service Proxy in the Services designer](docs/images/10-create-cqrs-operations.png)

Which will generate the following:

![The result of using Create CQRS Operations on a Service Proxy in the Services designer](docs/images/11-created-cqrs-operations-result.png)

To create a Service mapped to the service proxy operations, right-click the service proxy and select the _Create Service_ option:

![The Create Services context menu option on a Service Proxy in the Services designer](docs/images/12-create-service.png)

Which will generate the following:

![The result of using Creates Service on a Service Proxy in the Services designer](docs/images/13-create-service-result.png)

When you run the Software Factory, it will now generate implementations for the CQRS Operations or Services which inject the service proxy interface, call the methods and return the result.
