
using System.Text.Json.Serialization;
using BizDbAccess.AppStart;
using BizLogic.AppStart;
using BookApp.Logger;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceLayer.AppStart;

namespace BookApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            var connection = Configuration
                .GetConnectionString("DefaultConnection");

            services.AddDbContext<EfCoreContext>(
                options => options.UseSqlServer(connection));

            services.AddHttpContextAccessor();

            services.RegisterBizDbAccessDi();
            services.RegisterBizLogicDi();
            services.RegisterServiceLayerDi();
        }
        /****************************************************************
        #A This method in the Startup class sets up services
        #B Sets up a series of services to use with controllers and Views
        #C You get the connection string from the appsettings.json file, which can be changed when you deploy.
        #D Configures the application�s DbContext to use SQL Server and provide the connection         
         ****************************************************************/


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            loggerFactory.AddProvider(new RequestTransientLogger(() => httpContextAccessor));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}