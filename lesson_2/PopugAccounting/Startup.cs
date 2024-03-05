using AuthorizationServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PopugAccounting.Logic;
using PopugCommon.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PopugAccounting
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPopugTokenAuthentication();

            services.AddHttpClient()
              .AddHttpContextAccessor();

            //Add Sqlite DataBase for demo purpose only.
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("SqliteDb"));
            }, ServiceLifetime.Singleton);

            services.AddSingleton<KafkaConsumer, TasksEventsConsumer>();

            services.AddHostedService<KafkaConsumersStartupService>();

            services.AddControllers();
            services.AddCors(policies =>
            {
                policies.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            services.AddSwaggerGen(c => StartupHelpers.IntiSwaggerAuth(c, "PopugAccounting"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PopugAccounting v1");
                    c.OAuthClientId("BF2C6EC3-338A-4EE3-9D97-F98A2A559186");
                    c.OAuthClientSecret("BF2C6EC3-338A-4EE3-9D97-F98A2A559186");
                    c.OAuthAppName("PopugAuthorizationServer");
                    c.OAuthScopeSeparator(",");
                    c.OAuthUsePkce();
                });
            }
            app.UseAuthentication();

            app.UseRouting();

            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
