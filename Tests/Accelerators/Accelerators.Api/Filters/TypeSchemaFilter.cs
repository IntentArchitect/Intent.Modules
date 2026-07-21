using System.Text.Json.Nodes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Swashbuckle.TypeSchemaFilter", Version = "1.0")]

namespace Accelerators.Api.Filters
{
    public class TypeSchemaFilter : ISchemaFilter
    {
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema is OpenApiSchema concreteSchema)
            {
                if (context.Type == typeof(TimeSpan) || context.Type == typeof(TimeSpan?))
                {
                    concreteSchema.Example = JsonValue.Create("00:00:00");
                }

                if (context.Type == typeof(DateOnly) || context.Type == typeof(DateOnly?))
                {
                    concreteSchema.Example = JsonValue.Create(DateTime.Today.ToString("yyyy-MM-dd"));
                }
            }
        }
    }
}