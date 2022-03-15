using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public class OauthController : Controller
    {
        [HttpGet]
        public IActionResult Authorizae(
            string response_type , //authrizan flow type
         string client_id,
         string redirect_uri,
         string scope,//what info i need
         string state //confirm back to same client
            )
        {
            var qry = new QueryBuilder();
            qry.Add("redirectUri", redirect_uri);
            qry.Add("state", state);
            return View(model:qry.ToString());
        }

        [HttpPost]
        public IActionResult Authorizae(string username, 
         string redirectUri,
         string state)
        {
            const string code = "BABABABAVABAB";
            var qry = new QueryBuilder();
            qry.Add("code", code);
            qry.Add("state", state);
            return Redirect($"{redirectUri}{qry.ToString()}");
        }

     
        public async Task<IActionResult> Token(
            string grant_type,
            string code,
            string redirect_uri,
            string client_id
            )
        {
            //affter mechanism 4 validating code
            var claims = new[]
           {
                new Claim(JwtRegisteredClaimNames.Sub,"some_id"),
                new Claim("Granny","coockie")
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constatnts.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;
            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                Constatnts.Issuer,
                Constatnts.Audiance,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials);
            var access_token = new JwtSecurityTokenHandler().WriteToken(token);
            var responseObject = new
            {
                access_token,
                token_type="Bearer",raw_Claim="oauthTutorial"
            };
            var responseJson = JsonConvert.SerializeObject(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(responseJson);
            await Response.Body.WriteAsync(responseBytes,0, responseBytes.Length);
            return Redirect(redirect_uri);
            
        }
    }
}
