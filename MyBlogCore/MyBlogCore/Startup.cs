using Autofac;
using Blog.Core.Extensions.ServiceExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MyBlogCore.AutofacConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlogCore
{
    public class Startup
    {
        protected IHostEnvironment _hostEnvironment { get; }
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            _hostEnvironment = hostEnvironment;

        }

        public IConfiguration Configuration { get; }
        private readonly string apiName = "MyBlog.Core";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSqlsugarSetup(Configuration);
            services.AddControllers().AddControllersAsServices();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = $"{apiName} 接口文档——dotnetcore 5.0",
                    Description = $"{apiName} HTTP API V1",
                                                           //Contact = new OpenApiContact { Name = apiName, Email = "2334344234@163.com" },//编辑联系方式
                    License = new OpenApiLicense { Name = apiName }//编辑许可证
                });
                options.OrderActionsBy(o => o.RelativePath);
                options.IncludeXmlComments(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "MyBlogCore.xml"), true); // 把接口文档的路径配置进去。第二个参数表示的是是否开启包含对Controller的注释容纳【第二个参数可以不加】
                options.IncludeXmlComments(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Blog.Core.Model.xml"), true);
                options.IncludeXmlComments(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Blog.Core.Entities.xml"), true);
            });
            services.AddRazorPages();
        }
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<ConfigureAutofac>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
                //app.UseSwaggerUI(optins => { optins.  });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                   name: "default-api",
                   pattern: "api/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
        }
    }
}
