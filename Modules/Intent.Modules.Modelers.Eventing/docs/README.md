# Intent.Modelers.Eventing

This module extends the services designer to support modeling Message-Based Integration.

Update handlers to not  suggest if published

## What is Message-Based Integration / Event Driven Architecture

Message-based integration is a design approach where systems communicate asynchronously by exchanging messages through a message broker, such as RabbitMQ, Kafka, or Azure Service Bus. This approach decouples systems, meaning each service can operate independently, improving scalability, fault tolerance, and flexibility in system design. It allows services to exchange data or trigger actions without being directly dependent on each other’s availability or implementation, fostering resilience and enabling integration across diverse platforms and technologies. Message-based integration is particularly useful in distributed systems where real-time or asynchronous processing is required, such as processing orders, handling events, or coordinating microservices.

More detailed information available [here](https://docs.intentarchitect.com/articles/application-development/modelling/services-designer/message-based-integration-modeling/message-based-integration-modeling.html#publishing-an-integration-message-from-a-command).

## Linking a copy of another application's message

A message is normally modelled once, in the eventing package of the application that owns it, and other applications add a package reference to use it. Some code doesn't work that way: a receiver or subscriber, often an existing (brownfield) one, declares its own class for a message another application publishes or sends. Model that copy in the application's own eventing package and link it to the original:

- Right-click a copied integration event and choose **Link To Published Integration Event** (an Integration Event Subscription), or a copied integration command and choose **Link To Sent Integration Command** (an Integration Command Subscription), then pick the original message.
- The copy's eventing package must reference the eventing package that holds the original, so add that package reference first if it's missing.
- The link always runs from the copy to the original. An event links only to an event and a command only to a command, and each copy links to one original.
- If the original isn't modelled anywhere in the solution, leave the copy unlinked.

The link documents the flow between applications; nothing is generated from it. Under the copy it reads `[subscribes to] <message>`.