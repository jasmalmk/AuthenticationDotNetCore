using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Configs
{
    public static class Configuration
    {
      
        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource>
            {
                new ApiResource("ApiOne"),
                new ApiResource("ApiTwo")
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
                new Client
                {
                    ClientId="client_id",
                    ClientSecrets={new Secret("client_secret".ToSha256())},
                    AllowedGrantTypes=GrantTypes.ClientCredentials,

                    AllowedScopes={ "ApiOne" }
                },
                new Client
                {
                    ClientId="client_id_mvc",
                    ClientSecrets={new Secret("client_secret_mvc".ToSha256())},
                    AllowedGrantTypes=GrantTypes.Code,
                   RedirectUris={ "https://localhost:44314/signin-oidc" },
                    AllowedScopes={
                        "ApiOne", 
                        "ApiTwo",
                    IdentityServerConstants.StandardScopes.OpenId,
                    //IdentityServerConstants.StandardScopes.Profile,
                    "rc.scope"
                    },

                    //puts all the claims in the id token
                    //AlwaysIncludeUserClaimsInIdToken=true, 
                    RequireConsent=false,
                    AllowOfflineAccess=true
                }
                ,
                new Client
                {
                    ClientId="client_id_js",
                    AllowedGrantTypes=GrantTypes.Implicit,

                   RedirectUris={ "https://localhost:44365/home/signin" },
                   AllowedCorsOrigins={ "https://localhost:44365" },
                    AllowedScopes={
                         IdentityServerConstants.StandardScopes.OpenId,
                        "ApiOne",
                        "ApiTwo",
                        "rc.scope"
                    },
                    RequireConsent=false,
                    AllowAccessTokensViaBrowser=true,
                    AccessTokenLifetime=1
                }


            };

        internal static List<ApiScope> Scopes = new List<ApiScope> {
            new ApiScope{Name = "ApiOne"},
            new ApiScope{Name = "ApiTwo"},
        };

        public static IEnumerable<IdentityResource> GetIdentityResources() =>
         new List<IdentityResource>
         {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name="rc.scope",
                    UserClaims =
                    {
                        "rc.grandma"
                    }
                }

         };
    }
}
