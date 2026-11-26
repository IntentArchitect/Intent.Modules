### Version 1.0.0-pre.0

- New feature: **Manage Gateway Routes...** on a gateway's Services package opens a form listing the HTTP endpoints (Commands, Queries and Operations with **Http Settings**) in the packages the gateway references. Tick an endpoint, or a folder for all of its endpoints, to add its route; untick to remove it. Each routed endpoint is tagged **[SECURED]** or **[UNSECURED]**, and **[RATE LIMITED]**, from the route's Secured, Unsecured and Rate Limit stereotypes. Change those on the route in the designer. When a route's verb or route no longer matches its endpoint, the form offers to update it. Your changes are made as soon as you submit the form, and Ctrl+Z undoes them.
- Fixed: A Route Association to a Command, Query or Operation without **Http Settings** is now flagged with a validation error on the association. Previously it was accepted silently, its script failed partway, and the route showed no verb or route. Pointing an existing route at such an endpoint also clears the verb, route and body it had before.

### Version 1.0.0

Initial release of the API Gateway module, providing tools for managing and configuring API gateways within the Intent platform.
