# Intent.Modelers.Services.DomainInteractions

This module provides designer support for our CRUD Modules.

## Domain Interaction Settings

This section defines how the application interacts with the Domain Layer. Each setting influences how data is mapped, queried, and updated within the domain.

### Default Mapping Mode

The Intent Architect mapping mode to use. Defaults to `Advanced Mapping`. Generally should not be changed.

### Default Query Implementation

Determines how entities are retrieved from the Domain Layer and mapped to DTOs.

- **Default**: Uses `AutoMapper`'s traditional mapping method, leveraging **Intent Architect** mapping configurations to map Domain Entities to DTOs.  
- **Project To**: Uses `AutoMapper ProjectTo`, enabling optimized database queries by mapping directly within LINQ expressions based on **Intent Architect** configurations. 

### Null Child Update Implementation

Controls how **null child entities** in a request are handled when updating the Domain Layer.  

- **Ignore**  
  If a `NULL` child entity is present in a service request, the existing child entity in the domain remains unchanged.  

  ``` csharp
  // if a NULL ProductDetails is received, existing ProductDetails details will remain unchanged
  if (request.ProductDetails != null)
  {
      entity.ProductDetails ??= new Domain.Entities.ProductDetails();
      entity.ProductDetails.Name = request.ProductDetails?.Name;
      entity.ProductDetails.Qty = request.ProductDetails?.Qty;
  }
  ```

- **Set to NULL** (Default behavior)  
  If a `NULL` child entity is present in a service request, the corresponding child entity in the domain is explicitly set to `NULL`.  
  
  ``` csharp
  // if a NULL ProductDetails is received, existing ProductDetails details will be set to null
  if (request.ProductDetails != null)
  {
      entity.ProductDetails ??= new Domain.Entities.ProductDetails();
      entity.ProductDetails.Name = request.ProductDetails?.Name;
      entity.ProductDetails.Qty = request.ProductDetails?.Qty;
  }
  else
  {
    entity.ProductDetails = null;
  }
  ```

## Pagination

When modeling your services any `Query` and/or service `Operation` which return `PagedResult<T>` will be realized as a paginated implementation.

![Sample Query](images/sample-query.png)

A paginated end point would typically have the following parameters:

- `PageNo` (int), the page you wish to return data from. (This sequence is 1 based i.e. 1,2,3...)
- `PageSize` (int), the no of items to return per page.
- `OrderBy` (string), string based order by clause.

If you would like 0 based page Indexing (0,1,2,...) replace `PageNo` with `PageIndex`.

If you want don't want to specify and `order by` clause, you can remove this parameter from your `Query` or service `Operation`.

### string based order by clause examples

Given a Customer with a Name and Surname these would all be valid order by clauses.

- "Name" - order by Name ascending
- "Surname" - order by Surname ascending
- "Name asc" - order by Surname ascending
- "Surname desc" - order by Surname descending
- "Surname desc, Name asc" - order by Surname descending, then Name ascending

You can use the `Paginate` context menu option on `Query` and/or service `Operation` elements to easily configure these endpoints.
