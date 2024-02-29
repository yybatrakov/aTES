namespace AuthorizationServer.Controllers
{
    using AuthorizationServer.Interfaces;
    using AuthorizationServer.Models;
    using AuthorizationServer.Persistence;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.JsonWebTokens;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class OAuthController : Controller
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly DataContext dataContext;
        private readonly ITokenGenerator tokenGenerator;
        public OAuthController(
            DataContext dataContext,
            ITokenGenerator tokenGenerator,
            SignInManager<IdentityUser> signInManager)
        {
            this.dataContext = dataContext;
            this.tokenGenerator = tokenGenerator;
            this.signInManager = signInManager;
        }

        //https://localhost:44370/oauth/authorize?client_id=BF2C6EC3-338A-4EE3-9D97-F98A2A559186&scope=user%20phone&response_type=code&redirect_uri=https%3A%2F%2Flocalhost%3A44336%2Foauth%2Fcallback&code_challenge=Pz1aQzMaQj4KEbMTKVZZEpv3j6YJ5cL92nqE2RddU-0&code_challenge_method=S256&state=CfDJ8JM3PxjMXlxBkQqvAKdxJ3h_PxrV1ozJEMJjXg9XQx6x31RQE3qn-GFcemmVlfTVmR7NT7nMcTWjpbZZx-yObKMyQ0xF2gsVgl0Vn0AIO043aLJnwnQLCkdygiEH1wsVYb3ukRypGdW1dJPK_G0LeqGu1S9SvPwT9R6yfWKskF9n_1tnTwkoAZdQyeSnYqeZwVpGe6xa1Gyhrr9xLnXWl68KV4UDB-hBsHBJPtJCBuKiFNNGNoEcJaLzLronaTV6U1Zf6Mo71Y29A5faH6o7wTHFawqhFbrF-7GiEI-s_ICE
        [HttpGet]
        public async Task<IActionResult> AuthorizeAsync(
            string client_id,
            string scope,
            string response_type,
            string redirect_uri,
            string state,
            string code_challenge,
            string code_challenge_method
            )
        {
            var clientid = Guid.Parse(client_id);
            var client = await dataContext.OAuthClients
                .Where(x => x.ClientId == clientid)
                .Include(x=>x.OAuthScopes)
                .FirstOrDefaultAsync();
            if(client is not null)
            {
                ViewData.Add("AppName", client.AppName);
                ViewData.Add("AppWebsite", client.Website);
                var queryBuilder = new QueryBuilder();
                queryBuilder.Add("redirect_uri", redirect_uri);
                queryBuilder.Add("state", state);


                TempData.Add("query", queryBuilder.ToString());
                return View();
              
            }
            return View();


        }
        [HttpPost]
        public async Task<IActionResult> AuthorizeAsync(
            AuthUser authLogin,
            string redirect_uri,
            string state
            )
        {

            var user = await dataContext.Users
                 .Where(x => x.UserName == AuthUserHelper.GetUserFromBeak(authLogin.Beak)).FirstOrDefaultAsync();

            var success = await signInManager.CheckPasswordSignInAsync(user, authLogin.Beak, false);
            if (success.Succeeded)
            {

                string auth_code = Guid.NewGuid().ToString();
                var queryBuilder = new QueryBuilder
                {
                    { "code", auth_code },
                    { "state", state }
                };
                StaticData.CurrentUserName = user.UserName;
                return Redirect($"{redirect_uri}{queryBuilder}");
            }
            else
            {
                TempData.Add("LoginError", "Beak is wrong");
                return View("Authorize", authLogin);
            }
        }
        
        public async Task<IActionResult> TokenAsync(
            string grant_type,
            string code, 
            string redirect_uri,
            string client_id,
            string code_verifier,
            string client_secret
            )
        {
            var clientid = Guid.Parse(client_id);
            var clientsecret = Guid.Parse(client_secret);
            var client = await dataContext.OAuthClients.Where(x => x.ClientId == clientid &&
            x.ClientSecret == clientsecret).AsNoTracking().FirstOrDefaultAsync();
            if(client is not null)
            {

                var user = await dataContext.Users.Where(x => x.UserName == StaticData.CurrentUserName).AsNoTracking().FirstOrDefaultAsync();
                var access_token = await tokenGenerator.GenerateAccessToken(user);
                var refresh_token = tokenGenerator.GenerateRefreshToken(user);
                var responseObject = new
                {
                    access_token,
                    refresh_token,
                    token_type = "Bearer"
                };
                return Ok(responseObject);
            }
            return View();
        }
        [HttpGet]
        [Route("oauth/validate")]
        public IActionResult ValidateAsync(string access_token)
        {
            
            var validation  = tokenGenerator.Validate(access_token);
            return validation ? Ok() : Forbid();

        }
        [HttpGet]
        [Route("oauth/scope")]
        public IActionResult GetScope(string access_token)
        {
            var scope = tokenGenerator.DecodeToken(access_token).Claims.First(x => x.Type == "scope").Value;
            var username = tokenGenerator.DecodeToken(access_token).Claims.First(x => x.Type == "_unid").Value;
            return Ok(new { scope,username});
        }

    }
}
