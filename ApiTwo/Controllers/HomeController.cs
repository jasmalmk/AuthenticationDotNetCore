using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
namespace ApiTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("/")]
      
        public async Task<IActionResult> Index()
        {
            //retrive access token
            var serverClient = _httpClientFactory.CreateClient();
            var discoveryDoc = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44309/");

            var tokenResponse = await serverClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                   
                    Address = discoveryDoc.TokenEndpoint,
                    ClientId = "client_id",
                    ClientSecret = "client_secret",
                    Scope = "ApiOne"
                });
            //get secret data
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);
            var response = await apiClient.GetAsync("https://localhost:44379/secret");
            var content = await response.Content.ReadAsStringAsync();
            return Ok(new
            {
                accesss_token=tokenResponse.AccessToken,
                message=content
            });
        }
    }
}
