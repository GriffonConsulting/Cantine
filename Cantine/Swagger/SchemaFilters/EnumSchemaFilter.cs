using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Cantine.Swagger.SchemaFilters
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();

                var fields = context.Type.GetFields(BindingFlags.Static | BindingFlags.Public);

                schema.Enum = fields.Select(field => new OpenApiString(field.Name)).Cast<IOpenApiAny>().ToList();
                schema.Type = "string";
                schema.Format = "string";
                schema.Properties = null;
                schema.AllOf = null;
            }
        }
    }
}