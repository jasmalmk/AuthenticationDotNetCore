
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;

namespace MvcProject.Controllers
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
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var userClaims = User.Claims;
           var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
           var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

            var result = await GetSecret(accessToken);
            await RefreshAccessToken();
            return View();
        }

        private async Task RefreshAccessToken()
        {
            var serverClient = _httpClentFactory.CreateClient();
            var discoveryDoc = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44309/");

            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var access_token = await HttpContext.GetTokenAsync("access_token");
            var refreshTokenClient = _httpClentFactory.CreateClient();

            var tokenResponse = await refreshTokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest {
            Address=discoveryDoc.TokenEndpoint,
                RefreshToken=refreshToken,
                ClientId="client_id_mvc",
                ClientSecret="client_secret_mvc"
            });

            

            var authInfo = await HttpContext.AuthenticateAsync("Coockie");

            authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);
            authInfo.Properties.UpdateTokenValue("id_token", tokenResponse.IdentityToken);

            await HttpContext.SignInAsync("Coockie", authInfo.Principal, authInfo.Properties);

            var accessTokenDiffrent = !access_token.Equals(tokenResponse.AccessToken);
        }

        public async Task<string> GetSecret(string accessToke)
        {
            var apiClient = _httpClentFactory.CreateClient();
            apiClient.SetBearerToken(accessToke);
            var response = await apiClient.GetAsync("https://localhost:44379/secret");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
