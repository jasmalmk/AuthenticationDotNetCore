using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.AuthRequirements
{
    public class JwtAuthRequirement : IAuthorizationRequirement
    {
    }
    public class JwtAuthRequirementHandler : AuthorizationHandler<JwtAuthRequirement>
    {
        private readonly HttpClient _client;
        private readonly HttpContext _httpContext;

        public JwtAuthRequirementHandler(
            IHttpClientFactory httpClientFactory
            ,IHttpContextAccessor httpContextAccessor)
        {
            _client = httpClientFactory.CreateClient();
            _httpContext = httpContextAccessor.HttpContext;
        }
        protected override async Task HandleRequirementAsync
            (AuthorizationHandlerContext context,
            JwtAuthRequirement requirement)
        {
           if(_httpContext.Request.Headers.TryGetValue("Authorization",out var authHeader))
            {
                var accesToken = authHeader.ToString().Split(' ')[1];
                var response = await _client.GetAsync($"https://localhost:44394/Oauth/Validate?access_token={accesToken}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }

            } 
        }
    }


}
