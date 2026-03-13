using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Front.Auth
{
    public class CustomAuthStateProvider: AuthenticationStateProvider
    {
        private readonly TokenStorage _tokenStorage;

        public CustomAuthStateProvider(TokenStorage tokenStorage    )
        {
            _tokenStorage = tokenStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenStorage.GetTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), authenticationType: "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        public async Task MarkUserAsAuthenticate(string token)
        {
            await _tokenStorage.SetTokenAsync(token);

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(user))
            );
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            return token.Claims;
        }
    }
}
