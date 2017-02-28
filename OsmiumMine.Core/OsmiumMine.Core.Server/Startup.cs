using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nancy.Owin;
using OsmiumMine.Core.Server.Configuration;

namespace OsmiumMine.Core.Server
{
    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                              .AddJsonFile("omserver.json", true)
                              .SetBasePath(env.ContentRootPath);

            config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var appConfig = OMServerConfigurator.GetConfiguration();
            config.Bind(appConfig);

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new OsmiumMineCoreServerBootstrapper(appConfig)));
        }
    }
}