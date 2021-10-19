using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;

namespace WebAppAccountManager.AuthenticationHandlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService _userService;

        public BasicAuthenticationHandler(
            IUserService userService,
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }


        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = "Basic";
            return base.HandleChallengeAsync(properties);
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //Authorization: Basic login:password
            var header = Request.Headers["Authorization"];
            if (header == default(StringValues))
            {
                return AuthenticateResult.NoResult();
            }

            var parsedHeader = AuthenticationHeaderValue.Parse(header);
            var credentials = Encoding.UTF8
                .GetString(Convert.FromBase64String(parsedHeader.Parameter))
                .Split(":", 2);

            var login = credentials.First();
            var password = credentials.Last();

            var user = await _userService.AuthenticateUserAsync(login, password);
            if (user is null)
            {
                return AuthenticateResult.Fail("Login or password is invalid");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
