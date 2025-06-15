using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // التحقق من المعلمات إذا كانت تحتوي على ملف
        var fileParams = context.ApiDescription.ParameterDescriptions
            .Where(p => p.ParameterDescriptor.ParameterType == typeof(IFormFile)).ToList();

        if (fileParams.Count > 0)
        {
            // تجنب إضافة معلمات مكررة
            foreach (var param in fileParams)
            {
                if (!operation.Parameters.Any(p => p.Name == param.Name))
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = param.Name,
                        In = ParameterLocation.Query,
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    });
                }
            }
        }
    }
}
