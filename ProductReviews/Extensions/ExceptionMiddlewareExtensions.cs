using Microsoft.AspNetCore.Builder;
using ProductReviews.CustomExceptionMiddleware;

namespace ProductReviews.Extensions
{
    /// <summary>
    /// This class is used to abstract any potential complex logic required to setup the exception middleware.
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// This function is used to setup the exception middleware.
        /// </summary>
        /// <param name="app">The Application Builder</param>
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
