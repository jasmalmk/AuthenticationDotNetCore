using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config=>{
                //to confirm we r authenticated
                config.DefaultAuthenticateScheme = "ClientCoockie";
                //sign in deal
                config.DefaultSignInScheme = "ClientCoockie";
                //alowed to do something
                config.DefaultChallengeScheme = "OurServer";

            })
                .AddCookie("ClientCoockie")
                .AddOAuth("OurServer",config=> {
                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";
                    config.CallbackPath = "/oauth/callback";
                    config.AuthorizationEndpoint = "https://localhost:44394/oauth/authorizae";
                    config.TokenEndpoint = "https://localhost:44394/oauth/token";
                    config.SaveTokens = true;

                    config.Events = new OAuthEvents()
                    {
                        OnCreatingTicket=context =>
                        {
                            var accessToken = context.AccessToken;
                            var base64payload = accessToken.Split('.')[1];
                            base64payload = base64payload.Replace('-', '+').Replace('_', '/').PadRight(4 * ((base64payload.Length + 3) / 4), '=');
                            var bytes = Convert.FromBase64String(base64payload);
                            var jsonPayload = Encoding.UTF8.GetString(bytes);
                            var claims = JsonConvert.DeserializeObject<Dictionary<string,string>>(jsonPayload);
                            foreach (var claim in claims)
                            {
                                context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddHttpClient();
            services.AddControllersWithViews().AddRazorRuntimeCompilation(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
