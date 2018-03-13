using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Autofac;
using Autofac.Extensions.DependencyInjection;

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
            app.UseJwtTokenMiddleware();
            app.UseMvc();
           
        }


    }
}
