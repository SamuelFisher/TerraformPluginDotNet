using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Serialization;
using TerraformPluginDotNet.Services;

namespace TerraformPluginDotNet
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            services.AddTransient(typeof(ResourceProviderHost<>));
            services.AddTransient(typeof(IResourceUpgrader<>), typeof(DefaultResourceUpgrader<>));
            services.AddTransient<IDynamicValueSerializer, DefaultDynamicValueSerializer>();
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            lifetime.ApplicationStarted.Register(() =>
            {
                logger.LogInformation("Application started.");
                Console.WriteLine($"1|5|tcp|127.0.0.1:5344|grpc|{Convert.ToBase64String(Program.Cert.RawData)}");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<Terraform5ProviderService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
