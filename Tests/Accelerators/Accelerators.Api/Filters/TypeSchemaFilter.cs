using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Swashbuckle.TypeSchemaFilter", Version = "1.0")]

namespace Accelerators.Api.Filters
{
    public class TypeSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(TimeSpan) || context.Type == typeof(TimeSpan?))
            {
                schema.Example = new OpenApiString("00:00:00"); // Set your desired format here
                schema.Type = "string"; // Override the default representation to be a string
            }

            if (context.Type == typeof(DateOnly) || context.Type == typeof(DateOnly?))
            {
                schema.Example = new OpenApiString(DateTime.Today.ToString("yyyy-MM-dd")); // Set your desired format here
                schema.Type = "string"; // Override the default representation to be a string
            }
        }
    }
}