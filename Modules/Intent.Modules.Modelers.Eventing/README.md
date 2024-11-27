# Intent.Modelers.Eventing

This module extends the services designer to support modeling Message-Based Integration.

Update handlers to not  suggest if published

## What is Message-Based Integration / Event Driven Architecture

Message-based integration is a design approach where systems communicate asynchronously by exchanging messages through a message broker, such as RabbitMQ, Kafka, or Azure Service Bus. This approach decouples systems, meaning each service can operate independently, improving scalability, fault tolerance, and flexibility in system design. It allows services to exchange data or trigger actions without being directly dependent on each other’s availability or implementation, fostering resilience and enabling integration across diverse platforms and technologies. Message-based integration is particularly useful in distributed systems where real-time or asynchronous processing is required, such as processing orders, handling events, or coordinating microservices.

More detailed information available [here](https://docs.intentarchitect.com/articles/application-development/modelling/services-designer/message-based-integration-modeling/message-based-integration-modeling.html#publishing-an-integration-message-from-a-command).