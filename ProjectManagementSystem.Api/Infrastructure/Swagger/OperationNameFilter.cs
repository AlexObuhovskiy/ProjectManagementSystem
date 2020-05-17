using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectManagementSystem.Api.Infrastructure.Swagger
{
    /// <summary>
    /// Class OperationNameFilter.
    /// Implements the <see cref="IOperationFilter" />
    /// </summary>
    /// <seealso cref="IOperationFilter" />
    public class OperationNameFilter : IOperationFilter
    {
        /// <summary>
        /// Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation</param>
        /// <param name="context">The context</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.OperationId = context.MethodInfo.Name;
        }
    }
}