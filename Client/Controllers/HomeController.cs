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
            var serverResponse = await AccessTokenRefreshWrapper(() => SecuredGetRequest("https://localhost:44394/Secret/index"));

            var apiResponse = await AccessTokenRefreshWrapper(() => SecuredGetRequest("https://localhost:44380/Secret/index"));


            return View();
        }
        public async Task<HttpResponseMessage> SecuredGetRequest(string url)
        {

            var token = await HttpContext.GetTokenAsync("access_token");
            var _client = _httpClentFactory.CreateClient();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            return await _client.GetAsync(url);
        }
            public async Task<HttpResponseMessage> AccessTokenRefreshWrapper(
               Func<Task<HttpResponseMessage>> initialRequest)
        {
            var response = await initialRequest();
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await RefreshAccessToken();
                response = await initialRequest();
            }
            return response;
        }

        private async Task RefreshAccessToken()
        {
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var refreshTokenClient = _httpClentFactory.CreateClient();

            var requestData = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44394/oauth/token")
            {
                Content = new FormUrlEncodedContent(requestData)
            };

            var basicCredentials = "username:password";
            var encodedbasicCredentials = Encoding.UTF8.GetBytes(basicCredentials);
            var bse64Cred = Convert.ToBase64String(encodedbasicCredentials);
            request.Headers.Add("Authorization", $"Basic {bse64Cred}");

            var response = await refreshTokenClient.SendAsync(request);
            var responsestring = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responsestring);

            var newAccessToken = responseData.GetValueOrDefault("access_token");
            var newRefrshToken = responseData.GetValueOrDefault("refresh_token");

            var authInfo = await HttpContext.AuthenticateAsync("ClientCoockie");

            authInfo.Properties.UpdateTokenValue("access_token", newAccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", newRefrshToken);

            await HttpContext.SignInAsync("ClientCoockie", authInfo.Principal, authInfo.Properties);
        }
    }
}
