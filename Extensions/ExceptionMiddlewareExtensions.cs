using Microsoft.AspNetCore.Builder;
using ProductReviews.CustomExceptionMiddleware;

namespace ProductReviews.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
