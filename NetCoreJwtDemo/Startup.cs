using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.IdentityModel.Tokens;

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
        public void ConfigureServices(IServiceCollection services)
        {
            //添加redis缓存
            services.AddRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "NetCoreJwtDemo";

            });
            services.AddSingleton(_appConfiguration);
            services.AddMvc();
            //services.AddAuthentication(options => {
            //    options.DefaultAuthenticateScheme = "JwtBearer";
            //    options.DefaultChallengeScheme = "JwtBearer";
            //    }).AddJwtBearer("JwtBearer", options =>
            //        {
            //            options.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];

            //            options.TokenValidationParameters = new TokenValidationParameters
            //            {
            //                // The signing key must match!
            //                ValidateIssuerSigningKey = true,
            //                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"])),

            //                // Validate the JWT Issuer (iss) claim
            //                ValidateIssuer = true,
            //                ValidIssuer = _appConfiguration["Authentication:JwtBearer:Issuer"],

            //                // Validate the JWT Audience (aud) claim
            //                ValidateAudience = true,
            //                ValidAudience = _appConfiguration["Authentication:JwtBearer:Audience"],

            //                // Validate the token expiry
            //                ValidateLifetime = true,

            //                // If you want to allow a certain amount of clock drift, set that here
            //                ClockSkew = TimeSpan.Zero
            //            };

            //        });
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseAuthentication();//app.UseMiddleware<AuthenticationMiddleware>();

            app.UseJwtTokenMiddleware();
            app.UseMvc();
           
        }
    }
}
