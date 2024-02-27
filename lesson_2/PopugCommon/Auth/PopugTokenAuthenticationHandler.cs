using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthorizationServer
{
    public class PopugTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        private readonly HttpClient client;
        
        public PopugTokenAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor) : base(options, logger, encoder, clock)
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            client = new HttpClient(handler);
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                if (IsAllowAnonymous(Request.HttpContext))
                {
                    return AuthenticateResult.Fail("AllowAnonymous");
                }

                if (Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    var accessToken = authHeader.ToString().Split(' ')[1];

                    var response = await client
                        .GetAsync($"https://host.docker.internal:52999/oauth/validate?access_token={accessToken}");
                    var scope = await response.Content.ReadAsStringAsync();
                    var token = new JwtSecurityToken(accessToken);

                    // scope might be email, etc..
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return AuthenticateResult.Fail("Нет доступа");

                    }

                    var claimsIdentity = new ClaimsIdentity(token.Claims, nameof(PopugTokenAuthenticationHandler));
                    var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);
                    return AuthenticateResult.Success(ticket);

                }
                return AuthenticateResult.Fail("Нет доступа");
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        private bool IsAllowAnonymous(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var retVal = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;

            return retVal;
        }
    }
}
