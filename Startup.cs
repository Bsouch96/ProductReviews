using Invoices.Helpers.Concrete;
using Invoices.Helpers.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using ProductReviews.CustomExceptionMiddleware;
using ProductReviews.Extensions;
using ProductReviews.Models;
using ProductReviews.Repositories.Concrete;
using ProductReviews.Repositories.Interface;
using System;

namespace ProductReviews
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddDbContext<Context.DbContext>(options => options.UseSqlServer
                (Configuration.GetConnectionString("ThamcoConnectionString"),
                    sqlServerOptionsAction: sqlOptions => sqlOptions.EnableRetryOnFailure
                    (
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(2),
                        errorNumbersToAdd: null
                    )
                )
            );

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = $"https://{Configuration["Auth0:Domain"]}/";
                options.Audience = Configuration["Auth0:Audience"];
            });

            services.AddControllers().AddNewtonsoftJson(j =>
            {
                j.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            /*if (_environment.IsDevelopment())
            {
                services.AddSingleton<IProductReviewsRepository, FakeProductReviewsRepository>();
            }
            else
            {
                
            }*/
            services.AddScoped<IProductReviewsRepository, SqlProductReviewsRepository>();
            services.AddMemoryCache();
            services.AddSingleton<IMemoryCacheAutomater, MemoryCacheAutomater>();
            services.Configure<MemoryCacheModel>(Configuration.GetSection("MemoryCache"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMemoryCacheAutomater memoryCacheAutomater, Context.DbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.ConfigureCustomExceptionMiddleware();
                context.Database.Migrate();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            memoryCacheAutomater.AutomateCache();
        }
    }
}
