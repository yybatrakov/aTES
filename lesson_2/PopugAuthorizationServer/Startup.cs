using System;
using System.Collections.Generic;
using System.Text;
using AuthorizationServer.Controllers;
using AuthorizationServer.Interfaces;
using AuthorizationServer.Persistence;
using AuthorizationServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthorizationServer
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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["secret_key"]));

            services.AddLkdTokenAuthentication();
            
            services.AddHttpClient()
              .AddHttpContextAccessor();
            
            services.AddCors(policies => {
                policies.AddDefaultPolicy(builder => {
                    builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            //Add Sqlite DataBase for demo purpose only.
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("SqliteDb"));
            });
            //Configure the default Identity settings.
            //I will configure password only for demo purpose only.
            var identity = services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                var passwordManager = options.Password;
                passwordManager.RequireDigit = false;
                passwordManager.RequireLowercase = false;
                passwordManager.RequireNonAlphanumeric = false;
                passwordManager.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddUserManager<UserManager<IdentityUser>>()
            .AddSignInManager<SignInManager<IdentityUser>>()
            .AddRoleValidator<RoleValidator<IdentityRole>>()
            .AddRoleManager<RoleManager<IdentityRole>>();
            services.AddTransient<UserLogic>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddControllersWithViews();
            services.AddControllers();
            services.AddSwaggerGen(c => StartupHelpers.IntiSwaggerAuth(c, "Popug oAuth server"));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Popug oAuth server");
                    c.OAuthClientId("BF2C6EC3-338A-4EE3-9D97-F98A2A559186");
                    c.OAuthClientSecret("BF2C6EC3-338A-4EE3-9D97-F98A2A559186");
                    c.OAuthAppName("PopugAuthorizationServer");
                    c.OAuthScopeSeparator(",");
                    c.OAuthUsePkce();
                });

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
