using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ProjectManagementSystem.Api.Infrastructure.Swagger;

namespace ProjectManagementSystem.Api.Infrastructure
{
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Adds the swagger generation.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
        {
            var assemblyName = typeof(Startup).Assembly.GetName();

            services.AddSwaggerGen(c =>
            {
                var docInfos = GetSwaggerDocInfo();
                foreach (var di in docInfos)
                {
                    c.SwaggerDoc(di.Item1,
                        new OpenApiInfo
                        {
                            Version = "v1",
                            Title = $"{assemblyName.Name} {di.Item1}",
                            Description = di.Item2
                        });
                }

                var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                foreach (var fi in dir.EnumerateFiles("*.xml"))
                {
                    c.IncludeXmlComments(fi.FullName);
                }

                c.OperationFilter<OperationNameFilter>();
                c.EnableAnnotations();
            });

            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        /// <summary>
        /// Uses the swagger UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>IApplicationBuilder.</returns>
        public static IApplicationBuilder UseSwaggerUi(this IApplicationBuilder app, IConfiguration configuration)
        {
            var swaggerEndpointPath = configuration["Endpoints:Swagger"];

            if (!string.IsNullOrWhiteSpace(swaggerEndpointPath))
            {
                app.UseSwaggerUI(swaggerUiOptions =>
                {
                    swaggerUiOptions.RoutePrefix = "swagger";

                    swaggerUiOptions.SwaggerEndpoint(swaggerEndpointPath.Replace("{SwaggerDoc}", "Project"), "Project");
                });
            }

            return app;
        }

        /// <summary>
        /// Gets the list of pair name and description for Swagger Doc.
        /// </summary>
        /// <returns>IEnumerable&lt;System.ValueTuple&lt;System.String, System.String&gt;&gt;.</returns>
        private static IEnumerable<(string, string)> GetSwaggerDocInfo()
        {
            return new List<(string, string)>
            {
                ("Project", "")
            };
        }
    }
}