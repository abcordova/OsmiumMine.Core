using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy.Owin;
using OsmiumMine.Core.Configuration;
using OsmiumMine.Core.Server.Configuration;

namespace OsmiumMine.Core.Server
{
    public class Startup
    {
        public const string ServerParametersConfigurationFileName = "omserver.json";
        public const string ServerStateStorageFileName = "omserver_state.lidb";

        private readonly IConfigurationRoot _config;
        private OMServerContext _serverContext;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile(ServerParametersConfigurationFileName,
                    true,
                    true)
                .SetBasePath(env.ContentRootPath);

            _config = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Adds services required for using options.
            services.AddOptions();
            // Register IConfiguration
            services.Configure<OMServerParameters>(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IApplicationLifetime applicationLifetime)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // Create default server parameters
            var serverParameters = new OMServerParameters
            {
                OMConfig = new OMDatabaseConfiguration()
            };
            // Bind configuration file data to server parameters
            _config.Bind(serverParameters);
            // Create a server context from the parameters
            _serverContext = OMServerConfigurator.CreateContext(serverParameters);
            // Load persistent state data
            OMServerConfigurator.LoadState(_serverContext, ServerStateStorageFileName);

            // Register shutdown
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new OMCoreServerBootstrapper(_serverContext)));
        }

        private void OnShutdown()
        {
            // Persist server state
            _serverContext.ServerState.Persist();
        }
    }
}