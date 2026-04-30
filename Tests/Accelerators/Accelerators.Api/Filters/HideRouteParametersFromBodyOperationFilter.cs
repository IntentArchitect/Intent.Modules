using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Swashbuckle.HideRouteParametersFromBodyOperationFilter", Version = "1.0")]

namespace Accelerators.Api.Filters
{
    /// <summary>
    /// Operation filter that removes properties from request body schema when they are already defined as route parameters.
    /// This prevents duplicate documentation of parameters that are supplied via the URL.
    /// </summary>
    public class HideRouteParametersFromBodyOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Only process operations with both route parameters and a request body
            if (operation.Parameters == null || operation.RequestBody?.Content == null)
            {
                return;
            }
            var parameters = operation.Parameters!;
            var requestBody = operation.RequestBody!;

            if (requestBody?.Content == null)
            {
                return;
            }
            var requestBodyContent = requestBody.Content;

            // Get all route parameter names (case-insensitive for matching)
            var routeParameters = parameters
                .Where(p => p.In == ParameterLocation.Path && !string.IsNullOrEmpty(p.Name))
                .Select(p => p.Name!.ToLowerInvariant())
                .ToHashSet();

            if (routeParameters.Count == 0)
            {
                return;
            }

            // Process each content type in the request body
            foreach (var contentType in requestBodyContent.Keys.ToList())
            {
                var content = requestBodyContent[contentType];

                if (content == null)
                {
                    continue;
                }
                var schema = content.Schema;

                if (schema == null)
                {
                    continue;
                }

                // Handle schema references - resolve them from the schema repository
                OpenApiSchema? concreteSchema = null;
                if (schema is OpenApiSchemaReference schemaReference)
                {
                    // Resolve the referenced schema from the schema repository
                    var schemaId = schemaReference.Reference.Id;

                    if (schemaId == null)
                    {
                        continue;
                    }

                    if (context.SchemaRepository.Schemas.TryGetValue(schemaId, out var resolvedSchema))
                    {
                        concreteSchema = resolvedSchema as OpenApiSchema;
                    }
                }
                else if (schema is OpenApiSchema directSchema)
                {
                    concreteSchema = directSchema;
                }

                if (concreteSchema?.Properties == null || !concreteSchema.Properties.Any())
                {
                    continue;
                }

                // Find properties that match route parameter names (case-insensitive)
                var propertyKeysToRemove = concreteSchema.Properties.Keys
                    .Where(key => routeParameters.Contains(key.ToLowerInvariant()))
                    .ToList();

                if (propertyKeysToRemove.Count == 0)
                {
                    continue;
                }

                // Remove matching properties from the schema
                foreach (var propertyKey in propertyKeysToRemove)
                {
                    concreteSchema.Properties.Remove(propertyKey);

                    if (concreteSchema.Required != null)
                    {
                        concreteSchema.Required.Remove(propertyKey);
                    }
                }
            }
        }
    }
}