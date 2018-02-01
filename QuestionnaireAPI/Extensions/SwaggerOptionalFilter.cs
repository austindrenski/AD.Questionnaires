using System.Linq;
using JetBrains.Annotations;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace QuestionnairesApi.Extensions
{
    /// <inheritdoc />
    [PublicAPI]
    public class SwaggerOptionalFilter : IOperationFilter
    {
        /// <inheritdoc />
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters is default)
            {
                return;
            }

            foreach (IParameter parameter in operation.Parameters)
            {
                parameter.Required = false;
            }
        }
    }
}