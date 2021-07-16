using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Practice.Models;
using Practice.Services;
using PracticeServices;
using AutoMapper;

namespace Practice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllersWithViews();
            //// In production, the Angular files will be served from this directory
            //services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
            //services.AddDbContext<ModelTestContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            //services.AddCors();
            //services.AddMvc();
            services.AddCors();
            services.AddDbContext<ModelTestContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireElevatedRights", policy => policy.RequireRole("Admin"));
            });
            services.AddControllers().ConfigureApiBehaviorOptions(options => { options.SuppressConsumesConstraintForFormFileParameters = true; 
                options.SuppressInferBindingSourcesForParameters = true; 
                options.SuppressModelStateInvalidFilter = true; 
                options.SuppressMapClientErrors = true; 
                options.ClientErrorMapping[404].Link = "https://httpstatuses.com/404";
            }).AddJsonOptions(options =>
                              options.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddSingleton<IItemService,ItemService>();
            services.AddSingleton<IPurchaseService, PurchaseService>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           
           //if (env.IsDevelopment())
           // {
           //     app.UseDeveloperExceptionPage();
           // }
           // else
           // {
           //     app.UseExceptionHandler("/Error");
           //     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
           //     app.UseHsts();
           // }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseMiddleware();
            app.UseRouting();
           // app.UseAuthorization();
            app.UseAuthentication();
            app.UseCors(c => c.AllowAnyOrigin()
                             .AllowAnyHeader()
                              .AllowAnyMethod());
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
