using System.Net.Http.Headers;

namespace Front.Auth
{
    public class AuthMessageHandler:DelegatingHandler
    {
        private readonly TokenStorage _tokenStorage;

        public AuthMessageHandler(TokenStorage tokenStorage)
        {
            _tokenStorage = tokenStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("=== AuthMessageHandler HIT ===");

            var token = await _tokenStorage.GetTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
            {
                if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = token.Substring("Bearer ".Length).Trim();

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            Console.WriteLine("Outgoing Authorization: " + request.Headers.Authorization);

            return await base.SendAsync(request, cancellationToken);
        }


    }
}
