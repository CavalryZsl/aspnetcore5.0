using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DapperExtension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
namespace DapperDemo01
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
            #region SingleDatabase  ע�������ַ���  ע��IDapperPlusDB  ע��IDbConnection
            var connection = string.Format(Configuration.GetConnectionString("Sqlite"), System.IO.Directory.GetCurrentDirectory());
            services.AddSingleton(connection);
            services.AddScoped<IDapperPlusDB, DapperPlusDB>();
            services.AddScoped<IDbConnection, SqliteConnection>();
            #endregion

            #region MultiDatabase  ע��������ݿ����Ӷ���
            //services.AddScoped<IDapperPlusDB, DapperPlusDB>(service =>
            //{
            //    return new DapperPlusDB(new SqliteConnection(string.Format(Configuration.GetConnectionString("Sqlite"), System.IO.Directory.GetCurrentDirectory())));
            //});
            //services.AddScoped<IDapperPlusDB, DapperPlusDB>(service =>
            //{
            //    return new DapperPlusDB(new NpgsqlConnection(Configuration.GetConnectionString("Postgre")));
            //});
            #endregion
            services.AddControllers()
                .AddNewtonsoftJson();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
    }
}
