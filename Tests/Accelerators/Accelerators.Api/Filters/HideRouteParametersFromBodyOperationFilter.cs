using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi.Models;
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

            // Get all route parameter names (case-insensitive for matching)
            var routeParameters = operation.Parameters
                .Where(p => p.In == ParameterLocation.Path)
                .Select(p => p.Name.ToLowerInvariant())
                .ToHashSet();

            if (routeParameters.Count == 0)
            {
                return;
            }

            // Process each content type in the request body
            foreach (var contentType in operation.RequestBody.Content.Keys.ToList())
            {
                var content = operation.RequestBody.Content[contentType];
                var schema = content.Schema;

                if (schema == null)
                {
                    continue;
                }

                // Handle schema references
                if (schema.Reference != null)
                {
                    // Get the actual schema from the context
                    var schemaRepository = context.SchemaRepository.Schemas;
                    var schemaId = schema.Reference.Id;

                    if (schemaRepository.TryGetValue(schemaId, out var referencedSchema))
                    {
                        schema = referencedSchema;
                    }
                }

                if (schema.Properties == null || !schema.Properties.Any())
                {
                    continue;
                }

                // Find properties that match route parameter names (case-insensitive)
                var propertiesToRemove = schema.Properties.Keys
                    .Where(key => routeParameters.Contains(key.ToLowerInvariant()))
                    .ToList();

                if (propertiesToRemove.Count == 0)
                {
                    continue;
                }

                // Create a new schema with the filtered properties
                var newSchema = new OpenApiSchema(schema);

                // Remove matching properties from the new schema
                foreach (var propertyName in propertiesToRemove)
                {
                    newSchema.Properties.Remove(propertyName);
                    newSchema.Required.Remove(propertyName);
                }

                // Replace the content schema with the new filtered schema
                operation.RequestBody.Content[contentType].Schema = newSchema;
            }
        }
    }
}