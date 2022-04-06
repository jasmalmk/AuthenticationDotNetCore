using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcProject
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config=> {
                config.DefaultScheme = "Coockie";
                config.DefaultChallengeScheme = "oidc";

            })
                .AddCookie("Coockie")
                .AddOpenIdConnect("oidc",config=> {
                    config.Authority = "https://localhost:44309/";
                    config.ClientId = "client_id_mvc";
                    config.ClientSecret = "client_secret_mvc";
                    config.SaveTokens = true;
                    config.ResponseType = "code";

                    //coockie claim maping
                    config.ClaimActions.DeleteClaim("amr");
                    config.ClaimActions.MapUniqueJsonKey("RawCoding.Grandma", "rc.grandma");

                    //two trips to load claims in to coockie
                    //but id token will be smaller
                    config.GetClaimsFromUserInfoEndpoint = true;
                    //configure scope
                    config.Scope.Clear();
                    config.Scope.Add("openid");
                    config.Scope.Add("rc.scope");
                    config.Scope.Add("ApiOne");
                    config.Scope.Add("ApiTwo");
                    config.Scope.Add("offline_access");
                });

            services.AddHttpClient();
            services.AddControllersWithViews();
        }

      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
