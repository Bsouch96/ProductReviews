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
            if(_environment.IsDevelopment())
            {
                services.AddDbContext<Context.DbContext>(options => options.UseSqlServer("localhost"));
            }
            else if(_environment.IsStaging() || _environment.IsProduction())
            {
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
            }

            SetupAuth(services);

            services.AddControllers().AddNewtonsoftJson(j =>
            {
                j.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            if (_environment.IsDevelopment())
            {
                services.AddSingleton<IProductReviewsRepository, FakeProductReviewsRepository>();
            }
            else
            {
                services.AddScoped<IProductReviewsRepository, SqlProductReviewsRepository>();
            }

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
            else if(env.IsStaging() || env.IsProduction())
            {
                app.ConfigureCustomExceptionMiddleware();
                context.Database.Migrate();
                memoryCacheAutomater.AutomateCache();
            }
            else if (_environment.IsEnvironment("IntegrationTests"))
            {
                app.ConfigureCustomExceptionMiddleware();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void SetupAuth(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = $"https://{Configuration["Auth0:Domain"]}/";
                options.Audience = Configuration["Auth0:Audience"];
            });

            services.AddAuthorization(o =>
            {
                o.AddPolicy("ReadReviews", policy =>
                    policy.RequireClaim("permissions", "read:product_reviews"));
                
                o.AddPolicy("ReadVisibleReviews", policy =>
                    policy.RequireClaim("permissions", "read:visible_product_reviews"));

                o.AddPolicy("ReadReview", policy =>
                    policy.RequireClaim("permissions", "read:product_review"));

                o.AddPolicy("CreateReview", policy =>
                    policy.RequireClaim("permissions", "add:product_review"));

                o.AddPolicy("UpdateReview", policy =>
                    policy.RequireClaim("permissions", "edit:product_review"));
            });
        }
    }
}
