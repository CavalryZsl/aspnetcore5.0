using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RestfulAPIDemo.Model;

namespace RestfulAPIDemo
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "RestfulAPIDemo", Version = "v1", Contact = new OpenApiContact { Email = "", Name = "RestfulAPIDemo" }, Description = "RestfulAPIDemo Details" });
                var xmlPath = Path.Combine(basePath, "RestfulAPIDemo.xml");
                options.IncludeXmlComments(xmlPath, true);
                options.DocInclusionPredicate((docName, description) => true);

            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                .ActionContext;
                return new UrlHelper(actionContext);
            });

            services
                .AddControllers(configure =>
                {
                    configure.ReturnHttpNotAcceptable = true;
                    configure.MaxModelValidationErrors = 200;
                    configure.ValidateComplexTypesIfChildValidationFails = false;
                })
                .AddXmlSerializerFormatters() //����֧��XML��ʽ�������
                .AddJsonOptions(op => op.JsonSerializerOptions.PropertyNameCaseInsensitive = true);//��Сд��ת��

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //���м��������Action���쳣
                //app.Use(async (context, next) =>
                //{
                //    try
                //    {
                //        await next.Invoke();
                //    }
                //    catch (Exception exc)
                //    {
                //        var logger = app.ApplicationServices.GetService(typeof(ILogger<Startup>)) as ILogger;
                //        logger.LogCritical(exc, exc.Message);
                //        var result = "һ��������";
                //        var data = Encoding.UTF8.GetBytes(result);
                //        context.Response.StatusCode = 500;
                //        context.Response.ContentType = "application/json";
                //        context.Response.Body.Write(data, 0, data.Length);
                //    }
                //});
                //ͨ���쳣��������
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = app.ApplicationServices.GetService(typeof(ILogger<Startup>)) as ILogger;
                            logger.LogError(exceptionHandlerFeature.Error.Message);
                        }
                        var result = "һ��������";
                        var data = Encoding.UTF8.GetBytes(result);
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        await context.Response.Body.WriteAsync(data, 0, data.Length);
                    });
                });
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "RestfulAPIDemo";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestfulAPIDemo");
            });
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}