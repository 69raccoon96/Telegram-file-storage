using System;
using System.IO;
using FileStorageAPI.Converters;
using FileStorageAPI.Services;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FileStorageAPI
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Create new instance <see cref="Startup"/>
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "FileStorageAPI", Version = "v1"}); });
            services.ConfigureSwaggerGen(options =>
            {
                var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileStorageAPI.xml");
                options.IncludeXmlComments(xmlPath);
            });
            services.AddSingleton(Configuration);
            RegisterDtoConverters(services);
            RegisterInfoStorage(services);
            RegisterApiServices(services);
        }

        
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileStorageAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static void RegisterDtoConverters(IServiceCollection services)
        {
            services.AddSingleton<IChatConverter, ChatConverter>();
        }

        private static void RegisterInfoStorage(IServiceCollection services)
        {
            services.AddSingleton<IDataBaseConfig>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var connectionString = $"Server={config["DbHost"]};" +
                                       $"Username={config["DbUser"]};" +
                                       $"Database={config["DbName"]};" +
                                       $"Port={config["DbPort"]};" +
                                       $"Password={config["DbPassword"]};" +
                                       "SSLMode=Prefer";
                return new DataBaseConfig(connectionString);
            });
            services.AddSingleton<IInfoStorageFactory, InfoStorageFactory>();
        }

        private static void RegisterApiServices(IServiceCollection services)
        {
            services.AddSingleton<IChatService, ChatService>();
        }
    }
}