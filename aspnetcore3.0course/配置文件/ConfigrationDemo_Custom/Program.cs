using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConfigrationDemo_Custom
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>//webBuilder������(host)���ù�������
                {

                    webBuilder.ConfigureAppConfiguration((context, config) => //config��Ӧ��(application)���ù�������
                    {
                        config.SetBasePath(Directory.GetCurrentDirectory());
                        config.AddYamlFile("appsettings.yaml");
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
