namespace AuthorizationServer.Services
{
    using AuthorizationServer.Interfaces;
    using AuthorizationServer.Persistence;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public class TokenGenerator : ITokenGenerator
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SymmetricSecurityKey key;
        private readonly IConfiguration configuration;
        public TokenGenerator(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["secret_key"]));
        }
        public async Task<string> GenerateAccessToken(IdentityUser user)
        {
            var claim = new List<Claim>
            {
                new Claim("_cuser",user.Id),
                new Claim("_unid",user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var descriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = credentials,
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = "popugAuthorizationServer"


            };
            var tokenHanlder = new JsonWebTokenHandler();
            var securityToken = tokenHanlder.CreateToken(descriptor);
            return tokenHanlder.CreateToken(descriptor);
        }


        public string GenerateRefreshToken(IdentityUser user)
        {
            var firstToken = $"{Guid.NewGuid()}-{DateTime.UtcNow}";
            var firstEncoded = Encoding.UTF8.GetBytes(firstToken);
            var firstPart = Convert.ToBase64String(firstEncoded);

            return firstPart + Guid.NewGuid().ToString() + "-" + Convert.ToBase64String(Encoding.UTF8.GetBytes(user.UserName));
        }

        public bool Validate(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler()
               .ValidateToken(token, new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = false,
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                   {
                       var jwt = new JwtSecurityToken(token);

                       return jwt;
                   },
               }, out var securityToken);

                return true;
            }
            catch (Exception ex) when(ex is SecurityTokenExpiredException)
            {

                return false;
            }
        }
        public ClaimsPrincipal DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler()
                .ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = key,
                    SaveSigninToken = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                }, out var securityToken);
            
            return tokenHandler;
        }

    }
}
