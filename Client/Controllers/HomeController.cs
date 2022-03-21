using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClentFactory;
      

        public HomeController(IHttpClientFactory httpClentFactory)
        {
            _httpClentFactory = httpClentFactory;
            
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
           var serverClient = _httpClentFactory.CreateClient();

            var token = await HttpContext.GetTokenAsync("access_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            serverClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var serverResponse = await serverClient.GetAsync("https://localhost:44394/Secret/index");
            await RefreshAccessToken();
            var apiClient = _httpClentFactory.CreateClient();
            token = await HttpContext.GetTokenAsync("access_token");
            apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var ApiResponse = await apiClient.GetAsync("https://localhost:44380/Secret/index");
            return View();
        }

        public async Task<HttpResponseMessage> RefreshAccessToken()
        {
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var refreshTokenClient = _httpClentFactory.CreateClient();

            var requestData = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44394/oauth/token") { 
                Content=new FormUrlEncodedContent(requestData)
            };

            var basicCredentials = "username:password";
            var encodedbasicCredentials = Encoding.UTF8.GetBytes(basicCredentials);
            var bse64Cred = Convert.ToBase64String(encodedbasicCredentials);
            request.Headers.Add("Authorization", $"Basic {bse64Cred}");

           var response= await refreshTokenClient.SendAsync(request);
            var responsestring = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responsestring);

            var newAccessToken = responseData.GetValueOrDefault("access_token");
            var newRefrshToken = responseData.GetValueOrDefault("refresh_token");

            var authInfo = await HttpContext.AuthenticateAsync("ClientCoockie");

            authInfo.Properties.UpdateTokenValue("access_token", newAccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", newRefrshToken);

            await HttpContext.SignInAsync("ClientCoockie", authInfo.Principal, authInfo.Properties);

            return "";
        }
    }
}
