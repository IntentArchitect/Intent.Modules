using System.Collections;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Swashbuckle.Security.AuthorizeCheckOperationFilter", Version = "1.0")]

namespace ArchTemplateResearch.Api.Filters
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!HasAuthorize(context))
            {
                return;
            }

            if (operation.Security == null)
            {
                operation.Security = new List<OpenApiSecurityRequirement>();
            }
            var securityRequirement = new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference("Bearer", context.Document), new List<string>() }
            };
            operation.Security.Add(securityRequirement);
        }

        private static bool HasAuthorize(OperationFilterContext context)
        {
            if (context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any())
            {
                return true;
            }

            return context.MethodInfo.DeclaringType != null
                && context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();
        }
    }
}