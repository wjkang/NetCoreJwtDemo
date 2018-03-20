using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using NetCoreJwtDemo.Controllers;
using NetCoreJwtDemo.Session;

namespace NetCoreJwtDemo
{
    public class Startup
    {
        private readonly IConfigurationRoot _appConfiguration;
        public Startup(IHostingEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //添加redis缓存
            services.AddRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "NetCoreJwtDemo";

            });
            services.AddSingleton(_appConfiguration);
            services.AddCors(options =>
            {
                options.AddPolicy("NetCoreJwtDemo", builder =>
                {
                    //App:CorsOrigins in appsettings.json can contain more than one address with splitted by comma.
                    builder
                        //.WithOrigins(_appConfiguration["App:CorsOrigins"].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(o => o.RemovePostFix("/")).ToArray())
                        .AllowAnyOrigin() //TODO: Will be replaced by above when Microsoft releases microsoft.aspnetcore.cors 2.0 - https://github.com/aspnet/CORS/pull/94
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            services.AddMvc().AddControllersAsServices();
            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "JwtBearer";
                        options.DefaultChallengeScheme = "JwtBearer";
                    }).AddJwtBearer("JwtBearer", jwtBearerOptions =>
                    {
                        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"])),

                            ValidateIssuer = true,
                            ValidIssuer = _appConfiguration["Authentication:JwtBearer:Issuer"],

                            ValidateAudience = true,
                            ValidAudience = _appConfiguration["Authentication:JwtBearer:Audience"],

                            ValidateLifetime = true, //validate the expiration and not before values in the token

                            ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
                        };
                    });

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.RegisterType<AspNetCorePrincipalAccessor>().As<IPrincipalAccessor>().SingleInstance();
            containerBuilder.RegisterType<ClaimsAbpSession>().As<IAbpSession>().SingleInstance();

            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseAuthentication();
            app.UseCors("NetCoreJwtDemo");
            app.UseJwtTokenMiddleware();
            app.UseMvc();
           
        }


    }
}
