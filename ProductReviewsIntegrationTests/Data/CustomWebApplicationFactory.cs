using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductReviews;

namespace ProductReviewsIntegrationTests.Data
{
    /// <summary>
    /// Class used to configure a new HttpClient.
    /// </summary>
    /// <typeparam name="TStartup">The Startup file.</typeparam>
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        /// <summary>
        /// This function is used to configure and seed an in memory database and any other startup features.
        /// </summary>
        /// <param name="builder">The WebHostBuilder used to configure required services.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (AppDbContext) using an in-memory database for testing.
                services.AddDbContext<ProductReviews.Context.DbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryAppDb");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var appDb = scopedServices.GetRequiredService<ProductReviews.Context.DbContext>();

                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    // Ensure the database is created.
                    appDb.Database.EnsureCreated();
                }
            }).UseEnvironment("IntegrationTests");
        }
    }
}
