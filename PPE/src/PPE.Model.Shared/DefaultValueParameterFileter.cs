using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;

namespace PPE.Model.Shared;

public class DefaultValueParameterFileter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema == null) return;
        var objectSchema = schema;
        foreach (var property in objectSchema.Properties)
        {
            if (property.Value.Type == "String" && property.Value.Default == null)
            {
                property.Value.Default = new OpenApiString("");
            }//else if(property.Key=="")

            DefaultValueAttribute? defaultAttribute = context.ParameterInfo?.GetCustomAttribute<DefaultValueAttribute>();
            if (defaultAttribute != null)
            {
                property.Value.Example = (IOpenApiAny)defaultAttribute.Value!;
            }
        }

    }
}