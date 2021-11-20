using Invoices.Helpers.Concrete;
using Invoices.Helpers.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using ProductReviews.CustomExceptionMiddleware;
using ProductReviews.Repositories.Concrete;
using ProductReviews.Repositories.Interface;
using System;

namespace ProductReviews
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddDbContext<Context.Context>(options => options.UseSqlServer
            (Configuration.GetConnectionString("ProductReviewsConnectionString")));

            services.AddControllers().AddNewtonsoftJson(j =>
            {
                j.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddMemoryCache();

            /*if(_environment.IsDevelopment())
            {
                //services.AddScoped<IProductReviewsRepository.>
            }
            else
            {
                services.AddSingleton<IProductReviewsRepository, FakeProductReviewsRepository>();
            }*/

            services.AddSingleton<IProductReviewsRepository, FakeProductReviewsRepository>();
            services.AddSingleton<IMemoryCacheAutomater, MemoryCacheAutomater>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMemoryCacheAutomater memoryCacheAutomater)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {

            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            memoryCacheAutomater.AutomateCache();
        }
    }
}
