using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy.Owin;
using OsmiumMine.Core.Configuration;
using OsmiumMine.Core.Server.Configuration;
using System.IO;

namespace OsmiumMine.Core.Server
{
    public class Startup
    {
        public const string ServerParametersConfigurationFileName = "omserver.json";
        public const string ServerStateStorageFileName = "omserver_state.lidb";

        private readonly IConfigurationRoot config;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                              .AddJsonFile(ServerParametersConfigurationFileName,
                                optional: true,
                                reloadOnChange: true)
                              .SetBasePath(env.ContentRootPath);

            config = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Adds services required for using options.
            services.AddOptions();
            // Register IConfiguration
            services.Configure<OMServerParameters>(config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Create default server parameters
            var serverParameters = new OMServerParameters
            {
                OMConfig = new OMDatabaseConfiguration()
            };
            // Bind configuration file data to server parameters
            config.Bind(serverParameters);
            // Create a server context from the parameters
            var serverContext = OMServerConfigurator.CreateContext(serverParameters);
            // Load persistent state data
            OMServerConfigurator.LoadState(serverContext, ServerStateStorageFileName);

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new OMCoreServerBootstrapper(serverContext)));
        }
    }
}