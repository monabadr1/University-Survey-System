using Shared;
using System.Net.Http.Json;

namespace Front.Consumer
{
    public class AuthConsumer
    {
        private readonly HttpClient _http;
        public AuthConsumer(HttpClient http) { 
            _http = http;
        }

        public async Task <string> LoginAsync(LoginDto dto)
        {
            var res = await _http.PostAsJsonAsync("api/Auth/login", dto);
            var content = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }
            var data = await res.Content.ReadFromJsonAsync<LoginResult>();
            return data!.Token;
        }

        public async Task<int> RegisterAsync(RegisterDto dto)
        {
            var res = await _http.PostAsJsonAsync("api/Auth/register", dto);
            res.EnsureSuccessStatusCode();

            var data=await res.Content.ReadFromJsonAsync<RegisterResult>();
            return data!.UserId;
        }
    }
}
