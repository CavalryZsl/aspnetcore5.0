using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HealthChecksDemo01
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
            //���ģʽ
            //services.AddHealthChecks();
            //�Զ��彡�����
            services.AddScoped<IDbConnection, Npgsql.NpgsqlConnection>();
            services.AddHealthChecks()
                //������
                .AddNpgSql(Configuration.GetConnectionString("PostgreSql"), tags: new[] { "pgsql" })
                //�Զ���
                .AddPostgre(Configuration.GetConnectionString("Postgre"))
                .AddCheck("health", () => HealthCheckResult.Healthy("Foo is OK!"), tags: new[] { "health" });

            services.AddControllers();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //���ģʽ
            //app.UseHealthChecks("/health");

            //�Ӷ˿�
            //app.UseHealthChecks("/health", 8800);

            //��ǩ
            app.UseHealthChecks("/health", 5000, new HealthCheckOptions()
            {
                //ResultStatusCodes =
                //{
                //    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                //    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                //    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                //},
                AllowCachingResponses = false,
                ResponseWriter = WriteResponse,
                Predicate = (check) => check.Tags.Contains("health")
            });
            app.UseHealthChecks("/readiness", 8800, new HealthCheckOptions
            {
                //�ñ�ǩ�������������֤
                Predicate = (check) => check.Tags.Contains("pg") || check.Tags.Contains("pgsql")
                //���������ɹ�
                //Predicate = (check) => check.Tags.Contains("pgsql")
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        /// <summary>
        /// ��������״̬
        /// </summary>
        /// <param name="httpContext">������</param>
        /// <param name="result">��������</param>
        /// <returns></returns>
        private static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";
            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));
            return httpContext.Response.WriteAsync(
                json.ToString(Formatting.Indented));
        }
    }
}
