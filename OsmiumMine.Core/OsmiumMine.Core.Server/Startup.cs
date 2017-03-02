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
        private readonly IConfigurationRoot config;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                              .AddJsonFile("omserver.json",
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

            var serverParameters = new OMServerParameters
            {
                OMConfig = new OMDatabaseConfiguration()
            };
            config.Bind(serverParameters);
            var appConfig = OMServerConfigurator.GetConfiguration(serverParameters);

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new OMCoreServerBootstrapper(appConfig)));
        }
    }
}