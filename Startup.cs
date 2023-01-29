using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using MarpajarosTPVAPI.Context;
using MarpajarosTPVAPI.Classes;
using MarpajarosTPVAPI.Business;

namespace MarpajarosTPVAPI
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
            services.AddControllers().AddNewtonsoftJson(options => 
            {
                options.UseMemberCasing();
            });
            services.AddDbContext<MarpajarosContext>(options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("MarpajarosTPVAPI")));
            services.TryAddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();
            BS.configuration = Configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Rewrites

            app.UseRouting();
            app.UseAuthorization();
            
            app.Use(async (context, next) => {

                // Añadimos el token al contexto.
                if (!String.IsNullOrEmpty(context.Request.Headers["access-token"])) {
                    context.Items["AccessToken"] = context.Request.Headers["access-token"];
                }
                context.Items["Environment"] = env;
                await next.Invoke();

            });

            app.Use(async (context, next) => {
                var Path = (string)context.Items["RewrittenPath"];
                var OriginalPath = (string)context.Items["OriginalPath"];

                await next.Invoke();

            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = new List<string> { "index.html" }
            });

            System.Web.HttpContext.Configure(app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>());

        }
    }
}
